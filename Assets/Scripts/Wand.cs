using System.Collections;
using System.Collections.Generic;
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
        if (isRecording)
        {
            DetectCurrentGesture();
        }
    }

    
    public void StartRecording() // function that starts recording the latest detected gesture
    {
        isRecording = true;
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



    // function that updates the latest detected gesture
    private void DetectCurrentGesture()
    {
        if (DetectedRock())
        {
            detectedGesture = WandGestures.Rock;
        }
        else if (DetectedPaper())
        {
            detectedGesture = WandGestures.Paper;
        }
        else if (DetectedScissor())
        {
            detectedGesture = WandGestures.Scissor;
        }
    }







    //TODO Add actual wand functionality
    private bool DetectedRock()
    {
        return Input.GetKey(KeyCode.R);
    }
    private bool DetectedPaper()
    {
        return Input.GetKey(KeyCode.P);
    }
    private bool DetectedScissor()
    {
        return Input.GetKey(KeyCode.S);
    }




}
