using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AttackState : IState
{
    
    private StateManager stateManager;
    private Player attacker;

    private float stateTime = GameSettings.AttackStateTime;

    private string answerdWord;


    public AttackState(StateManager stateManager)
    {
        this.stateManager = stateManager;
        attacker = this.stateManager.GetPlayerTurn();
    }

    public void Start()
    {
        if (attacker == stateManager.player1)
        {
            Actions.playerOneAttack(true);
            Actions.playerTwoAttack(false);

        }
        else
        {
            Actions.playerTwoAttack(true);
            Actions.playerOneAttack(false);
        }

        attacker.SpeechRecognitionController.Click();
        stateManager.StartCoroutine(WaitForWord()); // start the state with a couroutine
        stateManager.setLastSayedWord(""); // reset last sayed word to nothing
    }

    public void Update()
    {
        stateManager.uiHandler.drawTimer(stateManager.timer);
        stateManager.timer -= Time.deltaTime;
    }

    public void Exit()
    {

        stateManager.setLastSayedWord(answerdWord); 
    }



    private IEnumerator WaitForWord()
    {
        stateManager.timer = stateTime;
        answerdWord = "";

        while (stateManager.timer > 0)
        {

            yield return null;

            if (stateManager.smokeyMirror.HasAnswered()) // stop early if smoke and mirror says that the player has answered
            {
                break;
            }
        }

        attacker.SpeechRecognitionController.Click();                                                           //turn of recording

        while (!stateManager.gotWord)
        {
            yield return null;
        }
        yield return new WaitForSeconds(0.5f);
        stateManager.gotWord = false;


        Debug.Log("found answer sentance: " + stateManager.gottenWord);
        answerdWord = stateManager.Illegalwordlist.CleanWord(stateManager.FilterAnswerSentance(stateManager.gottenWord)); //clean word
        stateManager.gottenWord = "";


        //smoke and mirror check
        Debug.Log("start smoke and mirror check");

        stateManager.smokeyMirror.gotInput = false;
        stateManager.smokeyMirror.InputField.text = answerdWord;
        while (!stateManager.smokeyMirror.gotInput)
        {
            yield return null;
        }

        if (stateManager.smokeyMirror.isValid)
            answerdWord = stateManager.smokeyMirror.InputField.text;
        else
            answerdWord = "";


        if (string.IsNullOrEmpty(answerdWord) || stateManager.Illegalwordlist.isInList(answerdWord)) // if a already used word is given or no word is given then damage player check for ending and go back to rock paper siscor
        {
            //unsuccesfull attack

            if (attacker == stateManager.player1)
            {
                Actions.playerOneAttackSuccessful?.Invoke();
            }
            else
            {
                Actions.playerTwoAttackSuccessful?.Invoke();
            }


            attacker.DamagePlayer();
            stateManager.CheckForWinner();
            stateManager.TransitionToNextState(true);
        }
        else // if a corrrect word is give
        {
            // succesfull attack

            if (attacker == stateManager.player1)
            {
                Actions.playerOneAttackSuccessful?.Invoke();
            }
            else
            {
                Actions.playerTwoAttackSuccessful?.Invoke();
            }

            stateManager.setLastSayedWord(stateManager.Illegalwordlist.CleanWord(answerdWord)); // cleanword and change last set word into it
            stateManager.SwitchPlayerTurn();                                                    // switch the player turn
            stateManager.Illegalwordlist.addWord(answerdWord);                                  // add word to illegal word list
            stateManager.TransitionToNextState();                                               // transtion to next phase of the game
        }

    }

}
