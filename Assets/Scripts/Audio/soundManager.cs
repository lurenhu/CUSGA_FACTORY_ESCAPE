using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class soundManager : SingletonMonobehaviour<soundManager>
{
    [Header("文件管理与播放器")]
    public Sound[] musicSound, sfxSounds;
    public AudioSource musicSource, sfxSource;
    public bool is_update_text = false;
    [Header("音量控制")]
    public float totalVolume=1f;

    protected override void Awake()
    {
        base.Awake();

        PlayMusic("Theme");
    
        DontDestroyOnLoad(this);
    }

    private Coroutine fadeCoroutine; // 用于控制渐强的协程
    //设置总音量
    public void setTotalVolume(float volume) 
    {
        totalVolume = volume;
    }
    //分项音量由比例计算得出
    public void setMusicVolume(float volume)
    {
        musicSource.volume = volume*totalVolume;
        
    }
    public void setSfxVolume(float volume)
    {
        sfxSource.volume = volume*totalVolume;

    }

    public void PlayMusic(string name,bool loop = true)
    {
        Debug.Log("play");
        Sound s = Array.Find(musicSound, x => x.name == name);
        if (s == null)
        {
            Debug.Log("Not found");
        }
        else
        {
            musicSource.loop = loop;
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

    public void PlayMusic(AudioClip music,bool loop=true)
    {   
        musicSource.loop = loop;
        musicSource.clip = music;
        musicSource.Play();
        
    }

    //渐强播放音乐
    public void PlayMusicInFade(AudioClip music, bool loop=true, bool volume_is_up=true)
    {
        PlayMusic(music);
        FadeVolume(volume_is_up);
    }

    public void PlayMusicInFade(string name, bool loop = true, bool volume_is_up = true)
    {
        PlayMusic(name);
        FadeVolume(volume_is_up);
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    //渐弱停止音乐
    public void StopMusicInFade(bool volume_is_up = false)
    {
        FadeVolume(volume_is_up);
        //musicSource.Stop();
    }

    public void PlayTextSound(bool is_use,string soundName="textSound")
    {
        if (is_use)
        {
            Sound s = Array.Find(sfxSounds, x => x.name == soundName);
            if (s == null)
            {
                PlaySFX(soundName);
            }
        }
        
    }
    
    /// <summary>
    /// 可调用的渐强渐弱音量控制函数
    /// </summary>
    private void FadeVolume(bool volume_is_up)
    {
        float fadeDuration, startVolume, targetVolume;
        if (volume_is_up)
        {
             fadeDuration = 2.0f; // 渐强的持续时间
             startVolume = 0.0f;
             targetVolume = 1.0f;
        }
        else 
        {
             fadeDuration = 2.0f; // 渐强的持续时间
             startVolume = 1.0f;
             targetVolume = 0.0f;
        }
        StopFadeCoroutine();
        fadeCoroutine = StartCoroutine(FadeInCoroutine(fadeDuration,startVolume,targetVolume));
    }

    private IEnumerator FadeInCoroutine(float fadeDuration,float startVolume,float targetVolume)
    {

        float elapsedTime = 0.0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float volume = Mathf.Lerp(startVolume, targetVolume, elapsedTime / fadeDuration);
            volume=musicSource.volume*volume;
            musicSource.volume=volume;
            yield return null;
        }

        musicSource.volume = targetVolume; // 确保最终音量为目标音量
    }

    private void StopFadeCoroutine()
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
            fadeCoroutine = null;
        }
    }
    private void Start()
    {
        //PlayMusicInFade("textOccurSFX");
    }
    private void Update()
    {
        //文字渲染时的音效
        PlayTextSound(is_update_text);  

    }

}
