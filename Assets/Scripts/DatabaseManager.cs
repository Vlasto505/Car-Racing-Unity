using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using TMPro;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using System.Linq;

public class DatabaseManager : MonoBehaviour
{
    public static DatabaseManager Instance { get; private set; }

    [Header("Voliteľné: URL databázy (ak treba)")]
    [Tooltip("Nechaj prázdne, ak sa načíta z google-services.json, alebo vlož https://...-default-rtdb.firebaseio.com/")]
    [SerializeField] private string databaseUrl = "";

    [Header("UI (Canvas)")]
    [SerializeField] private TMP_Text scoresListText;

    [Header("UI (Canvas)")]
    [SerializeField] private int maxDisplayedScores = 5;

    private DatabaseReference dbRef;
    public bool IsReady { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Result != DependencyStatus.Available)
            {
                Debug.LogError($"Firebase závislosti nie sú dostupné: {task.Result}");
                UpdateScoresUI("Chyba pripojenia k Firebase.");
                return;
            }

            FirebaseDatabase db = string.IsNullOrWhiteSpace(databaseUrl)
                ? FirebaseDatabase.DefaultInstance
                : FirebaseDatabase.GetInstance(databaseUrl);

            dbRef = db.RootReference;
            IsReady = true;
            Debug.Log("Firebase Realtime Database je pripravená.");

            LoadScores();
        });
    }

    public void SaveScore(string playerName, int score)
    {
        if (!IsReady || dbRef == null)
        {
            Debug.LogWarning("Databáza ešte nie je pripravená na zápis.");
            return;
        }

        string cleanName = string.IsNullOrWhiteSpace(playerName) ? "Player" : playerName.Trim();
        string entryKey = dbRef.Child("scores").Push().Key;

        var payload = new Dictionary<string, object>
        {
            { "playerName", cleanName },
            { "score", score },
            { "timestamp", DateTimeOffset.UtcNow.ToUnixTimeSeconds() }
        };

        dbRef.Child("scores").Child(entryKey).SetValueAsync(payload).ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.LogError($"Chyba pri ukladaní skóre: {task.Exception}");
                return;
            }

            Debug.Log($"Skóre uložené: {cleanName} -> {score}");
            LoadScores();
        });
    }

    private struct ScoreEntry
    {
        public string PlayerName;
        public int Score;
        public long Timestamp;
    }

    public void LoadScores()
    {
        if (!IsReady || dbRef == null)
        {
            return;
        }

        dbRef.Child("scores").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.LogError($"Chyba pri načítaní skóre: {task.Exception}");
                UpdateScoresUI("Nepodarilo sa načítať skóre.");
                return;
            }

            DataSnapshot snapshot = task.Result;
            if (!snapshot.Exists)
            {
                Debug.Log("V databáze zatiaľ nie sú žiadne skóre.");
                UpdateScoresUI("Zatiaľ žiadne uložené skóre.");
                return;
            }

            List<ScoreEntry> entries = new List<ScoreEntry>();

            foreach (DataSnapshot child in snapshot.Children)
            {
                string nameVal = child.Child("playerName").Value?.ToString() ?? "Unknown";
                int scoreVal = int.TryParse(child.Child("score").Value?.ToString(), out int parsedScore)
                    ? parsedScore
                    : 0;
                long timeVal = long.TryParse(child.Child("timestamp").Value?.ToString(), out long parsedTime)
                    ? parsedTime
                    : 0;

                entries.Add(new ScoreEntry
                {
                    PlayerName = nameVal,
                    Score = scoreVal,
                    Timestamp = parsedTime
                });
            }

            int limit = Mathf.Max(1, maxDisplayedScores);
            var topEntries = entries
                .OrderByDescending(e => e.Score)
                .ThenByDescending(e => e.Timestamp)
                .Take(limit)
                .ToList();

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Top {topEntries.Count} skóre:");

            for (int i = 0; i < topEntries.Count; i++)
            {
                sb.AppendLine($"{i + 1}. {topEntries[i].PlayerName}: {topEntries[i].Score}");
            }

            UpdateScoresUI(sb.ToString());
        });
    }

    private void UpdateScoresUI(string text)
    {
        if (scoresListText != null)
        {
            scoresListText.text = text;
        }
    }
}