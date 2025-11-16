using UnityEngine;

public class Rain : MonoBehaviour
{
    [SerializeField] float points = 0.5f;

    private void OnParticleCollision(GameObject other)
    {
        Debug.Log(other.name);

        // Пытаемся найти Flower в родителях объекта
        Flower flower = other.GetComponentInParent<Flower>();

        if (flower != null)
        {
            flower.AddWater(points); // добавляем воду
        }
    }
}
