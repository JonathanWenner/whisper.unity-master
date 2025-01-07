using System.Collections;
using System.Collections.Generic;
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
        stateManager.StartCoroutine(WaitForWord());
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

        if (string.IsNullOrEmpty(answerdWord) || stateManager.Illegalwordlist.isInList(answerdWord) || !ValidateWord(answerdWord))
        {
            defender.DamagePlayer();
            stateManager.CheckForWinner();
            stateManager.TransitionToState(stateManager.stateFactory.createState(typeof (RPSState)), true);
        }
        else
        {
            stateManager.setLastSayedWord(stateManager.Illegalwordlist.CleanWord(answerdWord));
            stateManager.SwitchPlayerTurn();
            stateManager.Illegalwordlist.addWord(answerdWord);
            stateManager.TransitionToNextState(stateManager);
        }
            
    }



    private bool ValidateWord(string word)
    {
        return word.StartsWith(stateManager.getLastSayedWord()[stateManager.getLastSayedWord().Length - 1].ToString());
    }
}
