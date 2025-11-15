using UnityEngine;
using UnityEngine.TextCore.Text;

[RequireComponent(typeof(LineRenderer))]
public class CircleSunrays : MonoBehaviour
{
    [SerializeField] private int raysCount = 36;
    [SerializeField] private float maxDistance = 10f;
    [SerializeField] private LayerMask obstacleMask;
    
    [SerializeField] private float fullStartWidth = 0.6f;   
    [SerializeField] private float fullEndWidth = 0.3f;   
    
    [ColorUsage(true, true)]
    [SerializeField] private Color rayColor = new Color(1f, 0.92156863f, 0.015686275f, 0.4f);

    private LineRenderer[] _rays;
    private LineRenderer _template;
    
    public LineRenderer[] Rays => _rays;

    private void Awake()
    {
        _template = GetComponent<LineRenderer>();

        _template.useWorldSpace = true;
        _template.loop = false;
        _template.startWidth = fullStartWidth;
        _template.endWidth = fullEndWidth;
        _template.positionCount = 2;

        _rays = new LineRenderer[raysCount];

        for (int i = 0; i < raysCount; i++)
        {
            GameObject rayObj = new GameObject($"SunRay_{i}");
            rayObj.transform.SetParent(transform, false);

            LineRenderer lr = rayObj.AddComponent<LineRenderer>();

            lr.useWorldSpace = _template.useWorldSpace;
            lr.loop = _template.loop;
            lr.material = _template.material;
            lr.textureMode = _template.textureMode;
            lr.numCapVertices = _template.numCapVertices;
            lr.numCornerVertices = _template.numCornerVertices;
            lr.endColor =  _template.endColor;
            lr.startColor =  _template.startColor;
            lr.positionCount = 2;
            lr.sortingOrder = _template.sortingOrder;
            lr.material = new Material(Shader.Find("Sprites/Default"));
            lr.material.color = rayColor;

            _rays[i] = lr;
        }

        _template.enabled = false;
    }

    private void Update()
    {
        Vector2 origin = transform.position;

        for (int i = 0; i < raysCount; i++)
        {
            float angle = 360f / raysCount * i;
            float rad = angle * Mathf.Deg2Rad;

            Vector2 dir = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));

            RaycastHit2D hit = Physics2D.Raycast(origin, dir, maxDistance, obstacleMask);

            Vector3 start = origin;
            Vector3 end = hit.collider
                ? (Vector3)hit.point
                : (Vector3)(origin + dir * maxDistance);

            LineRenderer lr = _rays[i];

            lr.SetPosition(0, start);
            lr.SetPosition(1, end);

            float distance = Vector3.Distance(start, end);
            float t = Mathf.Clamp01(distance / maxDistance);

            lr.startWidth = fullStartWidth;
            lr.endWidth = Mathf.Lerp(fullStartWidth, fullEndWidth, t);
        }
    }
    
}
