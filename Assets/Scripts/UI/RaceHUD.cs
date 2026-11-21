using UnityEngine;
using TMPro;

public class RaceHUD : MonoBehaviour
{
    [Header("References (TextMeshPro)")]
    public CarController playerCar;
    public TMP_Text speedText;
    public GameObject pausePanel;
    public GameObject resultPanel;
    public TMP_Text resultText;

    [Header("Speed Display")]
    [Tooltip("Prepočet z Unity jednotiek/s na km/h (napr. 3.6f pre reálnejší pocit)")]
    public float speedMultiplier = 3.6f;

    private void Update()
    {
        if (playerCar != null && speedText != null)
        {
            float displaySpeed = Mathf.Abs(playerCar.CurrentSpeed) * speedMultiplier;
            speedText.text = $"{Mathf.RoundToInt(displaySpeed)} km/h";
        }

        if (GameManager.Instance != null && pausePanel != null)
        {
            pausePanel.SetActive(GameManager.Instance.IsPaused && !GameManager.Instance.IsRaceFinished);
        }
    }

    public void ShowResult(bool playerWon, float raceTime, int finalScore)
    {
        if (pausePanel != null)
            pausePanel.SetActive(false);

        if (resultPanel != null)
            resultPanel.SetActive(true);

        if (resultText != null)
        {
            string outcome = playerWon ? "VYHRAL SI!" : "PREHRAL SI!";
            resultText.text = $"{outcome}\nČas: {raceTime:F2} s\nSkóre: {finalScore}";
        }
    }

    // Metódy pre tlačidlá v UI
    public void OnResumeClicked()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsPaused)
        {
            GameManager.Instance.TogglePause();
        }
    }

    public void OnRestartClicked()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RestartLevel();
        }
    }
}