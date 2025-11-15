using UnityEngine;
using UnityEngine.InputSystem;

public class CursorBootstrap : MonoBehaviour
{
    [SerializeField] private Texture2D handTexture;
    [SerializeField] private Texture2D handGrabTexture;
    
    void Start()
    {
        SetNormal();
    }

    void Update()
    {
        if (Mouse.current == null) return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            SetClicking();
        }

        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            SetNormal();
        }
    }

    private void SetNormal()
    {
        Cursor.SetCursor(handTexture, Vector2.zero, CursorMode.Auto);
    }

    private void SetClicking()
    {
        Cursor.SetCursor(handGrabTexture, Vector2.zero, CursorMode.Auto);
    }
}
