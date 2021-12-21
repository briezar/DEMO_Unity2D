using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [SerializeField] private AudioSource musicSource, effectsSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void StopBGM()
    {
        musicSource.Stop();
    }

    private AudioClip lastBGM;

    public void PlayBGM(AudioClip clip)
    {
        lastBGM = musicSource.clip;
        musicSource.clip = clip;
        musicSource.Play();
    }

    public void PlayLastBGM()
    {
        musicSource.clip = lastBGM;
        musicSource.Play();
    }
    public void PlaySFX(AudioClip clip)
    {
        effectsSource.PlayOneShot(clip);
    }

    public void ChangeBGMVolume(float value)
    {
        musicSource.volume = value;
    }

    public void ChangeSFXVolume(float value)
    {
        effectsSource.volume = value;
    }
    public void ChangeMasterVolume(float value)
    {
        AudioListener.volume = value;
    }
}
