using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Whisper.Utils;
using Whisper;
using static UnityEngine.Networking.UnityWebRequest;
using UnityEngine.UIElements;
using UnityEngine.UI;
using Newtonsoft.Json.Bson;

public class VoiceRecognition : MonoBehaviour
{
    /// <summary>
    /// Stream transcription from microphone input.
    /// </summary>
    public class StreamingSampleMic : MonoBehaviour
    {
        public WhisperManager whisper;
        public MicrophoneRecord microphoneRecord;

        [Header("UI")]
        public Text text;
        
        private WhisperStream _stream;
        private string _result;

        private async void Start()
        {
            _stream = await whisper.CreateStream(microphoneRecord);
            _stream.OnResultUpdated += OnResult;
            _stream.OnSegmentUpdated += OnSegmentUpdated;
            _stream.OnSegmentFinished += OnSegmentFinished;
            _stream.OnStreamFinished += OnFinished;

            microphoneRecord.OnRecordStop += OnRecordStop;
        }

        // function that starts recording trough specified mic
        public void StartRecording()
        {
            if (!microphoneRecord.IsRecording)
            {
                _stream.StartStream();
                microphoneRecord.StartRecord();
            }
        }

        // function that stops recording
        public void StopRecording()
        {
            if (microphoneRecord.IsRecording)
            {
                microphoneRecord.StopRecord();
            }
        }

        public string GetCurrentResult(string result)
        {
            return _result;
        }


        private void OnResult(string result)
        {
            _result = result;
            text.text = result;
        }

        private void OnRecordStop(AudioChunk recordedAudio)
        {
            
        }



        private void OnSegmentUpdated(WhisperResult segment)
        {
            print($"Segment updated: {segment.Result}");
        }

        private void OnSegmentFinished(WhisperResult segment)
        {
            print($"Segment finished: {segment.Result}");
        }

        private void OnFinished(string finalResult)
        {
            print("Stream finished!");
        }
    }
}
