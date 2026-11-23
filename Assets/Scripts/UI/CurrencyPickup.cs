using UnityEngine;

public class CurrencyPickup : MonoBehaviour
{
    [SerializeField] private int pointsToAdd = 200;
    private const string CurrencyKey = "PlayerCurrency";
    private bool collected;

    private void OnTriggerEnter(Collider other)
    {
        if (collected) return;

        CarController car = other.GetComponentInParent<CarController>();
        if (car == null) return;

        collected = true;

        int currentCurrency = PlayerPrefs.GetInt(CurrencyKey, 0);
        currentCurrency += pointsToAdd;
        PlayerPrefs.SetInt(CurrencyKey, currentCurrency);
        PlayerPrefs.Save();

        RaceHUD hud = FindFirstObjectByType<RaceHUD>();
        if (hud != null)
        {
            hud.AddPickupPoints(pointsToAdd);
        }

        gameObject.SetActive(false);
    }
}