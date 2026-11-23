using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public sealed class RaceResultMainMenu : MonoBehaviour
{
    private RaceHUD raceHud;
    private Button resultMenuButton;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Initialize()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
        OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainMenu")
        {
            new GameObject("Menu Background Setup").AddComponent<RaceResultMainMenu>();
            return;
        }

        if (FindAnyObjectByType<GameManager>() == null ||
            FindAnyObjectByType<RaceResultMainMenu>() != null)
            return;

        var navigation = new GameObject("Race Result Navigation").AddComponent<RaceResultMainMenu>();
        navigation.raceHud = FindAnyObjectByType<RaceHUD>();
    }

    private void Start()
    {
        if (SceneManager.GetActiveScene().name == "MainMenu")
            AddBackground();
    }

    private static void AddBackground()
    {
        var canvas = GameObject.Find("MainPanel")?.transform.parent;
        var imageData = Resources.Load<TextAsset>("RacingMenuArtwork");
        if (canvas == null || imageData == null)
            return;

        var texture = new Texture2D(2, 2);
        if (!ImageConversion.LoadImage(texture, imageData.bytes, true))
        {
            Destroy(texture);
            return;
        }

        foreach (string pageName in new[] { "MainPanel", "LevelSelectPanel", "UpgradesPanel", "TopScoresPanel" })
        {
            var panel = canvas.Find(pageName);
            if (panel == null || panel.Find("Racing Background") != null)
                continue;

            var background = new GameObject("Racing Background", typeof(RectTransform), typeof(RawImage));
            background.layer = panel.gameObject.layer;
            background.transform.SetParent(panel, false);
            background.transform.SetAsFirstSibling();
            var image = background.GetComponent<RawImage>();
            image.texture = texture;
            image.color = pageName == "MainPanel" ? Color.white : new Color(.5f, .5f, .5f);
            image.raycastTarget = false;
            image.rectTransform.anchorMin = Vector2.zero;
            image.rectTransform.anchorMax = Vector2.one;
            image.rectTransform.offsetMin = Vector2.zero;
            image.rectTransform.offsetMax = Vector2.zero;
            var aspect = background.AddComponent<AspectRatioFitter>();
            aspect.aspectMode = AspectRatioFitter.AspectMode.EnvelopeParent;
            aspect.aspectRatio = (float)texture.width / texture.height;
        }
    }

    private void OnGUI()
    {
        var race = GameManager.Instance;
        if (race == null || (!race.IsRaceFinished && !race.IsPaused))
            return;

        var activePanel = raceHud == null ? null : (race.IsRaceFinished ? raceHud.resultPanel : raceHud.pausePanel);
        if (activePanel != null && activePanel.activeInHierarchy)
        {
            var panel = activePanel.transform;
            if (panel.GetComponentInChildren<RaceResultMainMenuButton>(true) != null)
                return;

            foreach (var restart in panel.GetComponentsInChildren<Button>(true))
            {
                var label = restart.GetComponentInChildren<TMP_Text>(true);
                string action = race.IsRaceFinished ? "Restart" : "Resume";
                if (label == null || (!restart.name.Equals(action + "Button", System.StringComparison.OrdinalIgnoreCase)
                    && !label.text.Trim().Equals(action, System.StringComparison.OrdinalIgnoreCase)))
                    continue;

                Button template = restart;
                if (!race.IsRaceFinished && raceHud.resultPanel != null)
                {
                    foreach (var candidate in raceHud.resultPanel.GetComponentsInChildren<Button>(true))
                        if (candidate.name == "RestartButton")
                        {
                            template = candidate;
                            break;
                        }
                }
                var menu = Instantiate(template, restart.transform.parent);
                ((RectTransform)menu.transform).anchorMin = ((RectTransform)restart.transform).anchorMin;
                ((RectTransform)menu.transform).anchorMax = ((RectTransform)restart.transform).anchorMax;
                ((RectTransform)menu.transform).anchoredPosition = ((RectTransform)restart.transform).anchoredPosition;
                menu.gameObject.SetActive(true);
                menu.name = "Main Menu";
                menu.gameObject.AddComponent<RaceResultMainMenuButton>();
                menu.transform.SetSiblingIndex(restart.transform.GetSiblingIndex() + 1);
                menu.GetComponentInChildren<TMP_Text>(true).text = "Main Menu";
                menu.onClick = new Button.ButtonClickedEvent();
                menu.onClick.AddListener(ReturnToMenu);
                var rect = (RectTransform)menu.transform;
                if (menu.transform.parent.GetComponent<LayoutGroup>() == null)
                {
                    var restartRect = (RectTransform)restart.transform;
                    if (!race.IsRaceFinished)
                    {
                        rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, restartRect.rect.width);
                        rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, restartRect.rect.height);
                        rect.anchoredPosition = restartRect.anchoredPosition -
                            Vector2.up * (restartRect.rect.height + 14f);
                        var resumeLabel = restart.GetComponentInChildren<TMP_Text>(true);
                        var menuLabel = menu.GetComponentInChildren<TMP_Text>(true);
                        menuLabel.fontSize = resumeLabel.fontSize;
                        menuLabel.enableAutoSizing = resumeLabel.enableAutoSizing;
                        menuLabel.fontSizeMin = resumeLabel.fontSizeMin;
                        menuLabel.fontSizeMax = resumeLabel.fontSizeMax;
                        return;
                    }

                    float width = Mathf.Max(rect.rect.width, 1f);
                    rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width * .64f);
                    restartRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width * .64f);
                    Vector2 center = restartRect.anchoredPosition;
                    float offset = width * .32f + 6f;
                    restartRect.anchoredPosition = center - Vector2.right * offset;
                    rect.anchoredPosition = center + Vector2.right * offset;
                }
                return;
            }
        }

        GUI.depth = -10;
        if (race.IsPaused && !race.IsRaceFinished && SceneManager.GetActiveScene().name == "Level1")
        {
            DrawJunglePause(race);
            return;
        }
        var button = new Rect((Screen.width - 132f) * .5f, Screen.height * .5f + 29f, 132f, 28f);
        if (GUI.Button(button, "Main Menu"))
            ReturnToMenu();
    }

    private static void DrawJunglePause(GameManager race)
    {
        float scale = Mathf.Min(Screen.width / 800f, Screen.height / 450f);
        var originalMatrix = GUI.matrix;
        var originalColor = GUI.color;
        float centerX = Screen.width * .5f;
        float centerY = Screen.height * .5f;

        GUI.color = new Color(1f, 1f, 1f, .48f);
        GUI.DrawTexture(new Rect(0f, 0f, Screen.width, Screen.height), Texture2D.whiteTexture);
        GUI.color = Color.white;
        GUIUtility.ScaleAroundPivot(Vector2.one * scale, new Vector2(centerX, centerY));

        var title = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontSize = 40 };
        GUI.Label(new Rect(centerX - 180f, centerY - 105f, 360f, 55f), "Game Paused", title);
        var button = new GUIStyle(GUI.skin.button) { fontSize = 24 };
        GUI.backgroundColor = new Color(.56f, 1f, .42f);
        bool resume = GUI.Button(new Rect(centerX - 100f, centerY - 28f, 200f, 34f), "RESUME", button);
        GUI.backgroundColor = new Color(1f, .45f, .45f);
        bool menu = GUI.Button(new Rect(centerX - 100f, centerY + 20f, 200f, 34f), "Main Menu", button);
        GUI.backgroundColor = Color.white;
        GUI.color = originalColor;
        GUI.matrix = originalMatrix;

        if (resume)
            race.TogglePause();
        if (menu)
            ReturnToMenu();
    }

    private static void ReturnToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}



public sealed class RaceResultMainMenuButton : MonoBehaviour {}





