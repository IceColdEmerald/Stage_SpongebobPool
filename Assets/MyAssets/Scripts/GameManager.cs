using System.Collections;
using TMPro;
using UnityEngine;

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
    [SerializeField] float currentLevelMultiplier = 1f;
    [SerializeField] float rockBookBonus = 0f;

    [Header("Canvas Screen Panels")]
    [SerializeField] GameObject gameScreen;
    [SerializeField] GameObject gameOverScreen;
    [SerializeField] GameObject shopScreen;
    [SerializeField] GameObject transitionScreen;

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
        target = CalculateLevelGoal(level);
        UpdateVisualHUD();

        if (gameScreen != null) gameScreen.SetActive(true);
        if (gameOverScreen != null) gameOverScreen.SetActive(false);
        if (shopScreen != null) shopScreen.SetActive(false);
        if (transitionScreen != null) transitionScreen.SetActive(false);

        LoadCurrentLevelLayout();
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
    }

    int CalculateLevelGoal(int currentLevel)
    {
        if (currentLevel <= 0) return 0;
        return (135 * currentLevel * currentLevel) + (140 * currentLevel) + 375;
    }

    void TriggerGameOver()
    {
        
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
        target = CalculateLevelGoal(level);
        timeRemaining = 60f;
        isGameOver = false;

        UpdateVisualHUD();

        LoadCurrentLevelLayout();

        if (shopScreen != null) shopScreen.SetActive(false);
        if (gameScreen != null) gameScreen.SetActive(true);
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

            //currentLevelInstance.transform.localPosition = Vector3.zero;
            currentLevelInstance.transform.localRotation = Quaternion.identity;
            currentLevelInstance.transform.localScale = Vector3.one;
        }
    }
}
