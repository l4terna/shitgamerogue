using System.Collections;
using UnityEngine;

public class WeatherSwitcher : MonoBehaviour
{
    [SerializeField] private GameObject snowfall;
    [SerializeField] private GameObject rain;

    void Start()
    {
        SetRain();
        StartCoroutine(WeatherSwitch());
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private IEnumerator WeatherSwitch()
    {
        while (true)
        {
            yield return new WaitForSeconds(5f);

            if (rain.activeSelf)
            {
                SetSnowfall();
            }
            else SetRain();   
        }
    }

    private void SetSnowfall()
    {
        snowfall.SetActive(true);
        Invoke(nameof(DisableRain), 1f);
    }
    

    private void SetRain()
    {
        rain.SetActive(true);
        Invoke(nameof(DisableSnowfall), 1f);
    }

    private void DisableRain()
    {
        rain.SetActive(false);
    }
    
    private void DisableSnowfall()
    {
        snowfall.SetActive(false);
    }
}
