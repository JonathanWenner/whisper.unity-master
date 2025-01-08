using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SmokeyMirror : MonoBehaviour
{
    [SerializeField] Button stopRecording;
    [SerializeField] Button validButton;
    [SerializeField] Button InValidButton;
    [SerializeField] public TMP_InputField InputField;

    public bool isValid = false;
    public bool gotInput = false;

    private bool answerGiven;


    // Start is called before the first frame update
    void Start()
    {
        validButton.onClick.AddListener(OnValidPPress);
        InValidButton.onClick.AddListener(OnInvalidPress);
        stopRecording.onClick.AddListener(OnStopRecordingPress);
    }

    private void OnStopRecordingPress()
    {
        answerGiven = true;
    }

    private void OnValidPPress()
    {
        isValid = true;
        gotInput = true;
    }
    private void OnInvalidPress()
    {
        isValid = false;
        gotInput = true;
    }

    public bool HasAnswered()
    {
        if (answerGiven)
        {
            answerGiven= false;
            return true;
        }
        return false;

    }




    public string GetInputText()
    {
        return InputField.text;
    }

    public bool IsValid()
    {
        return isValid;
    }
}
