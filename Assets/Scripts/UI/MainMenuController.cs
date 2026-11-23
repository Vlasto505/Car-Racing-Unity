using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [Header("Panely v MainMenu")]
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private GameObject levelSelectPanel;
    [SerializeField] private GameObject upgradesPanel;
    [SerializeField] private GameObject topScoresPanel;

    [Header("Názvy scén levelov")]
    [SerializeField] private string level1SceneName = "SandboxTest";
    [SerializeField] private string level2SceneName = "Level2";
    [SerializeField] private string level3SceneName = "Level3";

    private void Start()
    {
        Time.timeScale = 1f;
        ShowMainPanel();
    }

    public void ShowMainPanel()
    {
        SetPanels(main: true, levels: false, upgrades: false, scores: false);
    }

    public void OpenLevelSelect()
    {
        SetPanels(main: false, levels: true, upgrades: false, scores: false);
    }

    public void OpenUpgrades()
    {
        SetPanels(main: false, levels: false, upgrades: true, scores: false);
    }

    public void OpenTopScores()
    {
        SetPanels(main: false, levels: false, upgrades: false, scores: true);

        if (DatabaseManager.Instance != null && DatabaseManager.Instance.IsReady)
        {
            DatabaseManager.Instance.LoadScores();
        }
    }

    public void LoadLevel1()
    {
        LoadSceneByName(level1SceneName);
    }

    public void LoadLevel2()
    {
        LoadSceneByName(level2SceneName);
    }

    public void LoadLevel3()
    {
        LoadSceneByName(level3SceneName);
    }

    public void LoadSceneByName(string sceneName)
    {
        if (string.IsNullOrWhiteSpace(sceneName))
        {
            Debug.LogWarning("Názov scény nie je zadaný.");
            return;
        }

        SceneManager.LoadScene(sceneName);
    }

    private void SetPanels(bool main, bool levels, bool upgrades, bool scores)
    {
        if (mainPanel != null) mainPanel.SetActive(main);
        if (levelSelectPanel != null) levelSelectPanel.SetActive(levels);
        if (upgradesPanel != null) upgradesPanel.SetActive(upgrades);
        if (topScoresPanel != null) topScoresPanel.SetActive(scores);
    }
}