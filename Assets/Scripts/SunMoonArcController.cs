using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class SunMoonArcController : MonoBehaviour
{
    [Header("Sun")]
    [SerializeField] private Transform sun;
    [SerializeField] private CircleSunrays sunRays;
    [SerializeField] private float sunRadius = 10f;
    [SerializeField] private float sunStartAngle = 0f;    
    [SerializeField] private float sunEndAngle = 180f;    

    [Header("Moon")]
    [SerializeField] private Transform moon;
    [SerializeField] private CircleSunrays moonRays;
    [SerializeField] private float moonRadius = 10f;
    [SerializeField] private float moonStartAngle = 180f; 
    [SerializeField] private float moonEndAngle = 0f;     

    [Header("Time")]
    [SerializeField] private float dayDuration = 10f;   
    [SerializeField] private float nightDuration = 10f; 

    [Range(0f, 0.5f)]
    [SerializeField] private float fadePortion = 0.2f;  

    [Header("Global Light 2D")]
    [SerializeField] private Light2D globalLight;
    [SerializeField] private float daySunIntensity = 1f; 
    [SerializeField] private float nightSunIntensity = 0.1f;

    [Header("Cycle starting")]
    [SerializeField] private bool startAtNight = false;

    private void Awake()
    {
        if (!globalLight)
            globalLight = FindAnyObjectByType<Light2D>();
    }

    private void Start()
    {
        SetRaysAlpha(sunRays, 0f);
        SetRaysAlpha(moonRays, 0f);

        if (startAtNight)
        {
            StartCoroutine(NightThenDayLoop());
        }
        else
        {
            StartCoroutine(DayThenNightLoop());
        }
    }

    private IEnumerator DayThenNightLoop()
    {
        while (true)
        {
            yield return StartCoroutine(PlaySunPhase());
            yield return StartCoroutine(PlayMoonPhase());
        }
    }

    private IEnumerator NightThenDayLoop()
    {
        while (true)
        {
            yield return StartCoroutine(PlayMoonPhase());
            yield return StartCoroutine(PlaySunPhase());
        }
    }

    private IEnumerator PlaySunPhase()
    {
        if (!sun || !sunRays)
            yield break;

        SetRaysAlpha(moonRays, 0f);

        MoveBodyToAngle(sun, sunRadius, sunStartAngle);

        float elapsed = 0f;

        while (elapsed < dayDuration)
        {
            float t = elapsed / dayDuration; 

            float angle = Mathf.Lerp(sunStartAngle, sunEndAngle, t);
            MoveBodyToAngle(sun, sunRadius, angle);

            float alpha = ComputeFadeAlpha(t);
            SetRaysAlpha(sunRays, alpha);

            float lightFactor = ComputeFadeAlpha(t);
            ApplyLighting(lightFactor); 

            elapsed += Time.deltaTime;
            yield return null;
        }

        MoveBodyToAngle(sun, sunRadius, sunEndAngle);
        SetRaysAlpha(sunRays, 0f);
        ApplyLighting(0f);
    }

    private IEnumerator PlayMoonPhase()
    {
        if (!moon || !moonRays)
            yield break;

        SetRaysAlpha(sunRays, 0f);

        MoveBodyToAngle(moon, moonRadius, moonStartAngle);

        float elapsed = 0f;

        while (elapsed < nightDuration)
        {
            float t = elapsed / nightDuration; 

            float angle = Mathf.Lerp(moonStartAngle, moonEndAngle, t);
            MoveBodyToAngle(moon, moonRadius, angle);

            float alpha = ComputeFadeAlpha(t);
            SetRaysAlpha(moonRays, alpha);

            float moonLightFactor = alpha * 0.4f; 
            ApplyLighting(moonLightFactor);

            elapsed += Time.deltaTime;
            yield return null;
        }

        MoveBodyToAngle(moon, moonRadius, moonEndAngle);
        SetRaysAlpha(moonRays, 0f);
        ApplyLighting(0f);
    }


    private float ComputeFadeAlpha(float t)
    {

        if (t < fadePortion)
        {
            return Mathf.InverseLerp(0f, fadePortion, t);
        }

        if (t > 1f - fadePortion)
        {
            return Mathf.InverseLerp(1f, 1f - fadePortion, t); 
        }

        return 1f;
    }

    private void MoveBodyToAngle(Transform body, float radius, float angleDeg)
    {
        if (!body) return;

        float rad = angleDeg * Mathf.Deg2Rad;

        Vector2 offset = new Vector2(
            Mathf.Cos(rad),
            Mathf.Sin(rad)
        ) * radius;

        body.position = (Vector2)transform.position + offset; 
    }

    private void SetRaysAlpha(CircleSunrays rays, float alpha)
    {
        if (!rays) return;

        LineRenderer[] lrArray = rays.Rays;
        if (lrArray == null) return;

        foreach (var lr in lrArray)
        {
            if (!lr) continue;

            var c1 = lr.startColor;
            var c2 = lr.endColor;

            c1.a = alpha;
            c2.a = alpha;

            lr.startColor = c1;
            lr.endColor = c2;
        }
    }

    private void ApplyLighting(float factor)
    {
        if (!globalLight) return;
        
        globalLight.intensity = Mathf.Lerp(nightSunIntensity, daySunIntensity, factor);
    }
}
