using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TextDisappearing : MonoBehaviour
{
    public UnityEvent OnDisappear;
    public void Disappear()
    {
        OnDisappear?.Invoke();
    }
}
