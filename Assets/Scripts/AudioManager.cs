using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
    [Range(0f, 1f)]
    public float volume;
    [Range(0.1f, 3f)]
    public float pitch;
    public bool loop = false;
    [Range(0f, 1f)]
    public float spatialBlend;

    [HideInInspector]
    public AudioSource source;
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public Sound[] effects;
    public Sound[] musics;

    private Dictionary<string, Sound> effectsDictionary = new Dictionary<string, Sound>();
    private Dictionary<string, Sound> musicDictionary = new Dictionary<string, Sound>();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        InitializeSounds(effects, effectsDictionary);
        InitializeSounds(musics, musicDictionary);
    }

    private void InitializeSounds(Sound[] soundsArray, Dictionary<string, Sound> soundDict)
    {
        foreach (Sound s in soundsArray)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
            s.source.spatialBlend = s.spatialBlend;
            soundDict[s.name] = s;
        }
    }

    public void PlayEffectAtPoint(string name, Vector3 position)
    {
        if (effectsDictionary.TryGetValue(name, out Sound s))
        {
            AudioSource.PlayClipAtPoint(s.clip, position, s.volume);
        }
        else
        {
            Debug.LogWarning($"Sound effect named {name} not found!");
        }
    }

    public void PlayMusic(string name)
    {
        if (musicDictionary.TryGetValue(name, out Sound s) && !s.source.isPlaying)
        {
            s.source.Play();
        }
        else if (s == null)
        {
            Debug.LogWarning($"Music named {name} not found!");
        }
    }

    public void PlayEffect(string name)
    {
        if (effectsDictionary.TryGetValue(name, out Sound s) && !s.source.isPlaying)
        {
            s.source.Play();
        }
        else if (s == null)
        {
            Debug.LogWarning($"Music named {name} not found!");
        }
    }

    public void StopMusic(string name)
    {
        if (musicDictionary.TryGetValue(name, out Sound s))
        {
            s.source.Stop();
        }
    }

    public void SetMusicVolume(float volume)
    {
        foreach (var s in musicDictionary.Values)
        {
            s.source.volume = volume;
        }
    }

    public void SetEffectsVolume(float volume)
    {
        foreach (var s in effectsDictionary.Values)
        {
            s.source.volume = volume;
        }
    }
}