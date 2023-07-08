using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : PersistenSingleton<AudioManager>
{
    [SerializeField] AudioSource sFXPlayer;

    const float MIN_PITCH = 0.9f;
    const float MAX_PITCH = 1.1f;

    // Used for UI SFX

    public void PlayerSFX(AudioData audioData)
    {
        sFXPlayer.PlayOneShot(audioData.audioClip, audioData.volume); // 这个函数不会覆盖掉正在播放得音效
    }

    // Used for repeat-play SFX

    public void PlayRandomSFX(AudioData audioData)
    {
        sFXPlayer.pitch = Random.Range(MIN_PITCH, MAX_PITCH);
        PlayerSFX(audioData);
    }

    public void PlayRandomSFX(AudioData[] audioDatas)
    {
        PlayRandomSFX(audioDatas[Random.Range(0, audioDatas.Length)]);
    }
}

// System.Serializable 这个类得共有变量才能被序列化暴露到编辑器中
[System.Serializable] public class AudioData
{
    public AudioClip audioClip;
    public float volume;
}
