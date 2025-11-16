using UnityEngine;

public class Rain : MonoBehaviour
{
    [SerializeField] private float waterIncreasePoints = 0.5f;

    private void OnParticleCollision(GameObject other)
    {
        if (other.CompareTag("Flower"))
        {
            Flower flower = other.GetComponent<Flower>();
            flower?.AddWater(waterIncreasePoints); 
        }
    }

}
