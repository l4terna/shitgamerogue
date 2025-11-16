using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class Wind : MonoBehaviour
{
    [SerializeField] private float repulsion = 0.1f;
    [SerializeField] private float windDuration = 3f;
    
    [SerializeField] private float minDurationWithNoWind = 30f;
    [SerializeField] private float maxDurationWithNoWind = 120f;

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

    void Start()
    {
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
                    _velocityOverLifetime.x = new ParticleSystem.MinMaxCurve(3f);
                    _windDirection = Vector2.right;
                }
                else
                {
                    _velocityOverLifetime.x = new ParticleSystem.MinMaxCurve(-3f);
                    _windDirection = Vector2.left;
                }
                
                _ps.Play();
                yield return new WaitForSeconds(windDuration);

                _ps.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                windOn = false;
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