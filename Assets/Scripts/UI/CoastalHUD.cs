using UnityEngine;
using UnityEngine.SceneManagement;

public class CoastalHUD : MonoBehaviour
{
    public CarController car;
    private float elapsed;

    private void Update()
    {
        var race = GameManager.Instance;
        if (!race.IsPaused && !race.IsRaceFinished) elapsed += Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.R))
        {
            Time.timeScale = 1;
            Restart();
        }
    }

    private static void Restart()
    {
        Time.timeScale = 1;
        var scene = SceneManager.GetActiveScene();
        #if UNITY_EDITOR
        if (scene.buildIndex < 0)
        {
            UnityEditor.SceneManagement.EditorSceneManager.LoadSceneInPlayMode(
                scene.path, new LoadSceneParameters(LoadSceneMode.Single));
            return;
        }
        #endif
        SceneManager.LoadScene(scene.buildIndex);
    }

    private void OnGUI()
    {
        var race = GameManager.Instance;
        if (race == null) return;
        GUI.Box(new Rect(18,18,220,81), "COASTAL RUN");
        GUI.Label(new Rect(34,42,195,24), $"{Mathf.Abs(car.CurrentSpeed)*3.6f:F0} km/h     {elapsed:F1} s");
        GUI.Label(new Rect(34,67,195,24), "WASD / Arrows   R Restart");
        if (race.IsRaceFinished || race.IsPaused)
        {
            var rect = new Rect(Screen.width*.5f-125, Screen.height*.5f-40, 250, 80);
            GUI.Box(rect,race.IsRaceFinished ? $"Finish   {elapsed:F2} s" : "Paused");
            if (GUI.Button(new Rect(rect.x+59,rect.y+33,132,28),race.IsRaceFinished ? "Race again" : "Continue"))
            {
                if (race.IsRaceFinished) Restart();
                else race.TogglePause();
            }
        }
    }
}
