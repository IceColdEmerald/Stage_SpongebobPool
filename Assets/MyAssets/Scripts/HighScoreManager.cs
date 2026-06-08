using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class HighScoreManager : MonoBehaviour
{
    public static HighScoreManager Instance { get; private set; }

    private HighScoreData data = new();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Bootstrap()
    {
        if (Instance == null)
        {
            GameObject go = new GameObject("HighScoreManager");
            go.AddComponent<HighScoreManager>();
        }
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadScores();
    }

    public int GetBestScore()
    {
        if (data.entries.Count == 0)
            return 0;

        return data.entries[0].score;
    }

    public bool IsHighScore(int score)
    {
        if (data.entries.Count < 5)
            return true;

        return score > data.entries.Last().score;
    }

    public void AddScore(string playerName, int score)
    {
        data.entries.RemoveAll(x => x.playerName == playerName);

        data.entries.Add(new HighScoreEntry()
        {
            playerName = playerName,
            score = score
        });

        data.entries = data.entries
            .OrderByDescending(x => x.score)
            .Take(5)
            .ToList();

        SaveScores();
    }

    public List<HighScoreEntry> GetScores()
    {
        return data.entries;
    }

    void SaveScores()
    {
        string json = JsonUtility.ToJson(data);

        PlayerPrefs.SetString("HighScores", json);
        PlayerPrefs.Save();
    }

    void LoadScores()
    {
        if (PlayerPrefs.HasKey("HighScores"))
        {
            string json = PlayerPrefs.GetString("HighScores");
            data = JsonUtility.FromJson<HighScoreData>(json);
        }

        if (data == null)
            data = new HighScoreData();
    }

    [ContextMenu("Reset Leaderboard")]
    public void ResetLeaderboard()
    {
        data.entries.Clear();
        SaveScores();

        Debug.Log("Leaderboard reset!");
    }
}