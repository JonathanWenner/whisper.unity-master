using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class UIhandler : MonoBehaviour
{
    [SerializeField] GameObject defendUI;
    [SerializeField] GameObject attackUI;
    [SerializeField] GameObject RPSUI;

    [SerializeField] TMP_Text timerText;

    [SerializeField] TMP_Text Livesp1Text;
    [SerializeField] TMP_Text Livesp2Text;

    public void showCurrentPhase(System.Type stateType)
    {
        if (stateType == typeof (RPSState))
        {
            defendUI.SetActive(false);
            attackUI.SetActive(false);
            RPSUI.SetActive(true);
        }

        if (stateType == typeof(AttackState))
        {
            defendUI.SetActive(false);
            attackUI.SetActive(true);
            RPSUI.SetActive(false);
        }

        if (stateType == typeof(DefendState))
        {
            defendUI.SetActive(true);
            attackUI.SetActive(false);
            RPSUI.SetActive(false);
        }
    }

    public void drawTimer(float timerValue)
    {
        if (timerValue > 0)
        {
            timerText.text = ((int)timerValue).ToString();
        }
        else
        {
            timerValue = 0;
            timerText.text = 0.ToString();
        }
        
    }

    public void livesDrawLives(float p1Lives, float p2Lives)
    {
        Livesp1Text.text = p1Lives.ToString();
        Livesp2Text.text = p2Lives.ToString();
    }

}
