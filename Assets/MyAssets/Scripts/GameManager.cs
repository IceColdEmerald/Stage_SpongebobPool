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

    [Header("Progression Multipliers")]
    [SerializeField] float currentLevelMultiplier = 1f;
    [SerializeField] float rockBookBonus = 0f;

    public int CurrentMoney => money;
    public float RockCollectorBonus => rockBookBonus;

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
            NextLevel();
        }
        else
        {
            TriggerGameOver();
        }
    }

    void NextLevel()
    {
        level++;
        target = CalculateLevelGoal(level);

        timeRemaining = 60f;
        isGameOver = false;
        UpdateVisualHUD();
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
    }

    public void SpendMoney(int amount)
    {
        money -= amount;
        UpdateVisualHUD();
    }

    public void StartNextLevel()
    {
        NextLevel();
    }
}
