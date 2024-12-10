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
        if (targetLight == null)
        {
            targetLight = GetComponent<Light>(); // Automatically find the light if not assigned
        }

        // Store the initial intensity
        initialIntensity = targetLight.intensity;
    }

    void Update()
    {
        // PingPong between 0 and 1 over time and use it to lerp between the initial and target intensities
        float t = Mathf.PingPong(Time.time * lerpSpeed, 1f);
        targetLight.intensity = Mathf.Lerp(initialIntensity, targetIntensity, t);
    }

    // Optional method to reset to the initial intensity
    public void ResetIntensity()
    {
        targetIntensity = initialIntensity;
    }
}
