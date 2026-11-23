using UnityEngine;
using UnityEngine.UI;

internal static class GarageSelectedCarHighlight
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Register()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
        Apply(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
    }

    private static void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene,
        UnityEngine.SceneManagement.LoadSceneMode mode)
    {
        Apply(scene);
    }

    private static void Apply(UnityEngine.SceneManagement.Scene scene)
    {
        if (scene.name != "MainMenu")
            return;

        var garage = Object.FindAnyObjectByType<CarGarageManager>();
        foreach (var button in Object.FindObjectsByType<Button>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            var image = button.GetComponent<RawImage>();
            if (image == null || button.transform.parent.name != "UpgradesPanel")
                continue;

            int index = button.name switch { "Car1" => 0, "Car2" => 1, "Car3" => 2, _ => -1 };
            if (index < 0 || garage == null || image.GetComponent<Outline>() != null)
                continue;

            var border = image.gameObject.AddComponent<Outline>();
            border.effectColor = new Color(1f, .78f, .22f, 1f);
            border.effectDistance = new Vector2(4f, 4f);
            void Refresh() => border.enabled = garage.GetSelectedCarIndex() == index;
            Refresh();
            foreach (var other in button.transform.parent.GetComponentsInChildren<Button>(true))
                other.onClick.AddListener(Refresh);
        }
    }
}

