using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightLerp : MonoBehaviour
{
    [Header("Light Settings")]
    public Light targetLight; // Reference to the light to control
    public float targetIntensity = 5f; // Desired intensity to lerp towards
    public float lerpSpeed = 2f; // Speed of the intensity change

    private float initialIntensity; // Store the initial intensity of the light

    void Start()
    {
        float randomStartTimer = Random.Range(0f, 1f); // Randomize the start time for each light
        
        StartCoroutine(RandomStart()); // Start the coroutine to randomize the start time

        if (targetLight == null)
        {
            targetLight = GetComponent<Light>(); // Automatically find the light if not assigned
        }

        // Store the initial intensity
        initialIntensity = targetLight.intensity;
    }

    private bool StartLightLerp = false;
    void Update()
    {
        if (StartLightLerp)
        {
            // PingPong between 0 and 1 over time and use it to lerp between the initial and target intensities
            float t = Mathf.PingPong(Time.time * lerpSpeed, 1f);
            targetLight.intensity = Mathf.Lerp(initialIntensity, targetIntensity, t);
        }

    }

    // Optional method to reset to the initial intensity
    public void ResetIntensity()
    {
        targetIntensity = initialIntensity;
    }

    private IEnumerator RandomStart()
    {
        yield return new WaitForSeconds(Random.Range(0f, 1.5f));
        StartLightLerp = true;
    }
}
