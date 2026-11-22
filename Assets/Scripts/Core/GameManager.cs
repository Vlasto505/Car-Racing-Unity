using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Level Info")]
    public int levelId = 1;
    public float baseScore = 1000f;

    [Header("UI")]
    public RaceHUD raceHUD;

    public bool IsPaused { get; private set; }
    public bool IsRaceFinished { get; private set; }

    private float raceTime;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        Time.timeScale = 1f;
    }

    private void Update()
    {
        if (IsRaceFinished) return;

        if (!IsPaused)
        {
            raceTime += Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        if (IsRaceFinished) return;

        IsPaused = !IsPaused;
        Time.timeScale = IsPaused ? 0f : 1f;
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void FinishRace(bool playerWon)
    {
        if (IsRaceFinished) return;

        IsRaceFinished = true;
        Time.timeScale = 1f;

        int finalScore = playerWon
            ? Mathf.Max(100, Mathf.RoundToInt(baseScore - raceTime * 15f))
            : 50;

        Debug.Log($"{(playerWon ? "VYHRAL SI!" : "PREHRAL SI!")} | Level: {levelId} | Čas: {raceTime:F2}s | Skóre: {finalScore}");
        DatabaseManager.Instance?.SaveScore("user1", finalScore);
        if (raceHUD != null)
        {
            raceHUD.ShowResult(playerWon, raceTime, finalScore);
        }
    }
}