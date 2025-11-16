using UnityEngine;

public class Rain : MonoBehaviour
{
    [SerializeField] private float waterIncreasePoint = 0.5f;

    private void OnParticleCollision(GameObject other)
    {
        if (other.CompareTag("Flower"))
        {
            Flower flower = other.GetComponentInParent<Flower>();
            flower?.AddWater(waterIncreasePoint);
        }
    }

}
