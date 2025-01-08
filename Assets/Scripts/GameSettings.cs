using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSettings : MonoBehaviour
{
    [SerializeField] public static float TransitionTime = 0.5f;
    [SerializeField] public static float RPSStateTime = 5f;
    [SerializeField] public static float AttackStateTime = 5f;
    [SerializeField] public static float DefendStateTime = 5f;



    private void Start()
    {
        
    }
}
