

using System.Collections;
using UnityEngine;

public class RythmGame : MonoBehaviour
{
    public Player player;

    // Prefabs for the rhythm game
    public GameObject DownSpritePrefab;
    public GameObject RightSpritePrefab;
    public GameObject LeftSpritePrefab;
    public Transform NodeContainer;

    public AudioSource audioSource;
    public AudioClip hitSound;
    float minPitch = 0.7f;
    float maxPitch = 1.3f;

    int succesfullChaneCount = 0;

    // Arrays for images, representation of set, bools for checking if done
    GameObject[] NodeSetSprites;
    PointArrows[] RandomNodeSet;
    bool[] CheckMarkSet;

    // Amount of gestures per set
    int setSize = GameSettings.RythmGameSetSize;

    public enum PointArrows
    {
        Right = 0, Down = 1, Left = 2, nothing
    }
    Wand.WandGestures foundGesture = Wand.WandGestures.nothing;

    private int currenPointer = 0;

    bool isTurn = false;

    public RythmGame()
    {
        NodeSetSprites = new GameObject[setSize];
        RandomNodeSet = new PointArrows[setSize];
        CheckMarkSet = new bool[setSize];
    }

    public void StartRythm(float phaseTime)
    {
        player.Wand.StartRecording(player.GetPlayerNumber(), GameSettings.DefendStateTime);
        player.Wand.ResetRecordedGesture();
        ResetArrowSet();
        succesfullChaneCount = 0;
        isTurn = true;
    }

    public void StopRythm()
    {
        isTurn = false;
        ClearPreviousNodes();
    }

    public float GetAttackerBoost()
    {
        float returnValue = 1;
        returnValue += succesfullChaneCount * GameSettings.RythmBoostPerSet;
        return returnValue;
    }

    private void Start()
    {
        //ResetArrowSet();
    }

    public void Update()
    {
        if (isTurn)
        {
            Wand.WandGestures foundGesture = player.Wand.GetDetectedGesture();

            
            PointArrows detectedArrow = PointArrows.nothing;
            if (Input.GetKeyDown(KeyCode.LeftArrow) || foundGesture == Wand.WandGestures.Scissor)
            {
                detectedArrow = PointArrows.Left;
            }
            if (Input.GetKeyDown(KeyCode.DownArrow) || foundGesture == Wand.WandGestures.Paper)
            {
                detectedArrow = PointArrows.Down;
            }
            if (Input.GetKeyDown(KeyCode.RightArrow) || foundGesture == Wand.WandGestures.Rock)
            {
                detectedArrow = PointArrows.Right;
            }

            if (detectedArrow != PointArrows.nothing)
            {
                checkForDetectedArrow(detectedArrow);
            }

            player.Wand.ResetRecordedGesture();
        }


    }

    private void ResetArrowSet()
    {
        System.Random rand = new System.Random();

        // Fill checkmark and gesture array
        for (int i = 0; i < setSize; i++)
        {
            RandomNodeSet[i] = (PointArrows)rand.Next(0, 3);
            CheckMarkSet[i] = false;
        }

        currenPointer = 0;

        // Do starting animation and spawn sprites
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
            audioSource.pitch = Random.Range(minPitch, maxPitch);
            audioSource.PlayOneShot(hitSound);

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
        // Clear completed nodes if they exist
        ClearPreviousNodes();

        // Create spacing between nodes
        float spacing = 1.2f;
        float startX = -((setSize - 1) * spacing) / 2f;
        Vector3 startPosition = new Vector3(startX, 0, 0);

        // Spawn in nodes according to what is in the random node set
        for (int i = 0; i < setSize; i++)
        {
            GameObject nodeSprite = null;

            if (RandomNodeSet[i] == PointArrows.Down)
            {
                nodeSprite = Instantiate(DownSpritePrefab, NodeContainer);
            }
            else if (RandomNodeSet[i] == PointArrows.Left)
            {
                nodeSprite = Instantiate(LeftSpritePrefab, NodeContainer);
            }
            else if (RandomNodeSet[i] == PointArrows.Right)
            {
                nodeSprite = Instantiate(RightSpritePrefab, NodeContainer);
            }

            if (nodeSprite != null)
            {
                // Place the nodes in the correct positions
                NodeSetSprites[i] = nodeSprite;
                nodeSprite.transform.localPosition = startPosition + new Vector3(0, 0, i * spacing);

                // Create a fade-in effect
                SpriteRenderer spriteRenderer = nodeSprite.GetComponent<SpriteRenderer>();
                if (spriteRenderer != null)
                {
                    StartCoroutine(FadeIn(spriteRenderer, 0.3f, i * 0.1f));
                }
            }
        }
    }

    // Fade-in effect for spawning nodes
    private IEnumerator FadeIn(SpriteRenderer sprite, float duration, float delay)
    {
        yield return new WaitForSeconds(delay); // Delay before starting fade-in

        float elapsedTime = 0f;
        Color originalColor = sprite.color;
        originalColor.a = 0;
        sprite.color = originalColor;

        while (elapsedTime < duration)
        {
            if (sprite == null)
                yield break;

            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsedTime / duration);
            sprite.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        if (sprite != null)
        {
            sprite.color = new Color(originalColor.r, originalColor.g, originalColor.b, 1); // Ensure final alpha is 1
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
        if (NodeSetSprites[currentPoint] != null)
        {
            SpriteRenderer spriteRenderer = NodeSetSprites[currentPoint].GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                StartCoroutine(FadeOut(spriteRenderer));
            }
        }
    }

    private IEnumerator FadeOut(SpriteRenderer sprite)
    {
        if (sprite == null)
        {
            yield break;
        }

        float fadeDuration = 0.5f;
        Color originalColor = sprite.color;

        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            if (sprite == null)
                yield break;

            float normalizedTime = t / fadeDuration;
            sprite.color = new Color(originalColor.r, originalColor.g, originalColor.b, 1 - normalizedTime);
            yield return null;
        }

        if (sprite != null)
        {
            sprite.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0);
            Destroy(sprite.gameObject);
        }
    }
}
