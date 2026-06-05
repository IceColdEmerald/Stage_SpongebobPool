using UnityEngine;
using UnityEngine.UIElements;

public class HighScoreScreenController : MonoBehaviour
{
    [SerializeField] private UIDocument uiDocument;

    [SerializeField] private string startSceneName = "StartScene";

    private Button playAgainButton;
    private VisualElement bubbleTransition;

    private bool isLoading;

    private void Awake()
    {
        VisualElement root = uiDocument.rootVisualElement;

        playAgainButton =
            root.Q<Button>("PlayAgainButton");

        bubbleTransition =
            root.Q<VisualElement>("BubbleTransition");

        playAgainButton.clicked += OnPlayAgainClicked;
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
        print("Play Again Clicked");
        isLoading = true;

        playAgainButton.SetEnabled(false);

        StartCoroutine(
            BubbleSceneTransition.LoadScene(
                this,
                bubbleTransition,
                startSceneName));
    }
}