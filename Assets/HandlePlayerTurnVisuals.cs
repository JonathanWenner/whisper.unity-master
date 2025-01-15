using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class HandlePlayerTurnVisuals : MonoBehaviour
{

    [SerializeField] StateManager stateManager;

    [SerializeField] CameraShake cameraShake;

    [Header("Lights Move")]
    [SerializeField] Animator[] LightAnimator;

    [Header("BOOK")]
    [SerializeField] Animator Book;
    [SerializeField] TextMeshPro BookText;
    [SerializeField] TextMeshPro BigLetter;

    [Header("Player one")]
    [SerializeField] GameObject playerOneTurnVisuals;
    [SerializeField] CanvasGroup playerOnePortrait;
    [SerializeField] Animator playerOnePortraitAnimator;
    [Header("Player Two")]
    [SerializeField] GameObject playerTwoTurnVisuals;
    [SerializeField] CanvasGroup playerTwoPortrait;
    [SerializeField] Animator playerTwoPortraitAnimator;

    [Header("Player Components")]

    [Header("Player 1")]
    [SerializeField] ParticleSystem SpellActivation;
    [SerializeField] ParticleSystem FireBolt;
    [SerializeField] GameObject ActivationCircle;
    [SerializeField] Image SpellActivationCircle;
    [SerializeField] ParticleSystem ExplosionImpact;
    [SerializeField] Animator FireAnimator;
    [SerializeField] ParticleSystem FireTextParticles;
    [SerializeField] TextMeshPro FireText;
    [Header("TextExplosion")]
    [SerializeField] ParticleSystem TextExplosion1;
    [SerializeField] TextMeshPro TextExplosionText;

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
    [SerializeField] ParticleSystem FrostTextParticles;
    [SerializeField] TextMeshPro FrostText;
    [Header("TextExplosion")]
    [SerializeField] ParticleSystem TextExplosion2;
    [SerializeField] TextMeshPro TextExplosionText2;




    // life 
    [SerializeField] TextMeshProUGUI playerTwoLife;
    [SerializeField] Animator PlayerTwoLoseLife;


    [Header("Environmental Effects")]
    [SerializeField] ParticleSystem DustFromImpact;



    private void Start()
    {
        OriginalColor = ActivationCircle.GetComponent<SpriteRenderer>().color;
        OriginalColor1 = ActivationCircle1.GetComponent<SpriteRenderer>().color;

        playerOneLife.text = "3";
        playerTwoLife.text = "3";

    }

    public void TestChangePlayerTurn()
    {
        _isPlayerOne = !_isPlayerOne;

        EnablePlayerTurnVisuals(_isPlayerOne);
    }

    private void OnEnable()
    {
        Actions.playerOneTurn += EnablePlayerTurnVisuals;

        // Defend
        Actions.StartDefend += InitializeDefend;
        Actions.DefendOutcome += SetDefendingOutcome;
        Actions.EndDefend += ActivateSpellAttack;

        // Attack
        Actions.StartAttack += InitializeFirstAttack;
        Actions.AttackOutcome += SetAttackOutcome;
        Actions.EndAttack += InitializeAttack;

        Actions.PlayerLoseLife += PlayerLosesLife;

        Actions.ResetBackToAttack += IsResetToAttack;
        //Actions.ResetToAttack += StartAttackSucessful;

    }

    private void OnDisable()
    {
        Actions.playerOneTurn -= EnablePlayerTurnVisuals;

        // Defend
        Actions.StartDefend -= InitializeDefend;
        Actions.DefendOutcome -= SetDefendingOutcome;
        Actions.EndDefend -= ActivateSpellAttack;

        Actions.StartAttack -= InitializeFirstAttack;
        Actions.AttackOutcome -= SetAttackOutcome;
        Actions.EndAttack -= InitializeAttack;

        Actions.PlayerLoseLife -= PlayerLosesLife;


    }

    private bool AttackSucessful = false;
    private bool DefendSucessful = false;
    private bool isResetToAttack = false;   

    private void SetDefendingOutcome(bool Success)
    {
        DefendSucessful = Success;
    }
 
    private void SetAttackOutcome(bool Sucess)
    {
        AttackSucessful = Sucess;
    }

    private void IsResetToAttack(bool isReset)
    {
        isResetToAttack = isReset;
    }

    bool FirstAttack = true;

    private void InitializeDefend()
    {


        // When the first player starts in the first round make sure the sequence start
        if (FirstAttack)
        {
            StartAttackOrDefend(_isPlayerOne, true);

        }
        else if (DefendSucessful && !FirstAttack)
        {
            //player failed, start attack
            StartAttackOrDefend(_isPlayerOne, false);        
        }
    }


    private void ActivateSpellAttack()
    {
        if (!DefendSucessful)
        {
            StartAttack();
        }
    }

    private void InitializeFirstAttack()
    {
        //if (FirstAttack)
        //{
        //    FirstAttack = false;
        //    StartCoroutine(StartAttackSequence(!_isPlayerOne));
        //}
    }
    private void InitializeAttack()
    {
        Debug.Log("Attack Sucessful: " + AttackSucessful);
        Debug.Log("player" + (_isPlayerOne ? "One" : "Two") + " Attacked");

        if (!AttackSucessful)
        {
            if (_isPlayerOne)
            {
                Book.Play("FAIL_PlayerOne");
            }
            else if (!isPlayerOneDefending)
            {
                Book.Play("FAIL_PlayerTwo");
            }
        }
        else if (AttackSucessful)
        {
            StartAttackOrDefend(_isPlayerOne, true);
        }


    }

    private int _PlayerOneLife = 3;
    private int _PlayerTwoLife = 3;

    public void PlayerLosesLife()
    { 
        if (_isPlayerOne)
        {
            PlayerOneLoseLife.Play("PlayerOneLoseLife");

            AudioManager.Instance.PlaySound("Ouch", 1f, true);

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

            AudioManager.Instance.PlaySound("Ouch", 1f, true);

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

        if (isPlayerOne)
        {
            playerOnePortraitAnimator.Play("PlayerSelected");
            playerOnePortrait.alpha = 1;
            playerTwoPortrait.alpha = 0.5f;
        }
        else
        {
            playerTwoPortraitAnimator.Play("PlayerSelected");
            playerOnePortrait.alpha = 0.5f;
            playerTwoPortrait.alpha = 1;
        }
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
                AudioManager.Instance.PlaySound("FireHit",1f,false);
                AudioManager.Instance.PlaySound("Shake", 0.7f, true);
                
                TextExplosionText.text = LastSaidWord;
                TextExplosion1.Play();
                ExplosionImpact.Emit(3);
                FireAnimator.SetTrigger("Flame");

                foreach (var lamp in LightAnimator)
                {
                    lamp.SetTrigger("LightShake");
                }

            }
            else if (!isPlayerOne)
            {
                AudioManager.Instance.PlaySound("FrostHit",1f, false);
                AudioManager.Instance.PlaySound("Shake", 0.7f, true);

                TextExplosionText2.text = LastSaidWord;
                TextExplosion2.Play();
                IceImpact.Play();
                FrostAnimator.SetTrigger("Frost");

                foreach (var lamp in LightAnimator)
                {
                    lamp.SetTrigger("LightShake");
                }
            }

            DustFromImpact.Play();

            Destroy(projectile);
        }
    }

    private string GetValidWord(string word, string fallback = "No word")
    {
        return string.IsNullOrEmpty(word) ? fallback : word;
    }

    private Coroutine attackCoroutinePlayerOne;
    private Coroutine attackCoroutinePlayerTwo;
    public void StartAttackOrDefend(bool IsPlayerOne, bool attacking)
    {
        LastSaidWord = GetValidWord(stateManager.getLastSayedWord());
      
        if (FirstAttack && IsPlayerOne && attacking)
        {
            Book.SetTrigger("OpenBookPlayerTwo");
            FirstAttack = false;

            attackCoroutinePlayerOne = StartCoroutine(StartAttackSequence(IsPlayerOne));
        }
        else if(!FirstAttack && IsPlayerOne && !attacking)
        {
            Debug.Log("PLAYER ONE DEFENDED?????");

            if (attackCoroutinePlayerOne != null) StopCoroutine(attackCoroutinePlayerOne);
            if (attackCoroutinePlayerTwo != null) StopCoroutine(attackCoroutinePlayerTwo);

            ResetPlayerVisualsForPlayerOne(true);
            Debug.Log("RESET!! player" + (IsPlayerOne ? "one" : "Two"));

            attackCoroutinePlayerTwo = StartCoroutine(StartAttackSequence(false));

            Book.Play("CloseBookPlayerTwo");

            FrostText.text = LastSaidWord;
            BigLetter.text = LastSaidWord.Substring(0, 1);
            FrostTextParticles.Play();
        }

        if (FirstAttack && !IsPlayerOne && attacking)
        {
            Book.SetTrigger("OpenBookPlayerOne");
            FirstAttack = false;

            attackCoroutinePlayerTwo = StartCoroutine(StartAttackSequence(IsPlayerOne));
        }

        // If player two is defending or Attacking

        else if (!FirstAttack && !IsPlayerOne && !attacking)
        {
            Debug.Log("PLAYER TWO DEFENDDED?????");

            if (attackCoroutinePlayerOne != null) StopCoroutine(attackCoroutinePlayerOne);
            if (attackCoroutinePlayerTwo != null) StopCoroutine(attackCoroutinePlayerTwo);

            Debug.Log("RESET!! player" + (IsPlayerOne ? "one" : "Two"));

            ResetPlayerVisualsForPlayerOne(false);

            attackCoroutinePlayerOne = StartCoroutine(StartAttackSequence(true));

            Book.Play("CloseBookPlayerOne");

            FireText.text = LastSaidWord;
            BigLetter.text = LastSaidWord.Substring(0, 1);
            FireTextParticles.Play();
        }

        BookText.text = LastSaidWord;

        // Play audio
        AudioManager.Instance.PlaySound("MagicCircle", 0.8f, true);

        if (isReversing)
        { 
            ReverseAttackSequence();
        }

    }

    private void StartSuccessfulAttack(bool IsPlayerOne)
    { 
        
    }

    private void ResetPlayerVisualsForPlayerOne(bool isPlayerOne)
    {
        if (isPlayerOne)
        {
            ActivationCircle.GetComponent<SpriteRenderer>().color = OriginalColor;
            SpellActivationCircle.fillAmount = 0;
            AttackOnStandbyEffectPlayerTwo.Stop();

            Debug.Log("ResetVisuals for" + (isPlayerOne ? "one" : "Two"));

        }
        else if (!isPlayerOne)
        {
            ActivationCircle1.GetComponent<SpriteRenderer>().color = OriginalColor1;
            SpellActivationCircle1.fillAmount = 0;
            AttackOnStandbyEffectPlayerOne.Stop();

            Debug.Log("ResetVisuals for" + (isPlayerOne ? "one" : "Two"));
        }
    }

    private Color OriginalColor;
    private Color OriginalColor1;
    [Header("Attack Sequence")]
    public bool isPaused = false;
    public bool isReversing = false;

    [Header("Attack Sequence")]
    [SerializeField] public float reverseSpeedMultiplier = 5f;

    private void StopAttackSequence()
    {
        StopAllCoroutines();

        ResetPlayerVisualsForPlayerOne(true);
        ResetPlayerVisualsForPlayerOne(false);
    }

    private string LastSaidWord;

    private float GestureSpeed = 1f;


    [Header("Attack On Standby effects")]
    [SerializeField] private ParticleSystem AttackOnStandbyEffectPlayerOne;
    [SerializeField] private ParticleSystem AttackOnStandbyEffectPlayerTwo;

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
                    elapsedTime += Time.deltaTime * GestureSpeed;
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

                if (progress >= 1f && !hasLoggedFilled)
                {
                    Debug.Log($"Circle is completely filled for Player {(isPlayerOne ? "One" : "Two")}.");
                    hasLoggedFilled = true;
                    hasLoggedEmpty = false;
                    HandleAttackCompletion(isPlayerOne);
                }
                else if (progress <= 0f && !hasLoggedEmpty)
                {
                    Debug.Log($"Circle is reset to empty for Player {(isPlayerOne ? "One" : "Two")}.");
                    hasLoggedEmpty = true;
                    hasLoggedFilled = false;
                }

                // Wait for the next frame
                yield return null;
            }
        }
    }

    private void HandleAttackCompletion(bool isPlayerOne)
    {
        if (isPlayerOne)
        {
            isPlayerOneDefending = false;
            AttackOnStandbyEffectPlayerTwo.Play();
        }
        else
        {
            isPlayerOneDefending = true;
            AttackOnStandbyEffectPlayerOne.Play();
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

    public bool isPlayerOneDefending = false;

    public void StartAttack()
    {

        if (!isPlayerOneDefending)
        {
            ActivateAttackAgainstPlayerOne();
        }
        else
        {
            ActivateAttackAgainstPlayerTwo();
        }
    }
    public void ActivateAttackAgainstPlayerOne()
    {
        AttackOnStandbyEffectPlayerOne.Stop();
        SpellActivation2.Play();
        AudioManager.Instance.PlaySound("FrostFly", 1f, false);
        AudioManager.Instance.PlaySound("MagicCircleCompleted", 1f, true);
        FireProjectile(false);
        ReverseAttackSequence();
    }

    public void ActivateAttackAgainstPlayerTwo()
    {
        AttackOnStandbyEffectPlayerOne.Stop();
        SpellActivation.Play();
        AudioManager.Instance.PlaySound("FireFly", 1f, false);
        AudioManager.Instance.PlaySound("MagicCircleCompleted", 1f, true);
        FireProjectile(true);
        ReverseAttackSequence();
    }

    private void Update()
    {


        _isPlayerOne = stateManager.isPlayerOneTurn;

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
