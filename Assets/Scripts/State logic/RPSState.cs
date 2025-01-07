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
    private float StateTime = 4f;
    private float timer;


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
        player1Gesture = Wand.WandGestures.nothing;
        player2Gesture = Wand.WandGestures.nothing;

        player1.Wand.StartRecording(player1.GetPlayerNumber(), StateTime);
        player2.Wand.StartRecording(player2.GetPlayerNumber(), StateTime);

        stateManager.StartCoroutine(WaitForChoices());
    }

    public void Update()
    {

    }

    public void Exit()
    {

    }

    private IEnumerator WaitForChoices()
    {
        timer = StateTime;
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            yield return null;


            //CHANGE BACK!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            if (Input.GetKeyDown(KeyCode.A))
                player1Gesture = Wand.WandGestures.Rock;
            if (Input.GetKeyDown(KeyCode.S))
                player2Gesture = Wand.WandGestures.Paper;
            //Wand.WandGestures foundGestureP1 = player1.Wand.GetDetectedGesture();
            //Wand.WandGestures foundGestureP2 = player2.Wand.GetDetectedGesture();


            if (player1Gesture != Wand.WandGestures.nothing && player2Gesture != Wand.WandGestures.nothing)
            {
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
        stateManager.SetPlayerTurn(player1);
        stateManager.TransitionToNextState(stateManager);
    }
    private void Player2WinsRPS()
    {
        stateManager.SetPlayerTurn(player2);
        stateManager.TransitionToNextState(stateManager);
    }
    private void TieRPS()
    {
        stateManager.TransitionToState(stateManager.stateFactory.createState(typeof(RPSState)), true);
    }
}
