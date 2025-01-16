using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSettings : MonoBehaviour
{
    [SerializeField] public static float TransitionTime = 0.5f;
    [SerializeField] public static float RPSStateTime = 5f;
    [SerializeField] public static float AttackStateTime = 15;
    [SerializeField] public static float DefendStateTime = 15;

    [SerializeField] public static int RythmGameSetSize = 3;
    [SerializeField] public static float RythmBoostPerSet = 2f;

    private void Start()
    {
        
    }
}
