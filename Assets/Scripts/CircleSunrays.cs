using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.TextCore.Text;

[RequireComponent(typeof(LineRenderer))]
public class CircleSunrays : MonoBehaviour
{
    [SerializeField] public float temperatureIncreasePoints = 1f;

    [SerializeField] public int raysCount = 36;
    [SerializeField] private float maxDistance = 10f;
    [SerializeField] private LayerMask obstacleMask;
    [SerializeField] private LayerMask flowerMask;

    [SerializeField] private float fullStartWidth = 0.6f;
    [SerializeField] private float fullEndWidth = 0.3f;

    [ColorUsage(true, true)]
    [SerializeField] private Color rayColor = new Color(1f, 0.92156863f, 0.015686275f, 0.4f);

    private LineRenderer[] _rays;
    private LineRenderer _template;

    public LineRenderer[] Rays => _rays;

    void Awake()
    {
        _template = GetComponent<LineRenderer>();

        _template.useWorldSpace = true;
        _template.loop = false;
        _template.startWidth = fullStartWidth;
        _template.endWidth = fullEndWidth;
        _template.positionCount = 2;

        RebuildRays();

        _template.enabled = false;
    }

    public void RebuildRays()
    {
        if (_rays != null)
        {
            for (int i = 0; i < _rays.Length; i++)
            {
                if (_rays[i] != null)
                    Destroy(_rays[i].gameObject);
            }
        }

        _rays = new LineRenderer[raysCount];

        for (int i = 0; i < raysCount; i++)
        {
            GameObject rayObj = new GameObject($"SunRay{i}");
            rayObj.transform.SetParent(transform, false);

            LineRenderer lr = rayObj.AddComponent<LineRenderer>();

            lr.useWorldSpace = _template.useWorldSpace;
            lr.loop = _template.loop;
            lr.material = new Material(Shader.Find("Sprites/Default"));
            lr.material.color = rayColor;

            lr.textureMode = _template.textureMode;
            lr.numCapVertices = _template.numCapVertices;
            lr.numCornerVertices = _template.numCornerVertices;
            lr.startColor = _template.startColor;
            lr.endColor = _template.endColor;

            lr.startWidth = fullStartWidth;
            lr.endWidth = fullEndWidth;

            lr.positionCount = 2;
            lr.sortingOrder = _template.sortingOrder;

            _rays[i] = lr;
        }
    }

    private void Update()
    {
        Vector2 origin = transform.position;

        for (int i = 0; i < raysCount; i++)
        {
            float angle = 360f / raysCount * i;
            float rad = angle * Mathf.Deg2Rad;

            Vector2 dir = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));


            int mask = obstacleMask | flowerMask;
            RaycastHit2D hit = Physics2D.Raycast(origin, dir, maxDistance, mask);

            Vector3 start = origin;

            if (hit.collider && hit.collider.gameObject.CompareTag("Flower"))
            {
                Flower flower = hit.collider.gameObject.GetComponentInParent<Flower>();

                flower?.AddTemperature(temperatureIncreasePoints);
            }

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

    public void SetRaysCount(int newCount)
    {
        newCount = Mathf.Max(1, newCount);
        if (newCount == raysCount)
            return;

        raysCount = newCount;
        RebuildRays();
    }

}
