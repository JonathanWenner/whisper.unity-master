using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor.PackageManager;

public static class Actions
{

    // Gesture phase

    public static Action<bool> isGesturePhase;


    // Player one 
    public static Action<bool> playerOneTurn;

        // Attack
        public static Action<bool> playerOneAttack;
        public static Action<bool> playerOneAttackSuccessful;

        // Defend
        public static Action<bool> playerOneDefend;
        public static Action<bool> playerOneDefendSuccessful;

    //________________________________________________________

    // Player two
    public static Action<bool> playerTwoTurn;

        // Attack
        public static Action<bool> playerTwoAttack;
        public static Action<bool> playerTwoAttackSuccessful;

        // Defend
        public static Action<bool> playerTwoDefend;
        public static Action<bool> playerTwoDefendSuccessful;


}
