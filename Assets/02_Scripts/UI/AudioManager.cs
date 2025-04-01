using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public AudioSource BGMaudioSource;
    public AudioSource SFXaudioSource;

    public List <AudioClip> bgmClips;
    public List <AudioClip> sfxClips;

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
            return;
        }
    }

    public void PlayBGM(AudioClip clip)
    {
        if(clip != null && BGMaudioSource != null)
        {
            BGMaudioSource.PlayOneShot(clip);
        }
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip != null && SFXaudioSource != null)
        {
            SFXaudioSource.PlayOneShot(clip);
        }
    }
}
