using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CameraShake : MonoBehaviour
{
    private Cinemachine.CinemachineVirtualCamera vcam;

    [SerializeField] private float shakeTime = 0.5f;
    [SerializeField] private float shakeAmp = 2;
    [SerializeField] private float shakeFreq = 3;

    private void Awake()
    {
        vcam = GetComponent<Cinemachine.CinemachineVirtualCamera>();
    }

    public void ShakeCamera()
    {
        Cinemachine.CinemachineBasicMultiChannelPerlin perlin = vcam.GetCinemachineComponent<Cinemachine.CinemachineBasicMultiChannelPerlin>();
        perlin.m_AmplitudeGain = shakeAmp;
        perlin.m_FrequencyGain = shakeFreq;
        StartCoroutine(StopShake(shakeTime));
    }

    private IEnumerator StopShake(float time)
    {
        yield return new WaitForSeconds(time);
        Cinemachine.CinemachineBasicMultiChannelPerlin perlin = vcam.GetCinemachineComponent<Cinemachine.CinemachineBasicMultiChannelPerlin>();
        perlin.m_AmplitudeGain = 0;
    }
}
