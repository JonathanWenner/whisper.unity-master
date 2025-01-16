using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class SmokeyMirror : MonoBehaviour
{
    [SerializeField] Button stopRecording;
    [SerializeField] Button validButton;
    [SerializeField] Button InValidButton;
    [SerializeField] public TMP_InputField InputField;
    public TextMeshProUGUI timer;
    public StateManager stateManager;

    public bool isValid = false;
    public bool gotInput = false;

    private bool answerGiven;


    // Start is called before the first frame update
    void Start()
    {
        validButton.onClick.AddListener(OnValidPPress);
        InValidButton.onClick.AddListener(OnInvalidPress);
        stopRecording.onClick.AddListener(OnStopRecordingPress);
        InputField.onEndEdit.AddListener(OnEnterAnswer);
    }
    private void Update()
    {
        timer.text = stateManager.timer.ToString();
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

    void OnEnterAnswer(string yes)
    {
        if (Input.GetKey(KeyCode.Return) || Input.GetKey(KeyCode.KeypadEnter))
        {
            OnValidPPress();
            Debug.Log("enter pressed");
        }
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
