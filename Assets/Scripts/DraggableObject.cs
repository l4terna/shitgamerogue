using UnityEngine;

public class DraggableObject : MonoBehaviour, IDraggable
{
    private Vector2 _offset;
    private Camera _cam;
    private Collider2D _parentCollider;
    private Collider2D _collider;
    
    private void Awake()
    {
        _cam = Camera.main;
        _parentCollider = transform.parent.GetComponent<Collider2D>();
        _collider = GetComponent<BoxCollider2D>();
    }

    public void OnDragStart(Vector2 hitPoint)
    {
        _offset = (Vector2)transform.position - hitPoint;
    }

    public void OnDrag(Vector2 worldPosition)
    {
        Vector2 targetPost = worldPosition + _offset;
        transform.position = targetPost;
        
        Bounds areaBounds = _parentCollider.bounds;
        Bounds capBounds = _collider.bounds;
        Vector2 extents = capBounds.extents;

        float minX = areaBounds.min.x + extents.x;
        float maxX = areaBounds.max.x - extents.x;
        float minY = areaBounds.min.y + extents.y;
        float maxY = areaBounds.max.y - extents.y;

        Vector3 pos = transform.position;

        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);

        transform.position = pos;
    }

    public void OnDragEnd()
    {
        Debug.Log("DraggableObject OnDragEnd");
    }
}
