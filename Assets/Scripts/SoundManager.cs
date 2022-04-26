using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;      // Instance of the Sound Manager
    public float globalVolume = 1;
    void Awake()
    {
        // Set the singleton instance
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        ClearEmptySources();

        GameObject volumeBar = GameObject.Find("Volume");

        if (volumeBar != null)
        {
            globalVolume = volumeBar.GetComponent<Slider>().value;
        }


        AudioSource[] sources = GetComponents<AudioSource>();

        // Return if no audio sources
        if (sources.Length == 0)
        {
            return;
        }

        // Delete all sources that are not playing
        foreach (AudioSource s in sources)
        {
            if (s.clip.name == "whitenoise")
            {
                s.volume = 0.25f * globalVolume;
            }
        }
    }

    /// <summary>
    /// Checks for all the audio sources in the GameObject and clears them.
    /// </summary>
    void ClearEmptySources()
    {
        AudioSource[] sources = GetComponents<AudioSource>();

        // Return if no audio sources
        if (sources.Length == 0)
        {
            return;
        }

        // Delete all sources that are not playing
        foreach (AudioSource s in sources)
        {
            if (!s.isPlaying)
            {
                Destroy(s);
            }
        }
    }

    /// <summary>
    /// Creates an audio source and plays the clip with the specified volume, pitch and loop
    /// </summary>
    /// <param name="clip"></param>
    /// <param name="volume"></param>
    /// <param name="pitch"></param>
    /// <param name="loop"></param>
    public void PlaySound(string clip, float volume, float pitch, bool loop, bool music)
    {
        AudioSource source = gameObject.AddComponent<AudioSource>();

        string path = "SFX/";
        if (music) { path = "Music/"; }

        AudioClip c = Resources.Load(path + clip) as AudioClip;
        source.clip = c;
        if (music)
        {
            source.volume = volume; //* PlayerPrefs.GetFloat("MusicVolume");
        }
        else
        {
            source.volume = volume * globalVolume; //* PlayerPrefs.GetFloat("SoundVolume");
        }
        source.pitch = pitch;
        source.loop = loop;
        source.Play();

        print("Playing sound (" + clip + ")");
    }

    public void PlaySound(string clip)
    {
        PlaySound(clip, 1, 1, false, false);
    }

    /// <summary>
    /// Stops all sounds with the specified clip playing.
    /// </summary>
    /// <param name="clip"></param>
    public void StopSound(string clip)
    {
        AudioSource[] sources = GetComponents<AudioSource>();

        // Return if no audio sources
        if (sources.Length == 0)
        {
            return;
        }

        // Delete all sources that are not playing
        foreach (AudioSource s in sources)
        {
            if (s.clip.name == clip)
            {
                Destroy(s);
            }
        }
    }

    /// <summary>
    /// Stops every sound clip playing.
    /// </summary>
    public void StopAllSounds()
    {
        AudioSource[] sources = GetComponents<AudioSource>();

        // Return if no audio sources
        if (sources.Length == 0)
        {
            return;
        }

        // Delete all sources that are not playing
        foreach (AudioSource s in sources)
        {
            Destroy(s);
        }
    }

    /// <summary>
    /// Set the global volume of the sounds playing in percentage values (0 = 0%, 0.5 = 50%, ect).
    /// </summary>
    public void SetGlobalVolume(float volume)
    {
        AudioSource[] sources = GetComponents<AudioSource>();

        // Return if no audio sources
        if (sources.Length == 0)
        {
            return;
        }

        // Set the volume of all sounds playing
        foreach (AudioSource s in sources)
        {
            s.volume *= volume;
        }
    }
}