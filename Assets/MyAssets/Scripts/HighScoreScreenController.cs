using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class HighScoreScreenController : MonoBehaviour
{
    private VisualElement nameInputPopup;
    private Label currentNameLabel;
    private Label bestScoreLabel;
    [SerializeField] private UIDocument uiDocument;
    [SerializeField] private string startSceneName = "StartScene";

    private Button playAgainButton;
    private VisualElement bubbleTransition;

    private Label[] nameLabels = new Label[5];
    private Label[] scoreLabels = new Label[5];

    bool isLoading;

    private void Start()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayShopMusic();

        RefreshLeaderboard();
        UpdateBestScore();
    }

    private void Awake()
    {
        VisualElement root = uiDocument.rootVisualElement;

        playAgainButton = root.Q<Button>("PlayAgainButton");
        bubbleTransition = root.Q<VisualElement>("BubbleTransition");
        bestScoreLabel = uiDocument.rootVisualElement.Q<Label>("BestScoreValue");
        nameInputPopup = root.Q<VisualElement>("NameInputPopup");
        currentNameLabel = root.Q<Label>("CurrentName");

        for (int i = 0; i < 5; i++)
        {
            nameLabels[i] = root.Q<Label>($"Name{i + 1}");
            scoreLabels[i] = root.Q<Label>($"Score{i + 1}");
        }

        playAgainButton.clicked += OnPlayAgainClicked;
    }

    void UpdateBestScore()
    {
        var scores =
            HighScoreManager.Instance.GetScores();

        if (scores.Count > 0)
        {
            bestScoreLabel.text = HighScoreManager.Instance
            .GetBestScore()
            .ToString();
        }
        else
        {
            bestScoreLabel.text = "0";
        }
    }

    void RefreshLeaderboard()
    {
        Debug.Log("HIGH SCORE MANAGER INSTANCE = " + HighScoreManager.Instance);
        List<HighScoreEntry> scores =
            HighScoreManager.Instance.GetScores();

        for (int i = 0; i < 5; i++)
        {
            if (i < scores.Count)
            {
                nameLabels[i].text = scores[i].playerName;
                scoreLabels[i].text = scores[i].score.ToString();
            }
            else
            {
                nameLabels[i].text = "---";
                scoreLabels[i].text = "0";
            }
        }
        
        UpdateBestScore();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) ||
            Input.GetKeyDown(KeyCode.Return))
        {
            OnPlayAgainClicked();
        }
    }

    private void OnDestroy()
    {
        if (playAgainButton != null)
            playAgainButton.clicked -= OnPlayAgainClicked;
    }

    private void OnPlayAgainClicked()
    {
        if (isLoading)
            return;

        isLoading = true;

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayButtonClick();

        playAgainButton.SetEnabled(false);

        StartCoroutine(
            BubbleSceneTransition.LoadScene(
                this,
                bubbleTransition,
                startSceneName));
    }
}