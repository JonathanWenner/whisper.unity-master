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
        Actions.StartAttack?.Invoke();

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



    }



    private IEnumerator WaitForWord()
    {
        stateManager.timer = stateTime;
        answerdWord = "";

        //while (stateManager.timer > 0)
        //{

        //    yield return null;

        //    if (stateManager.smokeyMirror.HasAnswered()) // stop early if smoke and mirror says that the player has answered
        //    {
        //        break;
        //    }
        //}

/*        attacker.SpeechRecognitionController.Click();         */                                                  //turn of recording

        //while (!stateManager.gotWord)
        //{
        //    yield return null;
        //}
        //yield return new WaitForSeconds(0.5f);
        //stateManager.gotWord = false;


        Debug.Log("found answer sentance: " + stateManager.gottenWord);
        //answerdWord = stateManager.Illegalwordlist.CleanWord(stateManager.FilterAnswerSentance(stateManager.gottenWord)); //clean word
        stateManager.gottenWord = "";


        //smoke and mirror check
        Debug.Log("start smoke and mirror check");

        stateManager.smokeyMirror.gotInput = false;
        //stateManager.smokeyMirror.InputField.text = answerdWord;
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

            stateManager.setLastSayedWord(answerdWord);

            Debug.Log("last sayed word: " + stateManager.getLastSayedWord());

            Actions.GetLastSaidWord?.Invoke(stateManager.getLastSayedWord());

            Actions.AttackOutcome(false);

            Actions.EndAttack?.Invoke();


            attacker.DamagePlayer();
            stateManager.SwitchPlayerTurn();                                                    // switch the player turn
            stateManager.CheckForWinner();
            stateManager.TransitionToNextState(true);

            Actions.ResetBackToAttack?.Invoke(true);
            Actions.ResetToAttack?.Invoke();

        }
        else // if a corrrect word is give
        {
            // succesfull attack

            stateManager.setLastSayedWord(answerdWord);

            Debug.Log("last sayed word: " + stateManager.getLastSayedWord());

            Actions.GetLastSaidWord?.Invoke(stateManager.getLastSayedWord());

            Actions.AttackOutcome(true);

            Actions.EndAttack?.Invoke();

            stateManager.setLastSayedWord(stateManager.Illegalwordlist.CleanWord(answerdWord)); // cleanword and change last set word into it
            stateManager.SwitchPlayerTurn();                                                    // switch the player turn
            stateManager.Illegalwordlist.addWord(answerdWord);                                  // add word to illegal word list
            stateManager.TransitionToNextState();                                               // transtion to next phase of the game
        }

    }

}
