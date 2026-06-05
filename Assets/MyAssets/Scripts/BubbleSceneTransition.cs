using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public static class BubbleSceneTransition
{
    public static IEnumerator LoadScene(
        MonoBehaviour runner,
        VisualElement bubbleTransition,
        string sceneName,
        int minBubbles = 100,
        int maxBubbles = 150,
        float minBubbleSize = 30f,
        float maxBubbleSize = 70f,
        float minSpeed = 300f,
        float maxSpeed = 450f,
        float extraTransitionTime = 2.5f,
        float zigzagSpeed = 5f,
        float minZigzagAmount = 20f,
        float maxZigzagAmount = 90f)
    {
        AsyncOperation loadOperation =
            SceneManager.LoadSceneAsync(sceneName);

        loadOperation.allowSceneActivation = false;

        bubbleTransition.Clear();
        bubbleTransition.AddToClassList("active");

        yield return null;

        float screenWidth = bubbleTransition.worldBound.width;
        float screenHeight = bubbleTransition.worldBound.height;

        if (screenWidth <= 0 || screenHeight <= 0)
        {
            screenWidth = Screen.width;
            screenHeight = Screen.height;
        }

        int bubbleCount = Random.Range(minBubbles, maxBubbles + 1);

        VisualElement[] bubbles = new VisualElement[bubbleCount];

        float[] startX = new float[bubbleCount];
        float[] startY = new float[bubbleCount];
        float[] speed = new float[bubbleCount];
        float[] zigzagAmount = new float[bubbleCount];
        float[] zigzagOffset = new float[bubbleCount];

        for (int i = 0; i < bubbleCount; i++)
        {
            VisualElement bubble = new VisualElement();
            bubble.AddToClassList("bubble");

            float size =
                Random.Range(minBubbleSize, maxBubbleSize);

            float x =
                Random.Range(0, screenWidth - size);

            float y =
                screenHeight + Random.Range(20f, 250f);

            bubble.style.width = size;
            bubble.style.height = size;
            bubble.style.left = x;
            bubble.style.top = y;

            bubbleTransition.Add(bubble);

            bubbles[i] = bubble;
            startX[i] = x;
            startY[i] = y;
            speed[i] = Random.Range(minSpeed, maxSpeed);
            zigzagAmount[i] =
                Random.Range(minZigzagAmount, maxZigzagAmount);
            zigzagOffset[i] =
                Random.Range(0f, 100f);
        }

        float timer = 0f;
        float extraTimer = 0f;
        bool sceneLoaded = false;

        while (true)
        {
            timer += Time.deltaTime;

            for (int i = 0; i < bubbleCount; i++)
            {
                float y =
                    startY[i] - speed[i] * timer;

                float x =
                    startX[i]
                    + Mathf.Sin(
                        (timer * zigzagSpeed)
                        + zigzagOffset[i])
                    * zigzagAmount[i];

                if (y < -maxBubbleSize)
                {
                    startY[i] =
                        screenHeight
                        + Random.Range(20f, 250f);

                    startX[i] =
                        Random.Range(0, screenWidth);

                    y = startY[i];
                }

                bubbles[i].style.top = y;
                bubbles[i].style.left = x;
            }

            if (loadOperation.progress >= 0.9f)
                sceneLoaded = true;

            if (sceneLoaded)
                extraTimer += Time.deltaTime;

            if (sceneLoaded &&
                extraTimer >= extraTransitionTime)
                break;

            yield return null;
        }

        loadOperation.allowSceneActivation = true;
    }
}