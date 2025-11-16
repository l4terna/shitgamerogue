
using UnityEngine;

public interface IDraggable
{
    event System.Action<IDraggable> DragCancelled;
    
    void OnDragStart(Vector2 hitPoint);

    void OnDrag(Vector2 worldPosition);
    
    void OnDragEnd();
}
