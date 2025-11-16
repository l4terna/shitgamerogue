using UnityEngine;

public class Rain : MonoBehaviour
{
    [SerializeField] public float waterIncreasePoints = 0.5f;

    private void OnParticleCollision(GameObject other)
    {
        if (other.CompareTag("Flower"))
        {
            Flower flower = other.GetComponentInParent<Flower>();
            flower?.AddWater(waterIncreasePoints); 
        }
    }

}
