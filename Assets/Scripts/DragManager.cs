using UnityEngine;
using UnityEngine.InputSystem;

public class DragManager : MonoBehaviour
{
    [SerializeField] LayerMask draggableMask;
    [SerializeField] private Camera cam;
    
    private IDraggable _currentDraggable;

    void Awake()
    {
        if (cam == null)
        {
            cam = Camera.main;
        }
    }

    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            TryStartDrag();
        }

        if (Mouse.current.leftButton.isPressed && _currentDraggable != null)
        {
            ContinueDrag();
        }

        if (Mouse.current.leftButton.wasReleasedThisFrame && _currentDraggable != null)
        {
            EndDrag();
        }
    }

    private void EndDrag()
    {
        _currentDraggable.OnDragEnd();
        _currentDraggable = null;
    }

    private void ContinueDrag()
    {
        _currentDraggable.OnDrag(GetScreenWorldPos());
    }

    private void TryStartDrag()
    {
        Vector2 origin = GetScreenWorldPos();

        Collider2D hitCollider = Physics2D.OverlapPoint(origin, draggableMask);

        if (hitCollider)
        {
            _currentDraggable = hitCollider.GetComponent<IDraggable>();
            
            if (_currentDraggable != null)
            {
                _currentDraggable.DragCancelled += OnDragCancelled;
                _currentDraggable.OnDragStart(origin);
            }
        }
    }

    private void OnDragCancelled(IDraggable obj)
    {
        if (_currentDraggable == obj) EndDrag();
    }

    private Vector2 GetScreenWorldPos()
    {
        Vector3 screenPos = Mouse.current.position.ReadValue();
        Vector3 worldPos = cam.ScreenToWorldPoint(screenPos);
        worldPos.z = 0f;

        return worldPos;
    }
}
