
using UnityEngine;

public interface IDraggable
{
    void OnDragStart(Vector2 hitPoint);

    void OnDrag(Vector2 worldPosition);
    
    void OnDragEnd();
}
