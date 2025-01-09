using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class HandlePlayerTurnVisuals : MonoBehaviour
{

    [Header("Player Turn Visuals")]
    [SerializeField] GameObject playerOneTurnVisuals;
    [SerializeField] GameObject playerTwoTurnVisuals;


    [Header("Player 1")]
    [SerializeField] ParticleSystem SpellActivation;
    [SerializeField] ParticleSystem FireBolt;
    [SerializeField] GameObject ActivationCircle;
    [SerializeField] Image SpellActivationCircle;

    [Header("Player 2")]
    [SerializeField] ParticleSystem SpellActivation2;
    [SerializeField] ParticleSystem FrostBolt2;
    [SerializeField] GameObject ActivationCircle1;
    [SerializeField] Image SpellActivationCircle1;

    [SerializeField] Slider playerOneAttackTimer;
    [SerializeField] Slider playerTwoAttackTimer;


    private void Start()
    {
        OriginalColor = ActivationCircle.GetComponent<SpriteRenderer>().color;
        OriginalColor1 = ActivationCircle1.GetComponent<SpriteRenderer>().color;
    }

    private void OnEnable()
    {
        Actions.playerOneTurn += EnablePlayerTurnVisuals;

        // Attack
        Actions.playerOneAttack += StartAttackTimerPlayerOne;
        Actions.playerTwoAttack += StartAttackTimerPlayerTwo;

        // Attack sucessful
        Actions.playerOneAttackSuccessful += StartAttackSucessful;
        Actions.playerTwoAttackSuccessful += StartAttackSucessful;

        // Defend
        Actions.playerOneDefend += StartAttackTimerPlayerOne;
    }

    private void OnDisable()
    {
        Actions.playerOneTurn -= EnablePlayerTurnVisuals;
    }

    private void StartDefending(bool isPlayerOne)
    {
        
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

    public void StartAttackTimerPlayerOne(bool isPlayerOne)
    {
        GameSettings.AttackStateTime = timer;
        GameSettings.AttackStateTime = playerOneAttackTimer.maxValue;
        isPlayerOneTurn = isPlayerOne;
    }

    private void StartAttackTimerPlayerTwo(bool isPlayerTwo)
    {
        GameSettings.AttackStateTime = timer;
        GameSettings.AttackStateTime = playerTwoAttackTimer.maxValue;
        isPlayerTwoTurn = true;
    }

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

    private IEnumerator FireProjectileCoroutine(bool isPlayerOne)
    {
        // Select the correct prefab, spawn point, target, and speed based on the player
        GameObject projectilePrefab = isPlayerOne ? FireBoltPrefab : FrostBoltPrefab;
        Transform spawnPoint = isPlayerOne ? FireBoltSpawnPoint : FrostBoltSpawnPoint;
        Transform target = isPlayerOne ? FireBoltTarget : FrostBoltTarget;
        float projectileSpeed = isPlayerOne ? FireBoltSpeed : FrostBoltSpeed;

        // Instantiate the projectile at the spawn point with no initial rotation (Quaternion.identity)
        GameObject projectile = Instantiate(projectilePrefab, spawnPoint.position, Quaternion.identity);

        // Set the rotation manually to ensure the correct rotation
        if (isPlayerOne) // FireBolt for Player One
        {
            projectile.transform.rotation = Quaternion.Euler(0, 90, 0); // Set FireBolt to 0, 90, 0
        }
        else // FrostBolt for Player Two
        {
            projectile.transform.rotation = Quaternion.Euler(0, -90, 0); // Set FrostBolt to 0, -90, 0
        }

        // Ensure the projectile moves towards the target
        while (projectile != null && Vector3.Distance(projectile.transform.position, target.position) > 0.1f)
        {
            // Move the projectile towards the target
            projectile.transform.position = Vector3.MoveTowards(
                projectile.transform.position,
                target.position,
                projectileSpeed * Time.deltaTime
            );

            yield return null; // Wait for the next frame
        }

        // Destroy the projectile after reaching the target
        if (projectile != null)
        {
            Destroy(projectile);
        }
    }


    private Coroutine attackCoroutinePlayerOne;
    private Coroutine attackCoroutinePlayerTwo;
    public void StartAttackSucessful()
    {
        if (_isPlayerOne)
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
        }
        else if (!_isPlayerOne)
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
                        FireProjectile(true);
                    }
                    else
                    {
                        SpellActivation2.Play();
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
        if (isPlayerOneTurn)
        {
            if (timer > 0)
            {
                playerOneAttackTimer.value = timer;
                timer -= Time.deltaTime;
            }
            else
            {
                playerOneAttackTimer.value = playerOneAttackTimer.maxValue;
            }
        }
        else
        {
            playerOneAttackTimer.value = playerOneAttackTimer.maxValue;
        }

        if (isPlayerTwoTurn)
        {
            if (timer > 0)
            {
                playerTwoAttackTimer.value = timer;
                timer -= Time.deltaTime;
            }
            else
            {
                playerTwoAttackTimer.value = playerTwoAttackTimer.maxValue;
            }
        }
        else
        {
            playerTwoAttackTimer.value = playerTwoAttackTimer.maxValue;
        }
    }
}
