using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public class Wand : MonoBehaviour
{
    // gestures that can be detected
    public enum WandGestures
    {
        Rock,
        Paper,
        Scissor,
        nothing
    }

    // detected gestures
    WandGestures detectedGesture = WandGestures.nothing;
    bool isRecording = false;

    private void Update()
    {

    }

    
    public void StartRecording(int playerNumber, float phaseTime) // function that starts recording the latest detected gesture
    {
        isRecording = true;

        float time = Mathf.RoundToInt(phaseTime * 1000);

        if (playerNumber == 1) TCPServer.instance.SendMessageToWand1("start " + time);
        else TCPServer.instance.SendMessageToWand2("start " + time);
    }
    public void StopRecording() // function that stops recording the latest 
    {
        isRecording = false;
    }
    public bool IsRecording() // function that returns true if gestures are being recorded
    {
        return isRecording;
    }
    public void StopRecordingAndResetGesture() // function that stops recording gestures and resets the last dected gesture to nothing
    {
        ResetRecordedGesture();
        isRecording = false;
    }
    public void ResetRecordedGesture() // function that sets the last detected gesture to nothing
    {
        detectedGesture = WandGestures.nothing;
    }
    public WandGestures GetDetectedGesture()
    {
        return detectedGesture;
    }


    // Process recieved commands
    public void callback(string msg) {

        if (isRecording) {

            switch (msg) {
                case "Right": 
                    detectedGesture = WandGestures.Rock;
                    break;
                case "Down": 
                    detectedGesture = WandGestures.Paper;
                    break;
                case "Left":
                    detectedGesture = WandGestures.Scissor;
                    break;

            }
        }
    }
}
