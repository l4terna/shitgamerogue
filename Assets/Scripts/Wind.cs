using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class Wind : MonoBehaviour
{
    [SerializeField] public float repulsion = 0.1f;
    [SerializeField] public float windDuration = 3f;
    
    [SerializeField] public float minDurationWithNoWind = 30f;
    [SerializeField] public float maxDurationWithNoWind = 120f;

    [SerializeField] private Transform spawnPoint1;
    [SerializeField] private Transform spawnPoint2;


    [SerializeField] private CloudDragging cloudDragging;

    private ParticleSystem _ps;
    private ParticleSystem.VelocityOverLifetimeModule  _velocityOverLifetime;
    private Vector2 _windDirection; // < 0.5 - to left
    
    void Awake()
    {
        _ps = GetComponent<ParticleSystem>();
        
        _velocityOverLifetime = _ps.velocityOverLifetime;
        _velocityOverLifetime.enabled = true; 
    }

    private void OnEnable()
    {
        // Останавливаем, очищаем систему и запускаем корутину для включения ветра
        _ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        StartCoroutine(WindSwitch(startWithWind: false));
    }

    void OnParticleCollision(GameObject other)
    {
        if (other.CompareTag("Cloud") && cloudDragging)
        {
            cloudDragging.AddWindPushSmooth(_windDirection * repulsion);
        }
    }

    private IEnumerator WindSwitch(bool startWithWind)
    {
        bool windOn = startWithWind;

        while (true)
        {
            if (windOn)
            {
                if (Random.value >= 0.5f)
                {
                    transform.position = spawnPoint1.position;
                    _velocityOverLifetime.x = new ParticleSystem.MinMaxCurve(1f, 4f);
                    _windDirection = Vector2.right;
                }
                else
                {
                    transform.position = spawnPoint2.position;
                    _velocityOverLifetime.x = new ParticleSystem.MinMaxCurve(-1f, -4f);
                    _windDirection = Vector2.left;
                }
                
                _ps.Play();
                AudioManager.Instance.PlayMusic("wind");
                yield return new WaitForSeconds(windDuration);

                _ps.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                windOn = false;

                yield return new WaitForSeconds(3.5f);
                AudioManager.Instance.StopMusic("wind");
            }
            else
            {
                float delay = Random.Range(minDurationWithNoWind, maxDurationWithNoWind);
                Debug.Log("Wind delay: " + delay);
                yield return new WaitForSeconds(delay);

                windOn = true;
            }
        }
    }

    public void StartWind()
    {
        StopAllCoroutines();
        _ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        StartCoroutine(WindSwitch(startWithWind: true));
    }
}