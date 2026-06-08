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

    int currentSelection = 0;

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

        UpdateArcadeFocus();
    }

    void Update()
    {
        if (isLoading) return;

        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            currentSelection = (currentSelection == 0) ? 1 : 0;
            UpdateArcadeFocus();
        }

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
        {
            ExecuteSelection();
        }
    }

    void UpdateArcadeFocus()
    {
        if (startButton != null)
        {
            startButton.style.scale = new StyleScale(StyleKeyword.Null);
        }
        if (highScoresButton != null)
        {
            highScoresButton.style.scale = new StyleScale(StyleKeyword.Null);
        }

        if (currentSelection == 0 && startButton != null)
        {
            startButton.Focus();
            startButton.style.scale = new StyleScale(new Scale(new Vector3(1.1f, 1.1f, 1f)));
        }
        else if (currentSelection == 1 && highScoresButton != null)
        {
            highScoresButton.Focus();
            highScoresButton.style.scale = new StyleScale(new Scale(new Vector3(1.1f, 1.1f, 1f)));
        }
    }

    void ExecuteSelection()
    {
        if (currentSelection == 0)
        {
            OnStartClicked();
        }
        else if (currentSelection == 1)
        {
            OnHighScoresClicked();
        }
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