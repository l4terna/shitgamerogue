using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class WeatherSwitcher : MonoBehaviour
{
    [SerializeField] private ParticleSystem snowfall;
    [SerializeField] private ParticleSystem rain;

    [SerializeField] public float snowfallDuration = 10f;
    
    [SerializeField] public float minDurationWithNoSnow = 30f;
    [SerializeField] public float maxDurationWithNoSnow = 120f;
    
    private SpriteRenderer _spriteRenderer;
        
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

                AudioManager.Instance.StopMusic("rain");
                AudioManager.Instance.PlayMusic("snow");

                yield return new WaitForSeconds(snowfallDuration);
            }
            else
            {
                snowfall.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                rain.Play();
                SetSnowColor(false);


                AudioManager.Instance.StopMusic("snow");
                AudioManager.Instance.PlayMusic("rain");

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


    public void StartSnow()
    {
        // Если корутина уже запущена, не запускаем повторно
        StopSnow(); // безопасно остановим текущую погоду
        StartCoroutine(WeatherSwitch());
    }

    public void StopSnow()
    {
        // Останавливаем снег
        if (snowfall != null && snowfall.isPlaying)
            snowfall.Stop(true, ParticleSystemStopBehavior.StopEmitting);

        // Включаем дождь
        if (rain != null && !rain.isPlaying)
            rain.Play();

        // Меняем цвет снега обратно
        SetSnowColor(false);

        // Останавливаем корутину безопасно
        StopAllCoroutines(); // если только эта корутина больше не нужна
    }
}