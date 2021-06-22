using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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

    Dictionary<int, AudioClip> audioClipDic = new Dictionary<int, AudioClip>();

    List<AudioSource> audioSources = new List<AudioSource>();

    private AudioSource bgmAudioSource;

    private void Awake()
    {
        instance = this;
        for (int i = 0; i < audioClips.Count; i++)
        {
            audioClipDic.Add(i, audioClips[i].audioClip);
        }
        bgmAudioSource = gameObject.AddComponent<AudioSource>();
    }

    public void SetBGM(int index)
    {
        bgmAudioSource.loop = true;
        bgmAudioSource.clip = bgms[index];
        bgmAudioSource.Play();
    }

    public void PlaySound(int index)
    {
        foreach (var item in audioSources)
        {
            if (!item.isPlaying)
            {
                item.clip = audioClipDic[index];
                item.Play();
                return;
            }
        }
        MakeNewAudioSource();
        PlaySound(index);
    }

    public void MakeNewAudioSource()
    {
        var temp = transform.gameObject.AddComponent<AudioSource>();
        audioSources.Add(temp);
    }

}
