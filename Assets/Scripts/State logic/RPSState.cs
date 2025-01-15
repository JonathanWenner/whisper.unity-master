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

    private GameObject preFabPaperP1;
    private GameObject preFabRockP1;
    private GameObject preFabSiscorP1;
    private GameObject[] preFabListP1;
    private int currentIndexSwitcher;

    private GameObject preFabPaperP2;
    private GameObject preFabRockP2;
    private GameObject preFabSiscorP2;
    private GameObject[] preFabListP2;
    private float switchTime = 0.5f;
    private float switchTimer = 0.5f;

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

        preFabPaperP1 = player1.Wand.prefabPaper;
        preFabSiscorP1 = player1.Wand.prefabSiscor;
        preFabRockP1 = player1.Wand.prefabRock;

        preFabPaperP2 = player2.Wand.prefabPaper;
        preFabSiscorP2 = player2.Wand.prefabSiscor;
        preFabRockP2 = player2.Wand.prefabRock;

        preFabListP1 = new GameObject[] { preFabRockP1, preFabPaperP1, preFabSiscorP1 };
        preFabListP2 = new GameObject[] { preFabRockP2, preFabPaperP2, preFabSiscorP2 };

        for (int i = 0; i < preFabListP1.Length; i++)
        {
            preFabListP1[i].SetActive(i == currentIndexSwitcher);
            preFabListP2[i].SetActive(i == currentIndexSwitcher);
        }


        player1Gesture = Wand.WandGestures.nothing;
        player2Gesture = Wand.WandGestures.nothing;

        //ACTIVATE WITH WANTS!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        //player1.Wand.StartRecording(player1.GetPlayerNumber(), StateTime);
        //player2.Wand.StartRecording(player2.GetPlayerNumber(), StateTime);

        stateManager.StartCoroutine(WaitForChoices());
    }

    public void Update()
    {
        stateManager.timer -= Time.deltaTime;
        switchTimer -= Time.deltaTime;
        stateManager.uiHandler.drawTimer(stateManager.timer);
    }

    public void Exit()
    {
        for (int i = 0;i < preFabListP1.Length;i++)
        {
            preFabListP1[i].SetActive(false);
            preFabListP2[i].SetActive(false);
        }
    }

    private IEnumerator WaitForChoices()
    {
        stateManager.timer = StateTime;
        
        while (stateManager.timer > 0)
        {
            yield return null;

            if (switchTimer < 0)
            {
                if (player1Gesture == Wand.WandGestures.nothing)
                    preFabListP1[currentIndexSwitcher].SetActive(false);

                if (player2Gesture == Wand.WandGestures.nothing)
                    preFabListP2[currentIndexSwitcher].SetActive(false);

                currentIndexSwitcher = (currentIndexSwitcher + 1) % preFabListP1.Length;

                if (player1Gesture == Wand.WandGestures.nothing)
                    preFabListP1[currentIndexSwitcher].SetActive(true);

                if (player2Gesture == Wand.WandGestures.nothing)
                    preFabListP2[currentIndexSwitcher].SetActive(true);

                switchTimer = switchTime;
            }

            if (player1Gesture != Wand.WandGestures.nothing)
            {
                if (player1Gesture == Wand.WandGestures.Rock)
                {
                    preFabListP1[0].SetActive(true);
                    preFabListP1[1].SetActive(false);
                    preFabListP1[2].SetActive(false);
                }
                else if (player1Gesture == Wand.WandGestures.Paper)
                {
                    preFabListP1[0].SetActive(false);
                    preFabListP1[1].SetActive(true);
                    preFabListP1[2].SetActive(false);
                }
                else if (player1Gesture == Wand.WandGestures.Scissor)
                {
                    preFabListP1[0].SetActive(false);
                    preFabListP1[1].SetActive(false);
                    preFabListP1[2].SetActive(true);
                }
            }
            if (player2Gesture != Wand.WandGestures.nothing)
            {
                if (player2Gesture == Wand.WandGestures.Rock)
                {
                    preFabListP2[0].SetActive(true);
                    preFabListP2[1].SetActive(false);
                    preFabListP2[2].SetActive(false);
                }
                else if (player2Gesture == Wand.WandGestures.Paper)
                {
                    preFabListP2[0].SetActive(false);
                    preFabListP2[1].SetActive(true);
                    preFabListP2[2].SetActive(false);
                }
                else if (player2Gesture == Wand.WandGestures.Scissor)
                {
                    preFabListP2[0].SetActive(false);
                    preFabListP2[1].SetActive(false);
                    preFabListP2[2].SetActive(true);
                }
            }


            //CHANGE BACK!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            if (Input.GetKeyDown(KeyCode.S))
                player2Gesture = Wand.WandGestures.Paper;


            // player1Gesture = player1.Wand.GetDetectedGesture();
            // player2Gesture = player2.Wand.GetDetectedGesture();


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
