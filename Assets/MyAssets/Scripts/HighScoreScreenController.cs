using UnityEngine;
using UnityEngine.UIElements;

public class HighScoreScreenController : MonoBehaviour
{
    [SerializeField] private UIDocument uiDocument;

    [SerializeField] private string startSceneName = "StartScene";

    private Button playAgainButton;
    private VisualElement bubbleTransition;

    private bool isLoading;

    private void Start()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayShopMusic();
    }

    private void Awake()
    {
        VisualElement root = uiDocument.rootVisualElement;

        playAgainButton =
            root.Q<Button>("PlayAgainButton");

        bubbleTransition =
            root.Q<VisualElement>("BubbleTransition");

        playAgainButton.clicked += OnPlayAgainClicked;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
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
        print("Play Again Clicked");
        isLoading = true;

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButtonClick();
        }

        playAgainButton.SetEnabled(false);

        StartCoroutine(
            BubbleSceneTransition.LoadScene(
                this,
                bubbleTransition,
                startSceneName));
    }
}