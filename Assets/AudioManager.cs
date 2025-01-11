using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    
    public static AudioManager Instance;

    [System.Serializable]
    public struct Sound
    {
        public string soundName;
        public AudioClip audioClip;
    }

    public List<Sound> sounds = new List<Sound>();

    private void Awake()
    {
        // Ensure there's only one instance of AudioManager
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    public void PlaySound(string name, float volume, bool enableRandomPitch)
    {
        // Find the sound by name
        Sound? sound = sounds.Find(s => s.soundName == name);
        if (sound.HasValue)
        {
            StartCoroutine(PlayAndDestroy(sound.Value.audioClip,volume,enableRandomPitch));
        }
        else
        {
            Debug.LogWarning($"Sound '{name}' not found!");
        }
    }

    private IEnumerator PlayAndDestroy(AudioClip clip, float volume, bool enableRandomPitch)
    {
        // Create a temporary AudioSource
        GameObject tempAudioSource = new GameObject("TempAudioSource");
        AudioSource audioSource = tempAudioSource.AddComponent<AudioSource>();
        audioSource.clip = clip;

        // Randomize pitch if enabled
        if (enableRandomPitch)
        {
            float randompitch = Random.Range(0.9f, 1.1f);
            audioSource.pitch = randompitch;
        }

        audioSource.volume = volume;

        audioSource.Play();

        // Wait for the clip to finish playing
        yield return new WaitForSeconds(clip.length);

        // Destroy the temporary AudioSource
        Destroy(tempAudioSource);
    }
}

