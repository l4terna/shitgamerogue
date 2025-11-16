using UnityEngine;

public class Rain : MonoBehaviour
{
    [SerializeField] float points = 0.5f;

    private void OnParticleCollision(GameObject other)
    {
        if (other.CompareTag("Flower"))
        {
            Flower flower = other.GetComponent<Flower>();
            if (flower != null)
            {
                flower.AddWater(points);
            }
        }
    }

}
