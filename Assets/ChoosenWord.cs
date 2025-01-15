using EasyTextEffects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChoosenWord : MonoBehaviour
{
    private TextEffect textEffect;
    private void Awake()
    {
        textEffect = GetComponent<TextEffect>();
    }
    public void StartTextEffect()
    { 
        textEffect.StartManualEffects();
    }
}
