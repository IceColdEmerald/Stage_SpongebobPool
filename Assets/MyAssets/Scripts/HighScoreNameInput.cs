using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class HighScoreNameInput : MonoBehaviour
{
    public static HighScoreNameInput Instance;

    [SerializeField]
    UIDocument uiDocument;

    VisualElement popup;
    Label currentName;

    char[] letters =
    {
        'A',
        'A',
        'A',
        'A',
        'A'
    };

    int currentIndex;
    int score;

    void Awake()
    {
        Instance = this;

        var root =
            uiDocument.rootVisualElement;

        popup =
            root.Q<VisualElement>("NameInputPopup");

        currentName =
            root.Q<Label>("CurrentName");

        popup.style.display = DisplayStyle.None;
    }

    public void Open(int finalScore)
    {
        score = finalScore;

        currentIndex = 0;

        letters[0] = 'A';
        letters[1] = 'A';
        letters[2] = 'A';
        letters[3] = 'A';
        letters[4] = 'A';

        UpdateName();

        popup.style.display =
            DisplayStyle.Flex;
    }

    void Update()
    {
        if (popup.style.display.value != DisplayStyle.Flex)
            return;

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            letters[currentIndex]++;

            if (letters[currentIndex] > 'Z')
                letters[currentIndex] = 'A';

            UpdateName();
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            letters[currentIndex]--;

            if (letters[currentIndex] < 'A')
                letters[currentIndex] = 'Z';

            UpdateName();
        }

        if (Input.GetKeyDown(KeyCode.Space) ||
            Input.GetKeyDown(KeyCode.Return))
        {
            currentIndex++;

            if (currentIndex > 4)
            {
                SaveScore();
            }
        }
    }

    void UpdateName()
    {
        currentName.text =
            $"{letters[0]} {letters[1]} {letters[2]} {letters[3]} {letters[4]}";
    }

    void SaveScore()
    {
        string playerName =
            new string(letters);

        HighScoreManager.Instance.AddScore(
            playerName,
            score);

        SceneManager.LoadScene("HighScoreScene");
    }
}