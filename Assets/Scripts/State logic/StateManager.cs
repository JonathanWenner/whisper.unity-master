using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class StateManager : MonoBehaviour
{
    private IState currentState;
    private GameType currrentGameMode;
    public int currentStateIndex = 0;
    
    public Player player1;
    public Player player2;

    public Illegalwordlist Illegalwordlist = new Illegalwordlist();
    public StateFactory stateFactory;

    string lastSayedWord;
    bool isPlayerOneTurn = true;

    [SerializeField] float transitionTime = 2f;


    private void Start()
    {
        stateFactory = new StateFactory(this);
    }

    public void SetGameMode(GameType gameType)
    {
        currrentGameMode = gameType;
        currentStateIndex = 0;
    }

    public void TransitionToNextState(object parameter)
    {
        if (currentStateIndex < currrentGameMode.StateSequence.Count)
        {
            System.Type nextStateType = currrentGameMode.StateSequence[currentStateIndex];
            IState nextState = stateFactory.createState(nextStateType);
            currentStateIndex++;
            TransitionToState(nextState);
            
        }
        else
        {
            currentStateIndex = currrentGameMode.loopingPoint;
            TransitionToNextState(parameter);
        }
    }


    public void SetState(IState newState)
    {
        // start the new state
        currentState = newState;
        currentState.Start();
    }

    public IEnumerable TransitionToState(IState newState, bool reset = false)
    {
        // leave the current state if it exists
        currentState?.Exit();

        yield return new WaitForSeconds(transitionTime);

        if (reset)
        {
            currentStateIndex = 1;
        }
        SetState(newState);
    }

    private void Update()
    {
        currentState?.Update();
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
    }
    
    public void SwitchPlayerTurn()
    {
        isPlayerOneTurn = !isPlayerOneTurn;
    }
    public string getLastSayedWord()
    {
        return lastSayedWord;
    }
    public void setLastSayedWord(string word)
    {
        lastSayedWord = word;
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
            return new RPSState(stateManager);
        }

        if (stateType == typeof(AttackState))
        {
            return new RPSState(stateManager);
        }

        if (stateType == typeof(DefendState))
        {
            return new RPSState(stateManager);
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



