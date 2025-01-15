using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSettings : MonoBehaviour
{
    [SerializeField] public static float TransitionTime = 0.5f;
    [SerializeField] public static float RPSStateTime = 5f;
    [SerializeField] public static float AttackStateTime = 10f;
    [SerializeField] public static float DefendStateTime = 10f;

    [SerializeField] public static int RythmGameSetSize = 5;
    [SerializeField] public static float RythmBoostPerSet = 0.1f;

    private void Start()
    {
        
    }
}
