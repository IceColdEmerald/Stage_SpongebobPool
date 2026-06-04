using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class StartScreenController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private UIDocument uiDocument;

    [Header("Scene")]
    [SerializeField] private string gameSceneName = "GameScene";

    [Header("Bubble Transition")]
    [SerializeField] private int minBubbles = 18;
    [SerializeField] private int maxBubbles = 35;
    [SerializeField] private float minBubbleSize = 25f;
    [SerializeField] private float maxBubbleSize = 110f;
    [SerializeField] private float minSpeed = 150f;
    [SerializeField] private float maxSpeed = 300f;
    [SerializeField] private float extraTransitionTime = 1.5f;
    [SerializeField] private float zigzagSpeed = 5f;
    [SerializeField] private float minZigzagAmount = 20f;
    [SerializeField] private float maxZigzagAmount = 90f;

    private Button startButton;
    private Button highScoresButton;
    private VisualElement bubbleTransition;

    private bool isLoading;

    private void Awake()
    {
        if (uiDocument == null)
            uiDocument = GetComponent<UIDocument>();

        VisualElement root = uiDocument.rootVisualElement;

        startButton = root.Q<Button>("StartButton");
        highScoresButton = root.Q<Button>("HighScoresButton");
        bubbleTransition = root.Q<VisualElement>("BubbleTransition");

        startButton.clicked += OnStartClicked;
        highScoresButton.clicked += OnHighScoresClicked;
    }

    private void Start()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayStartMenuMusic();
    }

    private void OnDestroy()
    {
        if (startButton != null)
            startButton.clicked -= OnStartClicked;

        if (highScoresButton != null)
            highScoresButton.clicked -= OnHighScoresClicked;
    }

    private void OnStartClicked()
    {
        if (isLoading)
            return;

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButtonClick();
            AudioManager.Instance.PlayBubbleTransition();
        }

        StartCoroutine(StartGameTransition());
    }

    private IEnumerator StartGameTransition()
    {
        isLoading = true;

        startButton.SetEnabled(false);
        highScoresButton.SetEnabled(false);

        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(gameSceneName);
        loadOperation.allowSceneActivation = false;

        yield return StartCoroutine(PlayBubbleTransition(loadOperation));

        loadOperation.allowSceneActivation = true;
    }

    private IEnumerator PlayBubbleTransition(AsyncOperation loadOperation)
    {
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

            float size = Random.Range(minBubbleSize, maxBubbleSize);

            float x = Random.Range(0, screenWidth - size);
            float y = screenHeight + Random.Range(20f, 250f);

            bubble.style.width = size;
            bubble.style.height = size;
            bubble.style.left = x;
            bubble.style.top = y;

            bubbleTransition.Add(bubble);

            bubbles[i] = bubble;
            startX[i] = x;
            startY[i] = y;
            speed[i] = Random.Range(minSpeed, maxSpeed);
            zigzagAmount[i] = Random.Range(minZigzagAmount, maxZigzagAmount);
            zigzagOffset[i] = Random.Range(0f, 100f);
        }

        float timer = 0f;
        float extraTimer = 0f;
        bool sceneIsLoaded = false;

        while (true)
        {
            timer += Time.deltaTime;

            for (int i = 0; i < bubbleCount; i++)
            {
                float y = startY[i] - speed[i] * timer;
                float x = startX[i] + Mathf.Sin((timer * zigzagSpeed) + zigzagOffset[i]) * zigzagAmount[i];

                if (y < -maxBubbleSize)
                {
                    startY[i] = screenHeight + Random.Range(20f, 250f);
                    startX[i] = Random.Range(0, screenWidth);
                    y = startY[i];
                }

                bubbles[i].style.top = y;
                bubbles[i].style.left = x;
            }

            if (loadOperation.progress >= 0.9f)
                sceneIsLoaded = true;

            if (sceneIsLoaded)
                extraTimer += Time.deltaTime;

            if (sceneIsLoaded && extraTimer >= extraTransitionTime)
                break;

            yield return null;
        }
    }

    private void OnHighScoresClicked()
    {
        Debug.Log("Open high scores screen");
    }
}