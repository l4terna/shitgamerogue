using UnityEngine;

public class Snowfall : MonoBehaviour
{
    [SerializeField] public float temperatureDecreasePoints = -0.5f;
    [SerializeField] public float waterIncreasePoints = 0.1f;

    private void OnParticleCollision(GameObject other)
    {
        if (other.CompareTag("Flower"))
        {
            Flower flower = other.GetComponentInParent<Flower>();
            flower?.AddTemperature(temperatureDecreasePoints);
            flower?.AddWater(waterIncreasePoints);
        }
    }
}
