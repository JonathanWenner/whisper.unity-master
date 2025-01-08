using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class HandlePlayerTurnVisuals : MonoBehaviour
{

    [SerializeField] GameObject playerOneTurnVisuals;
    [SerializeField] GameObject playerTwoTurnVisuals;

    [SerializeField] Slider playerOneAttackTimer;
    [SerializeField] Slider playerTwoAttackTimer;


    private void OnEnable()
    {
        Actions.playerOneTurn += EnablePlayerTurnVisuals;

        // Attack
        Actions.playerOneAttack += StartAttackTimerPlayerOne;
        Actions.playerTwoAttack += StartAttackTimerPlayerTwo;

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

    private void EnablePlayerTurnVisuals(bool isPlayerOne)
    {
        playerOneTurnVisuals.SetActive(isPlayerOne);
        playerTwoTurnVisuals.SetActive(!isPlayerOne);
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
