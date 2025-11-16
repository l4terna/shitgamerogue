using UnityEngine;

public class Rain : MonoBehaviour
{
    [SerializeField] float points = 0.5f;

    private void OnParticleCollision(GameObject other)
    {
        Debug.Log(other.name);

        // Проверяем тег объекта
        if (other.CompareTag("Flower"))
        {
            Flower flower = other.GetComponent<Flower>();
            if (flower != null)
            {
                flower.AddWater(points); // добавляем воду
            }
        }
    }

}
