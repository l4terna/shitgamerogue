using System;
using UnityEngine;

public class CloudRestrictor : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        var collisionPoint = other.ClosestPoint(transform.position);
        Debug.Log(collisionPoint);
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        var collisionPoint = other.ClosestPoint(transform.position);
        Debug.Log(collisionPoint);
    }
}
