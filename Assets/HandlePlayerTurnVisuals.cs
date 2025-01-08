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
        Actions.playerOneAttack += StartAttackTimerPlayerOne;
        Actions.playerTwoAttack += StartAttackTimerPlayerTwo;
    }

    private void OnDisable()
    {
        Actions.playerOneTurn -= EnablePlayerTurnVisuals;
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
        timer = 5f;
        isPlayerOneTurn = isPlayerOne;
    }

    private void StartAttackTimerPlayerTwo(bool isPlayerTwo)
    {
        timer = 5f;
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
