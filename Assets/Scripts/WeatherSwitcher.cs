using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class WeatherSwitcher : MonoBehaviour
{
    [SerializeField] private ParticleSystem snowfall;
    [SerializeField] private ParticleSystem rain;

    [SerializeField] private float snowfallDuration = 10f;
    
    [SerializeField] private float minDurationWithNoSnow = 30f;
    [SerializeField] private float maxDurationWithNoSnow = 120f;
    
    private SpriteRenderer _spriteRenderer;

    private int _originalLayer; 
        
    public WeatherType CurrentWeather
    {
        get
        {
            if (rain.isPlaying) return WeatherType.Rain;
            if (snowfall.isPlaying) return WeatherType.Snowfall;

            return WeatherType.None;
        }
    }

    public enum WeatherType
    {
        None,
        Rain,
        Snowfall
    }

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _originalLayer = gameObject.layer;
    }

    void Start()
    {
        rain.gameObject.SetActive(true);
        snowfall.gameObject.SetActive(true);

        rain.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        snowfall.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        
        StartCoroutine(WeatherSwitch());
    }

    private IEnumerator WeatherSwitch()
    {
        while (true)
        {
            if (rain.isPlaying)
            {
                rain.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                snowfall.Play();
                SetSnowColor(true);
                
                yield return new WaitForSeconds(snowfallDuration);
            }
            else
            {
                snowfall.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                rain.Play();
                SetSnowColor(false);
                
                float randomDelay = Random.Range(minDurationWithNoSnow, maxDurationWithNoSnow);
                Debug.Log("Rain delay: " + randomDelay);
                
                yield return new WaitForSeconds(randomDelay);
            }
        }
    }

    private void SetSnowColor(bool flag)
    {
        if (flag) _spriteRenderer.color = new Color(
            0f / 255f,
            112f / 255f,
            255f / 255f,
            255f / 255f
        );
        else _spriteRenderer.color = Color.white;
    }
    

    public void ForceSnowNow()
    {
        StopAllCoroutines();
        
        rain.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        snowfall.Play();
        
        StartCoroutine(WeatherSwitch());
    }
}