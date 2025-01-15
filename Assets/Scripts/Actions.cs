using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor.PackageManager;

public static class Actions
{

    // Gesture phase

    public static Action<bool> isGesturePhase;


    // Determine Player turn
    public static Action<bool> playerOneTurn;

    //defend
    public static Action StartDefend;
    public static Action<bool> DefendOutcome;
    public static Action EndDefend;

    //attack
    public static Action StartAttack;
    public static Action<bool> AttackOutcome;
    public static Action EndAttack;

    // player lose life
    public static Action PlayerLoseLife;


}
