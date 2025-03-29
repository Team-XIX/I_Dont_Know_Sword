using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundOptions : MonoBehaviour
{
    public AudioMixer audioMixer;

    public Slider Bgmslider;
    public Slider Sfxslider;

    public void SetBgmVoulme()
    {
        audioMixer.SetFloat("BGM", Mathf.Log10((float)Bgmslider.value) * 20);
        // 로그 함수 Log10은 입력값이 1일때, 0이고 0.1일때 -1, 0.01일때 -2 처럼 감소함
        // 따라서 슬라이더 값이 작아질수록 출력값이 음수가됨
        // 로그 값에 20을 곱하는 이유는 데시벨 변환 공식 때문
        // dB = 20 X 로그10
        //Bgmslider.value = 1  -> Mathf.Log10(1)  * 20 =  0 dB  (원래 볼륨)
        //Bgmslider.value = 0.5->Mathf.Log10(0.5) * 20 ≈ -6 dB(절반 볼륨)
        //Bgmslider.value = 0.1->Mathf.Log10(0.1) * 20 = -20 dB(거의 무음)
    }

    public void SetSfxVolume()
    {
        audioMixer.SetFloat("SFX", Mathf.Log10((float)Sfxslider.value) * 20);
    }
}
