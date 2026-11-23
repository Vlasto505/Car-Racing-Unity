using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public sealed class JungleRaceMusic : MonoBehaviour
{
    private AudioSource music;
    private bool paused;

    private void Awake()
    {
        music = GetComponent<AudioSource>();
    }

    private void Update()
    {
        var race = GameManager.Instance;
        if (race == null)
            return;

        if (race.IsRaceFinished)
        {
            music.Stop();
            enabled = false;
            return;
        }

        if (paused == race.IsPaused)
            return;

        paused = race.IsPaused;
        if (paused)
            music.Pause();
        else
            music.UnPause();
    }
}
