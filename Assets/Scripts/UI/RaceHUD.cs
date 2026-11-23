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

    [Header("Win Reward")]
    [SerializeField] private int winReward = 300;

    private const string CurrencyKey = "PlayerCurrency";
    private int pickupPointsThisRace = 0;
    private bool rewardGranted = false;

    public void AddPickupPoints(int points)
    {
        if (points <= 0) return;
        pickupPointsThisRace += points;
    }

    public void SetTargetCar(GameObject activeCar)
    {
        if (activeCar == null) return;

        CarController controller = activeCar.GetComponent<CarController>();
        if (controller == null)
        {
            controller = activeCar.GetComponentInChildren<CarController>();
        }

        if (controller != null)
        {
            playerCar = controller;
        }
        else
        {
            Debug.LogWarning($"RaceHUD: Na objekte '{activeCar.name}' sa nenašiel komponent CarController.");
        }
    }

    public void SetTargetCar(CarController controller)
    {
        if (controller != null)
        {
            playerCar = controller;
        }
    }

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
        int winBonus = 0;

        if (!rewardGranted)
        {
            rewardGranted = true;

            if (playerWon)
            {
                winBonus = winReward + Mathf.Max(0, finalScore);
                int currentCurrency = PlayerPrefs.GetInt(CurrencyKey, 0);
                PlayerPrefs.SetInt(CurrencyKey, currentCurrency + winBonus);
                PlayerPrefs.Save();
            }
        }

        int totalCurrency = PlayerPrefs.GetInt(CurrencyKey, 0);

        if (pausePanel != null)
            pausePanel.SetActive(false);

        if (resultPanel != null)
            resultPanel.SetActive(true);

        if (resultText != null)
        {
            string outcome = playerWon ? "VYHRAL SI!" : "PREHRAL SI!";
            resultText.text =
                $"{outcome}\n" +
                $"Čas: {raceTime:F2} s\n" +
                $"Skóre: {finalScore}\n" +
                $"Pickup body v preteku: +{pickupPointsThisRace}\n" +
                $"Bonus za výhru: +{winBonus}\n" +
                $"Celkové body: {totalCurrency}";
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