using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SoundOptions : MonoBehaviour
{
    public AudioMixer audioMixer;
    public Slider Bgmslider;
    public Slider Sfxslider;

    private void Start()
    {
        LoadMixerVolume(); 

        Bgmslider.onValueChanged.AddListener(SetBgmVolume);
        Sfxslider.onValueChanged.AddListener(SetSfxVolume);
    }

    private void LoadMixerVolume()
    {
        float bgmVolume, sfxVolume;

        if (audioMixer.GetFloat("BGM", out bgmVolume))
            Bgmslider.value = Mathf.Pow(10, bgmVolume / 20); // 데시벨 값을 0~1로 변환

        if (audioMixer.GetFloat("SFX", out sfxVolume))
            Sfxslider.value = Mathf.Pow(10, sfxVolume / 20);
    }

    public void SetBgmVolume(float value)
    {
        AudioManager.Instance.SetBGMVolume(value);
    }

    public void SetSfxVolume(float value)
    {
        AudioManager.Instance.SetSFXVolume(value);
    }
}
