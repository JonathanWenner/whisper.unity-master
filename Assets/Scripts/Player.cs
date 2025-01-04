using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Wand;

public class Player : MonoBehaviour
{
    [SerializeField] int lives = 3;
    [SerializeField] int playerNumber = 1;

    public Wand Wand;
    public VoiceRecognition VoiceRecognition;

    public void DamagePlayer(int damage = 1)
    {
        lives--;
    }
    public int GetPlayerNumber()
    {
        return playerNumber;
    }
}
