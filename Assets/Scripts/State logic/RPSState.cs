using Newtonsoft.Json.Bson;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class RPSState : IState
{
    private StateManager stateManager;
    private Player player1;
    private Player player2;
    private float StateTime = GameSettings.RPSStateTime;


    public RPSState(StateManager stateManager)
    {
        this.stateManager = stateManager;
        this.player1 = stateManager.player1;
        this.player2 = stateManager.player2;
    }

    private Wand.WandGestures player1Gesture; //variable that stores chosen gesture by player one
    private Wand.WandGestures player2Gesture; //variable that stores chosen gesture by player two

    public void Start()
    {
        Debug.Log("rock paper siscors has started");

        player1Gesture = Wand.WandGestures.nothing;
        player2Gesture = Wand.WandGestures.nothing;

        //ACTIVATE WITH WANTS!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        player1.Wand.StartRecording(player1.GetPlayerNumber(), StateTime);
        player2.Wand.StartRecording(player2.GetPlayerNumber(), StateTime);

        stateManager.StartCoroutine(WaitForChoices());
    }

    public void Update()
    {
        stateManager.timer -= Time.deltaTime;
        stateManager.uiHandler.drawTimer(stateManager.timer);
    }

    public void Exit()
    {

    }

    private IEnumerator WaitForChoices()
    {
        stateManager.timer = StateTime;
        while (stateManager.timer > 0)
        {
            yield return null;


            //CHANGE BACK!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

            if (Input.GetKeyDown(KeyCode.S))
                player2Gesture = Wand.WandGestures.Paper;


            player1Gesture = player1.Wand.GetDetectedGesture();
            player2Gesture = player2.Wand.GetDetectedGesture();


            if (player1Gesture != Wand.WandGestures.nothing && player2Gesture != Wand.WandGestures.nothing)
            {
                Debug.Log("both players chose rock paper or siscor");
                break;
            }
        }

        ResolveRPS();
    }

    private void ResolveRPS()
    {
        if (player1Gesture == Wand.WandGestures.nothing && player2Gesture == Wand.WandGestures.nothing)
        {
            TieRPS();
        }
        else if (player1Gesture != Wand.WandGestures.nothing && player2Gesture == Wand.WandGestures.nothing)
        {
            Player1WinsRPS();
        }
        else if (player1Gesture == Wand.WandGestures.nothing && player2Gesture != Wand.WandGestures.nothing)
        {
            Player2WinsRPS();
        }
        else if (player1Gesture != Wand.WandGestures.nothing && player2Gesture != Wand.WandGestures.nothing)
        {
            if (player1Gesture == player2Gesture)
            {
                TieRPS();
            }
            else if ((player1Gesture == Wand.WandGestures.Scissor && player2Gesture == Wand.WandGestures.Paper) ||
                    (player1Gesture == Wand.WandGestures.Rock && player2Gesture == Wand.WandGestures.Scissor) ||
                    (player1Gesture == Wand.WandGestures.Paper && player2Gesture == Wand.WandGestures.Rock))
            {
                Player1WinsRPS();
            }
            else if ((player1Gesture == Wand.WandGestures.Scissor && player2Gesture == Wand.WandGestures.Rock) ||
                     (player1Gesture == Wand.WandGestures.Rock && player2Gesture == Wand.WandGestures.Paper) ||
                     (player1Gesture == Wand.WandGestures.Paper && player2Gesture == Wand.WandGestures.Scissor))
            {
                Player2WinsRPS();
            }
        }
    }


    private void Player1WinsRPS()
    {
        Debug.Log("player 1 wins rock paper siscor");
        stateManager.SetPlayerTurn(player1);
        stateManager.TransitionToNextState();
    }
    private void Player2WinsRPS()
    {
        Debug.Log("player 2 wins rock paper siscor");
        stateManager.SetPlayerTurn(player2);
        stateManager.TransitionToNextState();
    }
    private void TieRPS()
    {
        Debug.Log("rock paper siscor is a tie");
        stateManager.TransitionToNextState(false, true);
    }
}
