using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Whisper.Samples;
using UnityEngine.Events;
using System.Text.RegularExpressions;

public class SpeechReadHandler : MonoBehaviour
{
    [SerializeField] public Transform Player1;
    [SerializeField] public Transform Player2;

    [SerializeField] public Rigidbody FireBall;




    public UnityEvent OnCorrectAnswer;
    public UnityEvent OnIncorrectAnswer;



    public enum PlayerTurn
    {
        Player1,
        Player2
    }

    private PlayerTurn _currentPlayerTurn;

    private void Start()
    {
        _currentPlayerTurn = PlayerTurn.Player1;
    }

    public void SwitchPlayerTurn()
    {
        _currentPlayerTurn = _currentPlayerTurn == PlayerTurn.Player1 ? PlayerTurn.Player2 : PlayerTurn.Player1;
    }

    public string currentWord;
    public string previousWord;
    public void GetWord(string word)
    {
        string cleanedWord = CleanWord(word);

        if (!string.IsNullOrWhiteSpace(cleanedWord))
        {
            Debug.Log("Word: " + cleanedWord);

            UpdatePreviousWord(cleanedWord);
            CheckWordMatch();
        }
        else
        {
            Debug.LogWarning("Received empty or invalid word!");
        }
    }

    public float maxVelocity = 1f;
    private void MoveFireBallToTargetedPlayer()
    {
        Vector3 targetPosition = _currentPlayerTurn == PlayerTurn.Player1 ? Player1.position : Player2.position;

        // Smoothly move the fireball towards the target player
        FireBall.velocity = (targetPosition - FireBall.position).normalized * maxVelocity;

        // Clamp the fireball's velocity to the maximum allowed speed
        FireBall.velocity = Vector3.ClampMagnitude(FireBall.velocity, maxVelocity);

        // Optional: Stop the fireball if it's very close to the target
        if (Vector3.Distance(FireBall.position, targetPosition) < 0.1f)
        {
            FireBall.velocity = Vector3.zero; // Stop the fireball
        }
    }

    private void FixedUpdate()
    {
        if (StartsWithLastLetter())
        {
            MoveFireBallToTargetedPlayer();
        }
    }



    private string CleanWord(string word)
    {
        // Regex pattern to remove all non-alphabetic characters (e.g., ".", ",", "!")
        string pattern = @"[^a-zA-Z]"; // Matches anything that's NOT a letter
        string cleanedWord = Regex.Replace(word, pattern, ""); // Replace non-letters with an empty string

        return cleanedWord.ToLower(); // Convert to lowercase for consistent comparison
    }

    private void UpdatePreviousWord(string word)
    {
        if (string.IsNullOrEmpty(currentWord))
        {
            currentWord = word;  // Set the first word as current
        }
        else
        {
            previousWord = currentWord;  // Move current to previous word only after first update
            currentWord = word;  // Set new word as current
        }
    }

    private void CheckWordMatch()
    {
        if (StartsWithLastLetter())
        {
            OnCorrectAnswer.Invoke();  // Trigger the correct answer event
            SwitchPlayerTurn();        // Switch turn only on correct answer
        }
        else
        {
            OnIncorrectAnswer.Invoke();  // Trigger the incorrect answer event
                                         // Optionally, you could add a penalty or other logic here
        }
    }

    private bool StartsWithLastLetter()
    {
        if (string.IsNullOrEmpty(previousWord)) return false;

        Debug.Log("Previous word ended with " + char.ToLower(previousWord[previousWord.Length - 1]));
        Debug.Log("Current word starts with " + char.ToLower(currentWord[0]));

        // Compare last letter of previousWord with the first letter of currentWord
        return char.ToLower(previousWord[previousWord.Length - 1]) == char.ToLower(currentWord[0]);
    }

}
