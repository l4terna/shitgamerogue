using System;
using System.Collections;
using UnityEngine;

public class CloudDragging : MonoBehaviour, IDraggable
{
    [SerializeField] private float windMoveDuration = 0.3f;

    private Vector2 _offset;
    private Vector2 _windOffset;      
    private bool _isDragging;

    private Collider2D _parentCollider;
    private Collider2D _collider;
    private WeatherSwitcher _weatherSwitcher;

    private Coroutine _windMoveRoutine;

    public event Action<IDraggable> DragCancelled;

    private void Awake()
    {
        _parentCollider = transform.parent.GetComponent<Collider2D>();
        _collider = GetComponent<Collider2D>();
        _weatherSwitcher = GetComponent<WeatherSwitcher>();
    }

    public void OnDragStart(Vector2 hitPoint)
    {
        if (_weatherSwitcher && _weatherSwitcher.CurrentWeather == WeatherSwitcher.WeatherType.Snowfall)
        {
            DragCancelled?.Invoke(this);
            return;
        }

        _isDragging = true;

        if (_windMoveRoutine != null)
        {
            StopCoroutine(_windMoveRoutine);
            _windMoveRoutine = null;
        }

        _offset = (Vector2)transform.position - hitPoint - _windOffset;
    }


    public void OnDrag(Vector2 worldPosition)
    {
        if (_weatherSwitcher && _weatherSwitcher.CurrentWeather == WeatherSwitcher.WeatherType.Snowfall)
        {
            DragCancelled?.Invoke(this);
            return;
        }

        Vector2 targetPos = worldPosition + _offset + _windOffset;
        transform.position = targetPos;

        ClampInsideArea();
    }

    public void OnDragEnd()
    {
        _isDragging = false;
    }
    
    public void AddWindPushSmooth(Vector2 delta)
    {
        if (_windMoveRoutine != null)
        {
            StopCoroutine(_windMoveRoutine);
        }

        _windMoveRoutine = StartCoroutine(MoveByWind(delta));
    }

    private IEnumerator MoveByWind(Vector2 delta)
    {
        if (!_isDragging)
        {
            Vector2 start = transform.position;
            Vector2 end = start + delta;

            float t = 0f;

            while (t < 1f)
            {
                t += Time.deltaTime / windMoveDuration;
                Vector2 pos = Vector2.Lerp(start, end, t);
                transform.position = pos;
                ClampInsideArea();
                yield return null;
            }

            transform.position = end;
            ClampInsideArea();
        }
        else
        {
            Vector2 startOffset = _windOffset;
            Vector2 endOffset = startOffset + delta;

            float t = 0f;

            while (t < 1f)
            {
                t += Time.deltaTime / windMoveDuration;
                _windOffset = Vector2.Lerp(startOffset, endOffset, t);
                yield return null;
            }

            _windOffset = endOffset;
        }

        _windMoveRoutine = null;
    }

    private void ClampInsideArea()
    {
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
}
