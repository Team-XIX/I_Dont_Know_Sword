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

    void Start()
    {
        if (targetButton != null)
            originalColor = buttonText.color;
    }

    public void OnPointerEnter(PointerEventData eventData) // 커서가 닿았을때
    {
        if(buttonText != null)
        buttonText.color = clickedColor;
    }

    public void OnPointerExit(PointerEventData eventData) //커서가 때졌을때
    {
        if(buttonText != null)
        buttonText.color = originalColor;
    }
}