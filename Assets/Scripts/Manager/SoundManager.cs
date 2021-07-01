using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

[Serializable]
public struct AudioStruct
{
    public int index;
    public AudioClip audioClip;
}

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    public AudioClip[] bgms;

    public List<AudioStruct> audioClips = new List<AudioStruct>();

    private readonly Dictionary<int, AudioClip> audioClipDic = new Dictionary<int, AudioClip>();

    private readonly List<AudioSource> audioSources = new List<AudioSource>();

    private AudioSource bgmAudioSource;

    private void Awake()
    {
        instance = this;
        for (int i = 0; i < audioClips.Count; i++)
        {
            audioClipDic.Add(i, audioClips[i].audioClip);
        }
        bgmAudioSource = gameObject.AddComponent<AudioSource>();
        DontDestroyOnLoad(this);
    }

    public void SetBGM(int index)
    {
        if (!GameManager.instance.sound) return;

        bgmAudioSource.loop = true;
        bgmAudioSource.clip = bgms[index];
        bgmAudioSource.Play();
    }

    public void PlaySound(int index)
    {
        if (!GameManager.instance.sound) return;

        foreach (var item in audioSources.Where(item => !item.isPlaying))
        {
            item.clip = audioClipDic[index];
            item.Play();
            return;
        }
        MakeNewAudioSource();
        PlaySound(index);

    }

    private void MakeNewAudioSource()
    {
        var temp = transform.gameObject.AddComponent<AudioSource>();
        audioSources.Add(temp);
    }

    public void OnChangeVolume()
    {
        if (GameManager.instance.sound)
        {
            bgmAudioSource.Play();
        }
        else
        {
            bgmAudioSource.Stop();
        }
    }

}
