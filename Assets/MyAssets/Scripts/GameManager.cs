using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Economic & Level Settings")]
    [SerializeField] int money = 0;
    [SerializeField] int target = 650;
    [SerializeField] int level = 1;
    [SerializeField] float timeRemaining = 60f;

    [Header("UI Elements")]
    [SerializeField] TMP_Text moneyText;
    [SerializeField] TMP_Text targetText;
    [SerializeField] TMP_Text levelText;
    [SerializeField] TMP_Text timeText;

    [Header("UI Elements (Shop HUD)")]
    [SerializeField] TMP_Text storeMoneyText;

    [Header("Progression Multipliers")]
    [SerializeField] float rockBookBonus = 0f;

    [Header("Canvas Screen Panels")]
    [SerializeField] GameObject gameScreen;
    [SerializeField] GameObject gameOverScreen;
    [SerializeField] GameObject shopScreen;
    [SerializeField] GameObject transitionScreen;
    [SerializeField] GameObject beforeGameScreen;

    [Header("UI Elementes (Intro Screen)")]
    [SerializeField] TMP_Text beforeGameGoalText;

    [Header("Shop Settings")]
    [SerializeField] ShopManager shopManager;

    [Header("Level Design & Spawning")]
    [SerializeField] GameObject[] levelPrefabs;
    [SerializeField] Transform levelSpawnParent;

    public int CurrentMoney => money;
    public float RockCollectorBonus => rockBookBonus;

    GameObject currentLevelInstance;
    bool isGameOver = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        StartCoroutine(StartLevelSequence());
    }

    void Update()
    {
        if (isGameOver) return;

        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;

            int secondsDisplay = (int)Mathf.Ceil(timeRemaining);
            timeText.text = $"{secondsDisplay}";
        }
        else
        {
            timeRemaining = 0;
            timeText.text = "0";
            CheckLevelEndCondition();
        }
    }

    IEnumerator StartLevelSequence()
    {
        isGameOver = true;

        target = CalculateLevelGoal(level);
        UpdateVisualHUD();

        if (beforeGameGoalText != null)
        {
            beforeGameGoalText.text = $"Your goal is ${target}";
        }

        if (gameScreen != null) gameScreen.SetActive(false);
        if (gameOverScreen != null) gameOverScreen.SetActive(false);
        if (shopScreen != null) shopScreen.SetActive(false);
        if (transitionScreen != null) transitionScreen.SetActive(false);
        if (beforeGameScreen != null) beforeGameScreen.SetActive(true);

        LoadCurrentLevelLayout();

        yield return new WaitForSeconds(2f);

        if (beforeGameScreen != null) beforeGameScreen.SetActive(false);
        if (gameScreen != null) gameScreen.SetActive(true);

        isGameOver = false;
    }

    public void AddMoney(int amount)
    {
        if (isGameOver) return;
        money += (amount > 0) ? amount : 1;
        UpdateVisualHUD();
    }

    void CheckLevelEndCondition()
    {
        isGameOver = true;

        if (money >= target)
        {
            StartCoroutine(LevelClearTransitionSequence());
        }
        else
        {
            TriggerGameOver();
        }
    }

    IEnumerator LevelClearTransitionSequence()
    {
        if (gameScreen != null) gameScreen.SetActive(false);
        if (transitionScreen != null) transitionScreen.SetActive(true);

        yield return new WaitForSeconds(3f);

        if (transitionScreen != null) transitionScreen.SetActive(false);
        if (shopScreen != null) shopScreen.SetActive(true);
        if (shopManager != null) shopManager.gameObject.SetActive(true);
    }

    int CalculateLevelGoal(int currentLevel)
    {
        if (currentLevel <= 0) return 0;
        return (135 * currentLevel * currentLevel) + (140 * currentLevel) + 375;
    }

    void TriggerGameOver()
    {
        StartCoroutine(GameOverSequence());
    }

    IEnumerator GameOverSequence()
    {
        if (gameScreen != null) gameScreen.SetActive(false);
        if (gameOverScreen != null) gameOverScreen.SetActive(true);

        yield return new WaitForSeconds(5f);
        SceneManager.LoadScene("StartScreen");
    }

    void UpdateVisualHUD()
    {
        if (moneyText != null) moneyText.text = $"{money}";
        if (targetText != null) targetText.text = $"{target}";
        if (levelText != null) levelText.text = $"{level}";
        if (timeText != null) timeText.text = Mathf.CeilToInt(timeRemaining).ToString();
        if (storeMoneyText != null) storeMoneyText.text = $"{money}";
    }

    public void SpendMoney(int amount)
    {
        money -= amount;
        UpdateVisualHUD();
    }

    public void StartNextLevel()
    {
        level++;
        timeRemaining = 60f;

        if (shopScreen != null) shopScreen.SetActive(false);
        if (shopManager != null) shopManager.gameObject.SetActive(false);

        StartCoroutine(StartLevelSequence());
    }

    void LoadCurrentLevelLayout()
    {
        if (currentLevelInstance != null)
        {
            Destroy(currentLevelInstance);
        }

        if (levelPrefabs == null || levelPrefabs.Length == 0)
        {
            Debug.LogWarning("No Level Prefabs have been assigned to the GameManager script yet.");
            return;
        }

        int targetPrefabIndex = level - 1;

        if (targetPrefabIndex >= levelPrefabs.Length)
        {
            targetPrefabIndex = targetPrefabIndex % levelPrefabs.Length;
        }

        if (levelPrefabs[targetPrefabIndex] != null)
        {
            currentLevelInstance = Instantiate(levelPrefabs[targetPrefabIndex], levelSpawnParent);
            currentLevelInstance.transform.localRotation = Quaternion.identity;
            currentLevelInstance.transform.localScale = Vector3.one;
        }
    }
}
