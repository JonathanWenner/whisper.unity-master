using System;
using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class RythmGame : MonoBehaviour
{
    public Player player;

    // images for rythm game
    public RawImage DownImagePrefab;
    public RawImage RightImagePrefab;
    public RawImage LeftImagePrefab;
    public Transform NodeContainer;

    int succesfullChaneCount = 0;

    // arrays for images, representation of set, bools for checking if done
    RawImage[] NodeSetImages;
    PointArrows[] RandomNodeSet;
    bool[] CheckMarkSet;

    // amount of gestures per set
    int setSize = GameSettings.RythmGameSetSize;

    public enum PointArrows
    {
        Right = 0, Down = 1, Left = 2, nothing
    }

    private int currenPointer = 0;



    public RythmGame()
    {
        NodeSetImages = new RawImage[setSize];
        RandomNodeSet = new PointArrows[setSize];
        CheckMarkSet = new bool[setSize]; 
    }


    public void StartRythm(float phaseTime)
    {
        ResetArrowSet();
        succesfullChaneCount = 0;
        player.Wand.StartRecording(player.GetPlayerNumber(), phaseTime);
    }
    public void StopRythm()
    {
        ClearPreviousNodes();
        player.Wand.StopRecording();
    }
    public float GetAttackerBoost()
    {
        float returnvalue = 1;

        returnvalue += succesfullChaneCount * GameSettings.RythmBoostPerSet;

        return returnvalue;
    }

    private void Start()
    {
        ResetArrowSet();
    }

    public void Update()
    {
        PointArrows detectedArrow = PointArrows.nothing;
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            detectedArrow = PointArrows.Left;
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            detectedArrow = PointArrows.Down;
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            detectedArrow = PointArrows.Right;
        }


        if (detectedArrow != PointArrows.nothing)
        {
            checkForDetectedArrow(detectedArrow);

        }
    }


    private void ResetArrowSet()
    {

        System.Random rand = new System.Random();

        // fill checkmark and gesture array
        for (int i = 0; i < setSize; i++)
        {
            RandomNodeSet[i] = (PointArrows)rand.Next(0, 3);
            CheckMarkSet[i] = false;
        }

        currenPointer = 0;

        // do starting animation and spawn pictures
        CreateRandomPointSetAnimation();
        
    }

    private void GoToNextArrow()
    {
        CheckMarkSet[currenPointer] = true;
        FinishCurrentPointAnimation(currenPointer);
        currenPointer++;
    }


    public void checkForDetectedArrow(PointArrows detectedPoint)
    {
        if (detectedPoint == RandomNodeSet[currenPointer])
        {
            if (currenPointer >= setSize - 1)
            {
                succesfullChaneCount++;
                ResetArrowSet();
            }
            else
            {
                GoToNextArrow();
            }
        }
    }

    public void CreateRandomPointSetAnimation()
    {
        // clear completed nodes if they exists
        ClearPreviousNodes();

        // create spacing between nodes
        float spacing = 100f;
        float startX = -((setSize - 1) * spacing) / 2f;
        Vector3 startPosition = new Vector3(startX, 0, 0);

        //spawn in nodes according to with is in the random node set
        for (int i = 0; i < setSize; i++)
        {
            RawImage nodeImage = null;

            if (RandomNodeSet[i] == PointArrows.Down)
            {
                nodeImage = Instantiate(DownImagePrefab, NodeContainer);
            }
            else if (RandomNodeSet[i] == PointArrows.Left)
            {
                nodeImage = Instantiate(LeftImagePrefab, NodeContainer);
            }
            else if (RandomNodeSet[i] == PointArrows.Right)
            {
                nodeImage = Instantiate(RightImagePrefab, NodeContainer);
            }

            if (nodeImage != null)
            {
                // place the nodes in the correct positions
                NodeSetImages[i] = nodeImage;
                nodeImage.rectTransform.anchoredPosition = startPosition + new Vector3(i * spacing, 0, 0);

                // create a fade in effect
                nodeImage.color = new Color(nodeImage.color.r, nodeImage.color.g, nodeImage.color.b, 0); 
                StartCoroutine(FadeIn(nodeImage, 0.3f, i * 0.1f)); 
            }
        }
    }

    // Fade in effect for spawining nodes
    private IEnumerator FadeIn(RawImage image, float duration, float delay)
    {
        yield return new WaitForSeconds(delay); // Delay before starting fade-in

        float elapsedTime = 0f;
        Color originalColor = image.color;

        while (elapsedTime < duration)
        {
            if (image == null)
                yield break;

            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsedTime / duration);
            image.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        if (image != null)
        {
            image.color = new Color(originalColor.r, originalColor.g, originalColor.b, 1); // Ensure final alpha is 1
        }
    }

    private void ClearPreviousNodes()
    {
        foreach (Transform child in NodeContainer)
        {
            Destroy(child.gameObject);
        }
    }

    public void FinishCurrentPointAnimation(int currentPoint)
    {
        if (NodeSetImages[currentPoint] != null)
        {
            StartCoroutine(FadeOut(NodeSetImages[currentPoint]));
        }
    }


    private IEnumerator FadeOut(RawImage image)
    {
        if (image == null)
        {
            yield break;
        }


        float fadeDuration = 0.5f;

        Color originalColor = image.color;

        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            if (image == null)
                yield break;

            float normalizedTime = t / fadeDuration;
            image.color = new Color(originalColor.r, originalColor.g, originalColor.b, 1 - normalizedTime);
            yield return null;
        }

        if (image != null)
        {
            image.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0);
            Destroy(image.gameObject);
        }
    }
}
