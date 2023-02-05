using System;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    
    public Sound[] soundEffects;
    public Sound[] backgroundMusics;

    [HideInInspector] public List<AudioSource> sfxList = new List<AudioSource>();
    [HideInInspector] public List<AudioSource> bgmList = new List<AudioSource>();
    public bool isOnMic;

    #region SETUP

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        if (Instance == null)
        {
            Instance = this;
            SetupAudio();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetupAudio()
    {
        for (int i = 0; i < soundEffects.Length; i++)
        {
            soundEffects[i].audioSource = gameObject.AddComponent<AudioSource>();
            soundEffects[i].audioSource.clip = soundEffects[i].clip;
            soundEffects[i].audioSource.volume = soundEffects[i].baseVolume;
            soundEffects[i].audioSource.pitch = soundEffects[i].basePitch;
            soundEffects[i].audioSource.loop = soundEffects[i].loop;
            sfxList.Add(soundEffects[i].audioSource);
        }

        for (int i = 0; i < backgroundMusics.Length; i++)
        {
            backgroundMusics[i].audioSource = gameObject.AddComponent<AudioSource>();
            backgroundMusics[i].audioSource.clip = backgroundMusics[i].clip;
            backgroundMusics[i].audioSource.volume = backgroundMusics[i].baseVolume;
            backgroundMusics[i].audioSource.pitch = backgroundMusics[i].basePitch;
            backgroundMusics[i].audioSource.loop = backgroundMusics[i].loop;
            bgmList.Add(backgroundMusics[i].audioSource);
        }
    }

    /// <summary>
    /// Set sfx and bgm volume
    /// </summary>
    /// <param name="sfxVolume">Range 0.0f - 1.0f</param>
    /// <param name="bgmVolume">Range 0.0f - 1.0f</param>
    public void SetMasterVolume(float sfxVolume, float bgmVolume)
    {
        for (int i = 0; i < sfxList.Count; i++)
        {
            sfxList[i].volume = sfxVolume;
        }

        for (int i = 0; i < bgmList.Count; i++)
        {
            bgmList[i].volume = bgmVolume;
        }
    }

    /// <param name="sfxVolume">Range 0.0f - 1.0f</param>
    public void SetSfxVolume(float sfxVolume)
    {
        for (int i = 0; i < sfxList.Count; i++)
        {
            sfxList[i].volume = sfxVolume;
        }
    }

    /// <param name="bgmVolume">Range 0.0f - 1.0f</param>
    public void SetBgmVolume(float bgmVolume)
    {
        for (int i = 0; i < bgmList.Count; i++)
        {
            bgmList[i].volume = bgmVolume;
        }
    }

    #endregion SETUP

    #region AUDIO PLAYER

    /// <param name="name">Name/title of SFX, should be at active list</param>
    public void PlaySfx(string name)
    {
        Sound sfx = Array.Find(soundEffects, sound => sound.name == name);
        if (sfx == null)
        {
            Debug.LogWarning("Audio " + name + " not found!!");
            return;
        }

        sfx.audioSource.Play();
    }

    /// <param name="name">Name/title of BGM, should be at active list</param>
    public void PlayBgm(string name)
    {
        Sound bgm = Array.Find(backgroundMusics, sound => sound.name == name);
        if (bgm == null)
        {
            Debug.LogWarning("Audio " + name + " not found!!");
            return;
        }

        for (int i = 0; i < backgroundMusics.Length; i++)
        {
            backgroundMusics[i].audioSource.Stop();
        }

        bgm.audioSource.Play();
    }

    public void StopBgm()
    {
        for (int i = 0; i < backgroundMusics.Length; i++)
        {
            backgroundMusics[i].audioSource.Stop();
        }
    }
    
    #endregion AUDIO PLAYER
    
}

[Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
    [Range(0f, 1f)] public float baseVolume = 1;
    [Range(0.1f, 3f)] public float basePitch = 1;
    public bool loop;
    [HideInInspector] public AudioSource audioSource;
}