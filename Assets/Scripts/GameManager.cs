using NUnit.Framework.Constraints;
using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //GameObjects needed
    [SerializeField] Player player1;
    [SerializeField] Player player2;

    Illegalwordlist illegalwords = new Illegalwordlist();

    //Timers for how long each phase takes 
    [SerializeField] float rockPaperScissorPhaseTime = 4f;
    [SerializeField] float attackPhaseTime = 5f;
    [SerializeField] float defendPhaseTime = 5f;
    [SerializeField] float resetPhaseTime = 1f;
    [SerializeField] float transitionTime = 0.5f;
    private float phaseTimer;


    public enum GamePhase   // Gamephases while playing the game
    {
        RockPaperScissor,
        Attack,
        Defend,
        Transition
    }
    GamePhase currentGamePhase = GamePhase.RockPaperScissor;    // variable that keeps track of the current Gamephase


    
    GamePhase nextGamePhase;    // store phase that is planned after transition phase
    
    // function that starts the process of transitioning to another phase
    private void StartTransitionToPhase(GamePhase phase)
    {
        phaseTimer = transitionTime;
        nextGamePhase = phase;
        currentGamePhase = GamePhase.Transition;
    }

    // methode that transistions between phases that leaves time for animations
    // phase is set and activated by using its start methode
    private void TransitionUpdate()
    {
        if (phaseTimer < 0)
        {
            if (nextGamePhase == GamePhase.RockPaperScissor)
            {
                StartRockPaperScissor();
            }
            else if (nextGamePhase == GamePhase.Attack)
            {
                StartAttackPhase();
            }
            else if (nextGamePhase == GamePhase.Defend)
            {
                StartDefendPhase();
            }
            else
            {
                Debug.LogError("non valid next gamephase: " + nextGamePhase.ToString());
            }
        }

    }

    private Wand.WandGestures player1Gesture; //variable that stores chosen gesture by player one
    private Wand.WandGestures player2Gesture; //variable that stores chosen gesture by player two
    private Player winner;

    private void StartRockPaperScissor()
    {
        // reset values to standerd
        player1Gesture = Wand.WandGestures.nothing;
        player2Gesture = Wand.WandGestures.nothing;
        winner = null;

        // set timer to time length
        phaseTimer = rockPaperScissorPhaseTime;

        // start recording gestures with the wand
        player1.Wand.StartRecording();
        player2.Wand.StartRecording();

        // set gamephase to rock paper scissors activating rock paper scissor 
        currentGamePhase = GamePhase.RockPaperScissor;
        Debug.Log("Start Rock Paper Scissor");
    }

    private void RockPaperScissorPhaseUpdate()
    {
        //Check for the first gesture that is recorded for both players, than record and lock it in if it isn't nothing
        Wand.WandGestures foundGestureP1 = player1.Wand.GetDetectedGesture();
        Wand.WandGestures foundGestureP2 = player2.Wand.GetDetectedGesture();
        
        if (player1Gesture == Wand.WandGestures.nothing 
            && foundGestureP1 != Wand.WandGestures.nothing)
        {
            player1Gesture = foundGestureP1;
            Debug.Log("player one gesture recorded: " + player1Gesture);
        }
        
        if (player2Gesture == Wand.WandGestures.nothing
            && foundGestureP2 != Wand.WandGestures.nothing)
        {
            player2Gesture = foundGestureP2;
            Debug.Log("player two gesture recorded: " + player2Gesture);
        }

        // if both gestures are recorded (A gesture is done) check outcome rock paper scissors if draw do rock paper scissors again else go to attack phase with the recorded winner
        if (player1Gesture != Wand.WandGestures.nothing 
            && player2Gesture != Wand.WandGestures.nothing)
        {
            if (player1Gesture == player2Gesture)
            {
                StartTransitionToPhase(GamePhase.RockPaperScissor);
                Debug.Log("rock paper scissors is a draw");
            }
            else if ((player1Gesture == Wand.WandGestures.Scissor && player2Gesture == Wand.WandGestures.Paper) ||
                     (player1Gesture == Wand.WandGestures.Rock && player2Gesture == Wand.WandGestures.Scissor) ||
                     (player1Gesture == Wand.WandGestures.Paper && player2Gesture == Wand.WandGestures.Rock))
            {
                winner = player1;
                StartTransitionToPhase(GamePhase.Attack);
                Debug.Log("Player one wins rock paper scissors");
            }
            else if ((player1Gesture == Wand.WandGestures.Scissor && player2Gesture == Wand.WandGestures.Rock) ||
                     (player1Gesture == Wand.WandGestures.Rock && player2Gesture == Wand.WandGestures.Paper) ||
                     (player1Gesture == Wand.WandGestures.Paper && player2Gesture == Wand.WandGestures.Scissor))
            {
                winner = player2;
                StartTransitionToPhase(GamePhase.Attack);
                Debug.Log("Player two wins rock paper scissors");
            }
        }

        // if time to do gesture runs out check if at least one player did a gesture, if so they win automaticly and go to attack phase if both players didn't do it redo rock paper scissors
        if (phaseTimer < 0)
        {
            if (player1Gesture != Wand.WandGestures.nothing && player2Gesture == Wand.WandGestures.nothing)
            {
                winner = player1;
                StartTransitionToPhase(GamePhase.Attack);
                Debug.Log("Player one wins rock paper scissors automaticly");
            }
            else if (player1Gesture == Wand.WandGestures.nothing && player2Gesture != Wand.WandGestures.nothing)
            {
                winner = player2;
                StartTransitionToPhase(GamePhase.Attack);
                Debug.Log("Player two wins rock paper scissors automaticly");
            }
            else
            {
                StartTransitionToPhase(GamePhase.RockPaperScissor);
                Debug.Log("No Gesture was entered");
            }
        }
    }


    private void StartAttackPhase()
    {

        currentGamePhase = GamePhase.Attack;
    }

    private void AttackPhaseUpdate()
    {
        
        if (phaseTimer < 0)
        {

        }
    }

    private string findAttackWord()
    {

        return " ";
    }


    private void StartDefendPhase()
    {

    }

    private void DefendPhaseUpdate()
    {

    }





    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        phaseTimer -= Time.deltaTime;

        if (currentGamePhase == GamePhase.Transition)
        {
            TransitionUpdate();
        }
    }
}
