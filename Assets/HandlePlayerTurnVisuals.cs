using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandlePlayerTurnVisuals : MonoBehaviour
{

    [SerializeField] GameObject playerOneTurnVisuals;
    [SerializeField] GameObject playerTwoTurnVisuals;


    private void OnEnable()
    {
        Actions.playerOneTurn += EnablePlayerTurnVisuals;
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
}
