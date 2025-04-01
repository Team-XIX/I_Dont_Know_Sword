using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class AudioManager : SingleTon<AudioManager>
{
    public AudioSource BGMaudioSource;

    public List<AudioSource> SFXaudioSources = new List<AudioSource>();
    public int MaxSFXaudioSources = 10;

    public List<AudioClip> bgmClips;
    public List<AudioClip> sfxClips;

    public AudioMixer audioMixer;
    public AudioMixerGroup sfxMixerGroup;
    public AudioMixerGroup bgmMixerGroup;

    private void Awake()
    {
        DontDestroyOnLoad(this);
        if (BGMaudioSource == null)
            BGMaudioSource = gameObject.AddComponent<AudioSource>();

        for (int i = 0; i < MaxSFXaudioSources; i++)
        {
            AudioSource source = gameObject.AddComponent<AudioSource>();
            SFXaudioSources.Add(source);
        }

        SceneManager.sceneLoaded += PlayBGMOnSceneChange; // 인덱스 순서대로 저장되어있는 클립 재생
        LoadVolume();
    }

    private void PlayBGMOnSceneChange(Scene scene, LoadSceneMode mode)
    {
        int sceneIndex = scene.buildIndex;

        if (sceneIndex >= 0 && sceneIndex < bgmClips.Count) // 씬 인덱스가 리스트 범위 안에 있는지 확인 빌드 세팅 확인
        {
            PlayBGM(bgmClips[sceneIndex]);
        }
    }
    public void PlayBGM(AudioClip clip)
    {
        if (BGMaudioSource.clip == clip && BGMaudioSource.isPlaying)
            return;

        if (clip != null && BGMaudioSource != null)
        {
            BGMaudioSource.clip = clip;
            BGMaudioSource.outputAudioMixerGroup = bgmMixerGroup; // 믹서 그룹을 통해 BGM볼륨 조절
            BGMaudioSource.loop = true;
            BGMaudioSource.Play();
        }
    }

    public void StopBGM()
    {
        if (BGMaudioSource != null)
        {
            BGMaudioSource.Stop();
        }
    }

    public void PlaySFX(AudioClip clip)
    {
        AudioSource SFXaudioSource = GetSFXSource();

        if (clip != null && SFXaudioSource != null)
        {
            SFXaudioSource.outputAudioMixerGroup = sfxMixerGroup;
            SFXaudioSource.PlayOneShot(clip);
        }
    }

    private AudioSource GetSFXSource()
    {
        for (int i = 0; i < SFXaudioSources.Count; i++)
        {
            if (!SFXaudioSources[i].isPlaying)
                return SFXaudioSources[i];
        }
        return SFXaudioSources[0];
    }

    public void SetBGMVolume(float volume)
    {
        audioMixer.SetFloat("BGM", Mathf.Log10(volume) * 20); // 데시벨 공식을 적용하여, 믹서를 통하여 볼륨 전달
    }

    public void SetSFXVolume(float volume)
    {
        audioMixer.SetFloat("SFX", Mathf.Log10(volume) * 20);
    }

    private void LoadVolume()
    {
        float bgmVolume, sfxVolume;

        if (audioMixer.GetFloat("BGM", out bgmVolume)) ;
        if (audioMixer.GetFloat("SFX", out sfxVolume)) ;
    }
}
