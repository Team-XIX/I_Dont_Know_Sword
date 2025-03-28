using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ChangeButtonColor : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Button targetButton; // 버튼
    public TMP_Text buttonText; 

    public Color clickedColor = Color.gray; // 클릭 시 변경할 색상
    private Color originalColor; // 원래 색상 적용

    public AudioSource audioSource; // 오디오 소스
    public AudioClip cursorSound; // 효과음

    void Start()
    {
        if (targetButton != null)
            originalColor = buttonText.color;
        if(audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    public void OnPointerEnter(PointerEventData eventData) // 커서가 닿았을때
    {
        if(buttonText != null)
        buttonText.color = clickedColor;
        audioSource.PlayOneShot(cursorSound);
    }

    public void OnPointerExit(PointerEventData eventData) //커서가 때졌을때
    {
        if(buttonText != null)
        buttonText.color = originalColor;
    }
}