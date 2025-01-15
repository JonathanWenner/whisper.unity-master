using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class StateManager : MonoBehaviour
{
    [SerializeField] public UIhandler uiHandler;
    [SerializeField] public SmokeyMirror smokeyMirror;

    private IState currentState;
    private GameType currrentGameMode;
    public int currentStateIndex = 0;

    [SerializeField] public Player player1;
    [SerializeField] public Player player2;

    public Illegalwordlist Illegalwordlist = new Illegalwordlist();
    public StateFactory stateFactory;

    string lastSayedWord;
    public bool isPlayerOneTurn = true;

    float transitionTime = GameSettings.TransitionTime;

    public List<string> ICastList = new List<string> { "I cast", "I cost", "I caused", "I cross", "I crossed", "I call"};
    public bool gotWord = false;
    public string gottenWord = "";

    public float timer;

    private void Start()
    {
        stateFactory = new StateFactory(this);
        SetGameMode(GameType.MultiChane());
    }



    public void SetGameMode(GameType gameType)
    {
        currrentGameMode = gameType;
        currentStateIndex = 0;
        currentState = stateFactory.createState(gameType.StateSequence[currentStateIndex]);
        currentState.Start();
    }

    public void TransitionToNextState(bool reset = false, bool toFirst = false)
    {
        Debug.Log("started standerd state transition");
        if (reset)
        {
            currentStateIndex = 1;
            System.Type nextStateType = currrentGameMode.StateSequence[currentStateIndex];
            IState nextState = stateFactory.createState(nextStateType);
            StartCoroutine(TransitionToState(nextState));
        }
        else if (toFirst)
        {
            currentStateIndex = 0;
            System.Type nextStateType = currrentGameMode.StateSequence[currentStateIndex];
            IState nextState = stateFactory.createState(nextStateType);
            StartCoroutine(TransitionToState(nextState));
        }

        else if (currentStateIndex < currrentGameMode.StateSequence.Count - 1)
        {
            System.Type nextStateType = currrentGameMode.StateSequence[currentStateIndex+1];
            IState nextState = stateFactory.createState(nextStateType);
            Debug.Log("standerd transition to: " + nextState.ToString());
            currentStateIndex++;
            StartCoroutine(TransitionToState(nextState));
        }
        else
        {
            Debug.Log("end of standerd transition sequence reachde resetting to beginning");
            currentStateIndex = currrentGameMode.loopingPoint;
            System.Type nextStateType = currrentGameMode.StateSequence[currentStateIndex];
            IState nextState = stateFactory.createState(nextStateType);
            StartCoroutine(TransitionToState(nextState));
        }
    }


    public void SetState(IState newState)
    {
        // start the new state
        currentState = newState;
        currentState.Start();
        Debug.Log("start next state: " + newState.ToString());
    }

    public IEnumerator TransitionToState(IState newState, bool reset = false)
    {
        // leave the current state if it exists
        currentState?.Exit();
        Debug.Log("exit current state");

        yield return new WaitForSeconds(transitionTime);
        Debug.Log("waited for transition to next state");

        if (reset)
        {
            currentStateIndex = 0;
        }

        SetState(newState);
    }

    private void Update()
    {
        currentState?.Update();
        uiHandler.livesDrawLives(player1.GetLives(), player2.GetLives());
        uiHandler.showCurrentPhase((currentState.GetType()));
    }

    public void CheckForWinner()
    {
        if (player1.GetLives() <= 0)
        {
            Debug.Log("player 1 wins");
            //ADD GO TO WIN SCREEN P1
        }
        else if (player2.GetLives() <= 0)
        {
            Debug.Log("player 2 wins");
            //ADD GO TO WIN SCREEN P2
        }
    }

    public Player GetPlayerTurn()
    {
        if (isPlayerOneTurn)
        {
            return player1;
        }
        else
        {
            return player2;
        }
    }

    public Player GetNotPlayerTurn()
    {
        if (!isPlayerOneTurn)
        {
            return player1;
        }
        else
        {
            return player2;
        }
    }

    public void SetPlayerTurn(Player player)
    {
        if (player == player1)
        {
            isPlayerOneTurn = true;
        }
        else if (player == player2)
        {
            isPlayerOneTurn = false;
        }

        Actions.playerOneTurn(isPlayerOneTurn);

    }
    
    public void SwitchPlayerTurn()
    {
        isPlayerOneTurn = !isPlayerOneTurn;

        Actions.playerOneTurn(isPlayerOneTurn);
    }
    public string getLastSayedWord()
    {
        return lastSayedWord;
    }
    public void setLastSayedWord(string word)
    {
        lastSayedWord = word;
    }

    public string FilterAnswerSentance(string sentence)
    {
        foreach (var phrase in ICastList)
        {
            // Create a regex pattern to find the phrase and a word after it
            string pattern = $@"\b{Regex.Escape(phrase)}\b\s+(\w+)?";
            Match match = Regex.Match(sentence, pattern, RegexOptions.IgnoreCase);

            if (match.Success)
            {
                // If a word is found after the phrase, return it
                if (match.Groups[1].Success)
                {
                    return match.Groups[1].Value;
                }
                else
                {
                    // No word after the phrase
                    return "";
                }
            }
        }
        return "";
    }


    public void GetWord(string word)
    {
        gotWord = true;
        gottenWord = word;
    }
}



public class StateFactory
{
    private StateManager stateManager;

    public StateFactory(StateManager stateManager)
    {
        this.stateManager = stateManager;
    }

    public IState createState(Type stateType)
    {
        if (stateType == typeof(RPSState))
        {
            Debug.Log("creat rpsState in factory");
            return new RPSState(stateManager);
        }

        if (stateType == typeof(AttackState))
        {
            Debug.Log("creat AttackState in factory");
            return new AttackState(stateManager);
        }

        if (stateType == typeof(DefendState))
        {
            Debug.Log("creat AttackState in factory");
            return new DefendState(stateManager);
        }

        throw new Exception("incorrect state given");
    }



}

public class GameType
{
    public string Name { get; private set; }
    public List<System.Type> StateSequence;
    public int loopingPoint;

    public GameType(string Name, List<System.Type> StateSequence, int loopingPoint)
    {
        this.Name = Name;
        this.StateSequence = StateSequence;
        this.loopingPoint = loopingPoint;
    }


    public static GameType SingleChane()
    {
        return new GameType("SingleChane", new List<System.Type>
        {
            typeof(RPSState),
            typeof(AttackState),
            typeof(DefendState),
        }, 0);
    }

    public static GameType MultiChane()
    {
        return new GameType("MultiChane", new List<System.Type>
        {
            typeof(RPSState),
            typeof(AttackState),
            typeof(DefendState),
            typeof(DefendState),
        }, 2);
    }
}



