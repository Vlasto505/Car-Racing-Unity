using UnityEngine;
using TMPro;

public class CarGarageManager : MonoBehaviour
{
    [System.Serializable]
    public class CarItem
    {
        public string carName = "Sport Car";
        public int price = 1000;
        public float speedMultiplier = 1.2f;
        public bool unlockedByDefault = false;
    }

    [Header("Dostupné vozidlá")]
    [SerializeField] private CarItem[] cars;

    [Header("UI v UpgradesPanel")]
    [SerializeField] private TMP_Text statusText;
    [SerializeField] private TMP_Text selectedCarText;
    [SerializeField] private int startingCurrency = 0;

    private const string CurrencyKey = "PlayerCurrency";
    private const string SelectedCarKey = "SelectedCarIndex";


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) || Input.GetKeyDown(KeyCode.F9))
        {
            ResetGarageAndCurrency();
        }
    }
    private void OnEnable()
    {
        if (!PlayerPrefs.HasKey(CurrencyKey))
        {
            PlayerPrefs.SetInt(CurrencyKey, startingCurrency);
        }

        if (cars != null && cars.Length > 0 && cars[0].unlockedByDefault)
        {
            PlayerPrefs.SetInt(GetUnlockKey(0), 1);
        }

        RefreshUI();
    }

    private string GetUnlockKey(int index) => $"CarUnlocked_{index}";

    public bool IsCarUnlocked(int index)
    {
        if (cars == null || index < 0 || index >= cars.Length) return false;
        if (cars[index].unlockedByDefault) return true;
        return PlayerPrefs.GetInt(GetUnlockKey(index), 0) == 1;
    }

    public int GetSelectedCarIndex()
    {
        int idx = PlayerPrefs.GetInt(SelectedCarKey, 0);
        if (cars == null || cars.Length == 0) return 0;
        return Mathf.Clamp(idx, 0, cars.Length - 1);
    }

    public void SelectOrBuyCar(int index)
    {
        if (cars == null || index < 0 || index >= cars.Length) return;

        if (IsCarUnlocked(index))
        {
            PlayerPrefs.SetInt(SelectedCarKey, index);
            PlayerPrefs.Save();
            if (statusText != null) statusText.text = $"Vybraté vozidlo: {cars[index].carName}";
            RefreshUI();
            return;
        }

        int currency = PlayerPrefs.GetInt(CurrencyKey, 0);
        int price = cars[index].price;

        if (currency >= price)
        {
            currency -= price;
            PlayerPrefs.SetInt(CurrencyKey, currency);
            PlayerPrefs.SetInt(GetUnlockKey(index), 1);
            PlayerPrefs.SetInt(SelectedCarKey, index);
            PlayerPrefs.Save();

            if (statusText != null) statusText.text = $"Kúpené a vybraté: {cars[index].carName}!";
        }
        else
        {
            if (statusText != null) statusText.text = $"Nedostatok bodov! Potrebuješ {price}, máš {currency}.";
        }

        RefreshUI();
    }

    private void RefreshUI()
    {
        if (cars == null || cars.Length == 0) return;

        int selected = GetSelectedCarIndex();
        int currency = PlayerPrefs.GetInt(CurrencyKey, 0);

        if (selectedCarText != null)
        {
            selectedCarText.text = $"Aktuálne auto: {cars[selected].carName} | Body: {currency}";
        }
    }
    public void ResetGarageAndCurrency()
    {
        if (cars != null)
        {
            for (int i = 0; i < cars.Length; i++)
            {
                PlayerPrefs.DeleteKey(GetUnlockKey(i));
            }

            if (cars.Length > 0 && cars[0].unlockedByDefault)
            {
                PlayerPrefs.SetInt(GetUnlockKey(0), 1);
            }
        }

        PlayerPrefs.SetInt(SelectedCarKey, 0);
        PlayerPrefs.SetInt(CurrencyKey, startingCurrency);
        PlayerPrefs.Save();

        if (statusText != null)
        {
            statusText.text = "Garáž a body boli resetované.";
        }

        RefreshUI();
        Debug.Log("CarGarageManager: Garáž a body boli resetované.");
    }
}