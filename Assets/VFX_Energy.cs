using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class VFX_Energy : MonoBehaviour
{
    [SerializeField] VisualEffect visualEffect;
    [SerializeField] Transform target;

    private void Update()
    {
        visualEffect.SetVector3("Target", target.position);
    }
}
