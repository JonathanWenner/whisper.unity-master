using System;
using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class RythmGame : MonoBehaviour
{
    RawImage Up;
    RawImage Right;
    RawImage Left;
    int succesfullChaneCount = 0;

    
    int setSize = GameSettings.RythmGameSetSize;

    public enum PointArrows
    {
        Up = 0, Down = 1, Left = 2
    }

    private int currenPointer = 0;
    PointArrows[] RandomPointSet;
    bool[] CheckMarkSet;


    public RythmGame()
    {
        RandomPointSet = new PointArrows[setSize];
        CheckMarkSet = new bool[setSize];
        ResetArrowSet();
    }

    private void ResetArrowSet()
    {
        System.Random rand = new System.Random();

        for (int i = 0; i < setSize; i++)
        {
            RandomPointSet[i] = (PointArrows)rand.Next(0, 2);
        }
        for (int i = 0; i < setSize; i++)
        {
            CheckMarkSet[i] = false;
        }
        currenPointer = 0;

    }

    private void GoToNextArrow()
    {
        CheckMarkSet[currenPointer] = true;
        currenPointer++;
    }


    public void checkForDetectedArrow(PointArrows detectedPoint)
    {
        if (detectedPoint == RandomPointSet[currenPointer])
        {
            if (currenPointer >= setSize - 1)
            {
                ResetArrowSet();
                succesfullChaneCount++;
            }
            else
            {
                GoToNextArrow();
            }
        }
    }

    public void SpawnRandomPointSet()
    {

    }



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
