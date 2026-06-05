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
    [SerializeField] private string highScoreSceneName = "HighScoreScene";

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
        
        isLoading = true;

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButtonClick();
        }

        startButton.SetEnabled(false);
        highScoresButton.SetEnabled(false);

        StartCoroutine(
            BubbleSceneTransition.LoadScene(
                this,
                bubbleTransition,
                gameSceneName));
    }

    private void OnHighScoresClicked()
    {
        if (isLoading)
            return;

        isLoading = true;

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButtonClick();
        }

        startButton.SetEnabled(false);
        highScoresButton.SetEnabled(false);

        StartCoroutine(
            BubbleSceneTransition.LoadScene(
                this,
                bubbleTransition,
                highScoreSceneName));
    }
}