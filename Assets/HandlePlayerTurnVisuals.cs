using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class HandlePlayerTurnVisuals : MonoBehaviour
{

    [SerializeField] CameraShake cameraShake;

    [Header("Player Turn Visuals")]
    [SerializeField] GameObject playerOneTurnVisuals;
    [SerializeField] GameObject playerTwoTurnVisuals;


    [Header("Player 1")]
    [SerializeField] ParticleSystem SpellActivation;
    [SerializeField] ParticleSystem FireBolt;
    [SerializeField] GameObject ActivationCircle;
    [SerializeField] Image SpellActivationCircle;
    [SerializeField] ParticleSystem ExplosionImpact;
    [SerializeField] Animator FireAnimator;

    // life 
    [SerializeField] TextMeshProUGUI playerOneLife;
    [SerializeField] Animator PlayerOneLoseLife;

    [Header("Player 2")]
    [SerializeField] ParticleSystem SpellActivation2;
    [SerializeField] ParticleSystem FrostBolt2;
    [SerializeField] GameObject ActivationCircle1;
    [SerializeField] Image SpellActivationCircle1;
    [SerializeField] ParticleSystem IceImpact;
    [SerializeField] Animator FrostAnimator;

    // life 
    [SerializeField] TextMeshProUGUI playerTwoLife;
    [SerializeField] Animator PlayerTwoLoseLife;



    private void Start()
    {
        OriginalColor = ActivationCircle.GetComponent<SpriteRenderer>().color;
        OriginalColor1 = ActivationCircle1.GetComponent<SpriteRenderer>().color;

        playerOneLife.text = "3";
        playerTwoLife.text = "3";

    }

    private void OnEnable()
    {
        Actions.playerOneTurn += EnablePlayerTurnVisuals;

        // Defend
        Actions.StartDefend += InitializeDefend;
        Actions.DefendOutcome += SetDefendingOutcome;
        //Actions.EndDefend += InitializeDefend;

        // Attack
        //Actions.StartAttack += InitializeAttack;
        Actions.AttackOutcome += SetAttackOutcome;
        Actions.EndAttack += InitializeAttack;

        Actions.PlayerLoseLife += PlayerLosesLife;

    }

    private void OnDisable()
    {
        Actions.playerOneTurn -= EnablePlayerTurnVisuals;

        // Defend
        Actions.StartDefend -= InitializeDefend;
        Actions.DefendOutcome -= SetDefendingOutcome;
        //Actions.EndDefend -= InitializeDefend;

        Actions.StartAttack -= InitializeAttack;
        Actions.AttackOutcome -= SetAttackOutcome;
        Actions.EndAttack -= InitializeAttack;

        Actions.PlayerLoseLife -= PlayerLosesLife;


    }

    private bool AttackSucessful = false;
    private bool DefendSucessful = false;

    private void SetDefendingOutcome(bool Success)
    {
        DefendSucessful = Success;
    }
 
    private void SetAttackOutcome(bool Sucess)
    {
        AttackSucessful = Sucess;
    }

    private void InitializeDefend()
    {
        Debug.Log("Defend Sucessful: " + DefendSucessful);
        Debug.Log("player" + (!_isPlayerOne ? "One" : "Two") + " Defended");

        if (DefendSucessful)
        {
            StartAttackSucessful();
        }
        else
        {
            Debug.Log("Unsuccesful Defend");
        }


    }

    private void InitializeAttack()
    {
        Debug.Log("Attack Sucessful: " + AttackSucessful);
        Debug.Log("player" + (_isPlayerOne ? "One" : "Two") + " Attacked");

        if (AttackSucessful)
        {
            StartAttackSucessful();
        }
        else
        { 
            Debug.Log("Unsuccesful Attack");
        }

    }

    private int _PlayerOneLife = 3;
    private int _PlayerTwoLife = 3;

    public void PlayerLosesLife()
    { 
        if (_isPlayerOne)
        {
            PlayerOneLoseLife.Play("PlayerOneLoseLife");

            if (_PlayerOneLife > 0)
            {
                _PlayerOneLife--;
            }
            else
            {
                // Player 1 lost
            }

            playerOneLife.text = _PlayerOneLife.ToString();
        }
        else
        {
            PlayerTwoLoseLife.Play("PlayerOneLoseLife");

            if (_PlayerTwoLife > 0)
            {
                _PlayerTwoLife--;
            }
            else
            {
                // Player 2 lost
            }

            playerTwoLife.text = _PlayerTwoLife.ToString();
        }
    }


    public bool _isPlayerOne = false;

    private void EnablePlayerTurnVisuals(bool isPlayerOne)
    {
        playerOneTurnVisuals.SetActive(isPlayerOne);
        playerTwoTurnVisuals.SetActive(!isPlayerOne);

        _isPlayerOne = isPlayerOne;
    }


    private float timer = 5f;
    private bool isPlayerOneTurn = false;
    private bool isPlayerTwoTurn = false;

    //public void StartAttackTimerPlayerOne(bool isPlayerOne)
    //{
    //    GameSettings.AttackStateTime = timer;
    //    GameSettings.AttackStateTime = playerOneAttackTimer.maxValue;
    //    isPlayerOneTurn = isPlayerOne;
    //}

    //private void StartAttackTimerPlayerTwo(bool isPlayerTwo)
    //{
    //    GameSettings.AttackStateTime = timer;
    //    GameSettings.AttackStateTime = playerTwoAttackTimer.maxValue;
    //    isPlayerTwoTurn = true;
    //}

    private void FireProjectile(bool isPlayerOne)
    {
        StartCoroutine(FireProjectileCoroutine(isPlayerOne));
    }

    [Header("FrostBolt")]
    [SerializeField] private GameObject FrostBoltPrefab;
    [SerializeField] private Transform FrostBoltSpawnPoint;
    [SerializeField] private Transform FrostBoltTarget;
    [SerializeField] private float FrostBoltSpeed = 5f;

    [Header("FireBolt")]
    [SerializeField] private GameObject FireBoltPrefab;
    [SerializeField] private Transform FireBoltSpawnPoint;
    [SerializeField] private Transform FireBoltTarget;
    [SerializeField] private float FireBoltSpeed = 5f;

    [Header("Projectile speed")]
    [SerializeField]
    private AnimationCurve speedCurve; // Define the curve in Unity Inspector to control speed over time
    [SerializeField]
    private float flightDuration = 1f; // Total duration for the projectile's flight


    // Losing life Unsuccesful attack or unsuccesful defense




    private IEnumerator FireProjectileCoroutine(bool isPlayerOne)
    {
        // Select the correct prefab, spawn point, target, and speed based on the player
        GameObject projectilePrefab = isPlayerOne ? FireBoltPrefab : FrostBoltPrefab;
        Transform spawnPoint = isPlayerOne ? FireBoltSpawnPoint : FrostBoltSpawnPoint;
        Transform target = isPlayerOne ? FireBoltTarget : FrostBoltTarget;

        // Instantiate the projectile at the spawn point with no initial rotation (Quaternion.identity)
        GameObject projectile = Instantiate(projectilePrefab, spawnPoint.position, Quaternion.identity);

        // Set the rotation manually to ensure the correct rotation
        if (isPlayerOne)
        {
            projectile.transform.rotation = Quaternion.Euler(0, 90, 0);
        }
        else
        {
            projectile.transform.rotation = Quaternion.Euler(0, -90, 0);
        }

        // Ensure the projectile moves towards the target
        float elapsedTime = 0f; // Track the time elapsed
        while (projectile != null && elapsedTime < flightDuration)
        {
            // Calculate the normalized time (0 to 1)
            float normalizedTime = elapsedTime / flightDuration;

            // Sample the AnimationCurve to get the speed at the current time
            float projectileSpeed = speedCurve.Evaluate(normalizedTime);

            // Move the projectile towards the target
            projectile.transform.position = Vector3.MoveTowards(
                projectile.transform.position,
                target.position,
                projectileSpeed * Time.deltaTime
            );

            elapsedTime += Time.deltaTime; // Increment elapsed time
            yield return null; // Wait for the next frame
        }

        // Destroy the projectile after reaching the target or finishing its flight duration
        if (projectile != null)
        {
            cameraShake.ShakeCamera();

            if (isPlayerOne)
            {
                ExplosionImpact.Play();
                FireAnimator.SetTrigger("Flame");
            }
            else if (!isPlayerOne)
            {
                IceImpact.Play();
                FrostAnimator.SetTrigger("Frost");
            }

            PlayerLosesLife();

            Destroy(projectile);
        }
    }

    private Coroutine attackCoroutinePlayerOne;
    private Coroutine attackCoroutinePlayerTwo;
    public void StartAttackSucessful()
    {
        if (!_isPlayerOne)
        {
            if (attackCoroutinePlayerOne != null)
            {
                StopCoroutine(attackCoroutinePlayerOne);
            }
            if (attackCoroutinePlayerTwo != null)
            {
                StopCoroutine(attackCoroutinePlayerTwo);
            }

            ResetPlayerVisualsForPlayerOne(false);

            attackCoroutinePlayerOne = StartCoroutine(StartAttackSequence(true));

            ReverseAttackSequence();
        }
        else if (_isPlayerOne)
        {
            if (attackCoroutinePlayerTwo != null)
            {
                StopCoroutine(attackCoroutinePlayerTwo);
            }
            if (attackCoroutinePlayerOne != null)
            {
                StopCoroutine(attackCoroutinePlayerOne);
            }

            ResetPlayerVisualsForPlayerOne(true);

            attackCoroutinePlayerTwo = StartCoroutine(StartAttackSequence(false));

            ReverseAttackSequence();
        }

    }

    private void ResetPlayerVisualsForPlayerOne(bool isPlayerOne)
    {
        if (isPlayerOne)
        { 
            ActivationCircle.GetComponent<SpriteRenderer>().color = OriginalColor;
            SpellActivationCircle.fillAmount = 0;
        }
        else if (!isPlayerOne)
        {
            ActivationCircle1.GetComponent<SpriteRenderer>().color = OriginalColor1;
            SpellActivationCircle1.fillAmount = 0;
        }
    }

    private Color OriginalColor;
    private Color OriginalColor1;

    private bool isPaused = false;
    private bool isReversing = false;

    [Header("Attack Sequence")]
    [SerializeField] public float reverseSpeedMultiplier = 5f;

    private void StopAttackSequence()
    {
        StopAllCoroutines();

        ResetPlayerVisualsForPlayerOne(true);
        ResetPlayerVisualsForPlayerOne(false);
    }

    private IEnumerator StartAttackSequence(bool isPlayerOne)
    {
        float elapsedTime = 0f;
        float duration = GameSettings.DefendStateTime;

        bool hasLoggedFilled = false; // To ensure logs are triggered only once
        bool hasLoggedEmpty = false;

        while (elapsedTime >= 0 && elapsedTime <= duration)
        {
            if (!isPaused)
            {
                // Update time
                if (isReversing)
                {
                    elapsedTime -= Time.deltaTime * reverseSpeedMultiplier; // Faster reverse
                }
                else
                {
                    elapsedTime += Time.deltaTime;
                }

                // Clamp elapsedTime to stay within bounds
                elapsedTime = Mathf.Clamp(elapsedTime, 0, duration);

                // Calculate progress
                float progress = elapsedTime / duration;

                // Update visuals
                if (isPlayerOne)
                {
                    UpdateVisuals(ActivationCircle, OriginalColor, SpellActivationCircle, progress);
                }
                else
                {
                    UpdateVisuals(ActivationCircle1, OriginalColor1, SpellActivationCircle1, progress);
                }

                // Debug logging
                if (progress >= 1f && !hasLoggedFilled)
                {
                    Debug.Log($"Circle is completely filled for Player {(isPlayerOne ? "One" : "Two")}.");
                    hasLoggedFilled = true;
                    hasLoggedEmpty = false; // Reset for when reversing

                    if (isPlayerOne)
                    {
                        SpellActivation.Play();
                        yield return new WaitForSeconds(0.5f);
                        FireProjectile(true);
                    }
                    else
                    {
                        SpellActivation2.Play();
                        yield return new WaitForSeconds(0.5f);
                        FireProjectile(false);
                    }

                    ReverseAttackSequence();
                }
                else if (progress <= 0f && !hasLoggedEmpty)
                {
                    Debug.Log($"Circle is reset to empty for Player {(isPlayerOne ? "One" : "Two")}.");
                    hasLoggedEmpty = true;
                    hasLoggedFilled = false; // Reset for when filling again
                }
            }

            // Wait for the next frame
            yield return null;
        }

        // If the sequence finishes naturally
        if (!isReversing && elapsedTime >= duration)
        {
            Debug.Log("Attack sequence completed.");
        }
    }

    // Helper to update visuals
    private void UpdateVisuals(GameObject circleObject, Color originalColor, UnityEngine.UI.Image fillCircle, float progress)
    {
        SpriteRenderer spriteRenderer = circleObject.GetComponent<SpriteRenderer>();
        float newAlpha = Mathf.Lerp(0, 1, progress);
        spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, newAlpha);
        fillCircle.fillAmount = progress;
    }

    // Control Methods
    public void PauseAttackSequence()
    {
        isPaused = true;
    }

    public void ResumeAttackSequence()
    {
        isPaused = false;
    }

    public void ReverseAttackSequence()
    {
        isReversing = !isReversing;
    }

    private void Update()
    {
        //if (isPlayerOneTurn)
        //{
        //    if (timer > 0)
        //    {
        //        playerOneAttackTimer.value = timer;
        //        timer -= Time.deltaTime;
        //    }
        //    else
        //    {
        //        playerOneAttackTimer.value = playerOneAttackTimer.maxValue;
        //    }
        //}
        //else
        //{
        //    playerOneAttackTimer.value = playerOneAttackTimer.maxValue;
        //}

        //if (isPlayerTwoTurn)
        //{
        //    if (timer > 0)
        //    {
        //        playerTwoAttackTimer.value = timer;
        //        timer -= Time.deltaTime;
        //    }
        //    else
        //    {
        //        playerTwoAttackTimer.value = playerTwoAttackTimer.maxValue;
        //    }
        //}
        //else
        //{
        //    playerTwoAttackTimer.value = playerTwoAttackTimer.maxValue;
        //}
    }
}
