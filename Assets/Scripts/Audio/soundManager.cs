using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class soundManager : SingletonMonobehaviour<soundManager>
{
    [Header("文件管理与播放器")]
    public Sound[] musicSound, sfxSounds;
    public AudioSource musicSource, sfxSource,textSource;
    public bool is_update_text=false;
    public float totalVolume=1f;
    

    private Coroutine fadeCoroutine; // 用于控制渐强的协程
    public void setTotalVolume(float volume) 
    {
        totalVolume = volume;
    }
    public void setMusicVolume(float volume)
    {
        musicSource.volume = volume*totalVolume;
        
    }
    public void setSfxVolume(float volume)
    {
        sfxSource.volume = volume*totalVolume;

    }

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
    public void PlayMusic(AudioClip music,bool loop=true)
    {   
        musicSource.loop = loop;
        musicSource.clip = music;
        musicSource.Play();
        
    }
    //渐强播放音乐
    public void PlayMusicInFade(AudioClip music, bool loop=true, bool volume_is_up=true)
    {
        musicSource.loop = loop;
        musicSource.clip = music;
        musicSource.Play();
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
    public void playTextSound(bool is_use,string soundName="textSound")
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
    
    //可调用的渐强渐弱音量控制函数
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
        playTextSound(is_update_text);  

    }

}
