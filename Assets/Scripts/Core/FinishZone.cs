using UnityEngine;

public class FinishZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (GameManager.Instance == null || GameManager.Instance.IsRaceFinished) return;

        if (other.GetComponentInParent<CarController>() != null)
        {
            GameManager.Instance.FinishRace(true);
        }
        else if (other.GetComponentInParent<AICarController>() != null)
        {
            GameManager.Instance.FinishRace(false);
        }
    }
}