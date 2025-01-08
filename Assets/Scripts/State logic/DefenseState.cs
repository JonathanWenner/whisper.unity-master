using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEditor.SceneManagement;
using UnityEngine;

public class DefendState : IState
{
    private StateManager stateManager;
    private Player defender;

    private float stateTime = 5f;
    private float timer;

    private string answerdWord;

    public DefendState(StateManager stateManager)
    {
        this.stateManager = stateManager;
        defender = this.stateManager.GetPlayerTurn();
    }


    public void Start()
    {
        defender.SpeechRecognitionController.Click();
        stateManager.StartCoroutine(WaitForWord());
    }

    public void Update()
    {
        stateManager.uiHandler.drawTimer(timer);
        timer -= Time.deltaTime;
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

            yield return null;

            if (stateManager.smokeyMirror.HasAnswered()) // stop early if smoke and mirror says that the player has answered
            {
                break;
            }
        }

        defender.SpeechRecognitionController.Click();                                                           //turn of recording

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


        if (string.IsNullOrEmpty(answerdWord) || stateManager.Illegalwordlist.isInList(answerdWord) || !true)
        {
            Debug.Log("wrong word back to rps: " + answerdWord);
            defender.DamagePlayer();
            stateManager.CheckForWinner();
            stateManager.TransitionToNextState(true);
        }
        else
        {
            Debug.Log("correct word");
            stateManager.setLastSayedWord(stateManager.Illegalwordlist.CleanWord(answerdWord));
            stateManager.SwitchPlayerTurn();
            stateManager.Illegalwordlist.addWord(answerdWord);
            stateManager.TransitionToNextState();
        }
            
    }


    private bool ValidateWord(string word)
    {
        stateManager.Illegalwordlist.CleanWord(word);
        return word.StartsWith(stateManager.getLastSayedWord()[stateManager.getLastSayedWord().Length - 1].ToString());
    }
}
