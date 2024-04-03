using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class soundManager : SingletonMonobehaviour<soundManager>
{
    [Header("文件管理与播放器")]
    public Sound[] musicSound, sfxSounds;
    public AudioSource musicSource, sfxSource;
    
    public void PlayMusic(string name)
    {
        Debug.Log("play");
        Sound s = Array.Find(musicSound, x => x.name == name);
        if (s == null)
        {
            Debug.Log("Not found");
        }
        else
        {
            musicSource.clip = s.clip;
            musicSource.Play();
        }
    }
    public void PlaySFX(string name)
    {
        Sound s = Array.Find(sfxSounds, x => x.name == name);
        if (s == null)
        {
            Debug.Log("Not found");
        }
        else
        {
            sfxSource.PlayOneShot(s.clip);
        }
    }
    public void PlaySFX(AudioClip SE)
    {
        sfxSource.PlayOneShot(SE);
    }
    public void PlayMusic(AudioClip music)
    {
        musicSource.clip = music;
        musicSource.Play();
    }
    public void StopMusic()
    {
        musicSource.Stop();
    }
}
