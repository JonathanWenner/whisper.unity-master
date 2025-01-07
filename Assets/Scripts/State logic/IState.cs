using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IState
{
    void Start();
    void Update();
    void Exit();
}
