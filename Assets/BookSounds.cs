using EasyTextEffects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookSounds : MonoBehaviour
{
    [SerializeField] TextEffect TextEffect;
    public void FlipPage()
    {
        AudioManager.Instance.PlaySound("FlipPage", 1f, true);
    }

    public void CloseBook()
    {
        AudioManager.Instance.PlaySound("CloseBook", 1f, true);
    }

    public void OpenBook()
    {
        AudioManager.Instance.PlaySound("OpenBook", 1f, true);
    }

    public void StartTextEffects()
    {
        TextEffect.StartManualEffects();
    }
}
