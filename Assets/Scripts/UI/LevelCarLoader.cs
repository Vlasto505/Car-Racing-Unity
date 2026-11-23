using UnityEngine;

public class LevelCarLoader : MonoBehaviour
{
    [Header("Vozidlá v scéne (v rovnakom poradí ako v garáži)")]
    [SerializeField] private GameObject[] carObjects;
    [SerializeField] private float[] speedMultipliers;

    [Header("Kamera, ktorá má sledovať aktívne auto")]
    [SerializeField] private FollowCamera targetCameraFollow;

    [Header("HUD")]
    [SerializeField] private RaceHUD raceHUD;

    private const string SelectedCarKey = "SelectedCarIndex";

    private void Awake()
    {
        if (carObjects == null || carObjects.Length == 0)
        {
            Debug.LogWarning("LevelCarLoader: Zoznam Car Objects nie je nastavený v Inspectore.");
            return;
        }

        int selectedIndex = Mathf.Clamp(PlayerPrefs.GetInt(SelectedCarKey, 0), 0, carObjects.Length - 1);

        for (int i = 0; i < carObjects.Length; i++)
        {
            if (carObjects[i] != null)
            {
                carObjects[i].SetActive(i == selectedIndex);
            }
        }

        GameObject activeCar = carObjects[selectedIndex];
        if (activeCar == null) return;

        float multiplier = (speedMultipliers != null && selectedIndex < speedMultipliers.Length)
            ? speedMultipliers[selectedIndex]
            : 1f;

        CarController controller = activeCar.GetComponent<CarController>();
        if (controller == null)
        {
            controller = activeCar.GetComponentInChildren<CarController>();
        }

        if (controller != null)
        {
            controller.ApplySpeedMultiplier(multiplier);
        }

        if (targetCameraFollow != null)
        {
            targetCameraFollow.SetTarget(activeCar.transform);
        }

        if (raceHUD != null)
        {
            raceHUD.SetTargetCar(activeCar);
        }

        Debug.Log($"Načítané vozidlo index {selectedIndex} ({activeCar.name}) s násobičom rýchlosti {multiplier}x.");
    }
}