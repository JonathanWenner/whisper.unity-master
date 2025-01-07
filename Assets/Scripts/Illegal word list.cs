using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class Illegalwordlist
{
    HashSet<string> illegalwordlist = new HashSet<string>(); 

    // function to add words to the illegal words list
    public void addWord(string word)
    {
        string cleanedWord = CleanWord(word);
        
        if (!string.IsNullOrWhiteSpace(cleanedWord))
        {
            illegalwordlist.Add(cleanedWord);
            Debug.Log("Word added to illegal list: " + cleanedWord);
        }
        else
        {
            Debug.Log("empty word not added: " + cleanedWord);
        }
    }

    // function to check for illegal words in illegal word list
    public bool isInList(string word)
    {
        string cleanedWord = CleanWord(word);

        return illegalwordlist.Contains(cleanedWord);
    }

    // function to empty the illegal word list
    public void ClearList()
    {
        illegalwordlist.Clear();
        Debug.Log("list is empty");
    }

    public string CleanWord(string word)
    {
        string pattern = @"[^a-zA-Z]"; // Matches anything that's NOT a letter
        string cleanedWord = Regex.Replace(word, pattern, ""); // Replace non-letters with an empty string

        return cleanedWord.ToLower(); // Convert to lowercase for consistent comparison
    }
}
