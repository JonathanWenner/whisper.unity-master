using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : IState
{
    private StateManager stateManager;
    private Player attacker;

    private float stateTime = 5f;
    private float timer;

    private string answerdWord;

    public AttackState(StateManager stateManager)
    {
        this.stateManager = stateManager;
        attacker = this.stateManager.GetPlayerTurn();
    }

    public void Start()
    {
        stateManager.StartCoroutine(WaitForWord()); // start the state with a couroutine
        stateManager.setLastSayedWord(""); // reset last sayed word to nothing
    }

    public void Update()
    {

    }

    public void Exit()
    {
        stateManager.setLastSayedWord(answerdWord); 
    }


    private IEnumerator WaitForWord()
    {
        timer = stateTime;
        answerdWord = "";

        while (timer > 0)
        {
            timer -= Time.deltaTime;

            yield return null;

            //CHANGE WITH VOCE RECOGNITION AND SMOKE AND MIRRRORS CHECK
            if (Input.GetKeyDown(KeyCode.Space))
            {
                answerdWord = "Test";
                break;
            }
        }


        if (string.IsNullOrEmpty(answerdWord) || stateManager.Illegalwordlist.isInList(answerdWord)) // if a already used word is given or no word is given then damage player check for ending and go back to rock paper siscor
        {
            attacker.DamagePlayer();
            stateManager.CheckForWinner();
            stateManager.TransitionToState(stateManager.stateFactory.createState(typeof(RPSState)), true);
        }
        else // if a corrrect word is give
        {
            stateManager.setLastSayedWord(stateManager.Illegalwordlist.CleanWord(answerdWord)); // cleanword and change last set word into it
            stateManager.SwitchPlayerTurn();                                                    // switch the player turn
            stateManager.Illegalwordlist.addWord(answerdWord);                                  // add word to illegal word list
            stateManager.TransitionToNextState(stateManager);                                   // transtion to next phase of the game
        }

    }
}
