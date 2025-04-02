using UnityEngine;
using UnityEngine.UI;

public class UICursor : MonoBehaviour
{
    public RectTransform cursor; // 커서 UI 이미지
    public RectTransform canvas; // 캔버스
    Vector2 mousePos; // 마우스 포지션

    void Update()
    {
        // UIManager 인스턴스가 있고, UI가 활성화되었을 경우 커서 표시
        if (UIManager.Instance != null && (UIManager.Instance.pauseMenu.activeSelf || UIManager.Instance.gameOverMenu.activeSelf))
        {
            Cursor.visible = true;
            cursor.gameObject.SetActive(false); // UI 커서를 숨김
            return;
        }

        if (!cursor.gameObject.activeSelf)
        {
            cursor.gameObject.SetActive(true);
        }

        Cursor.visible = false; // 기본적으로 마우스 커서 비활성화

        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas, Input.mousePosition, null, out mousePos);
        //스크린 좌표(Screen Space)에 있는 마우스 위치를 특정 RectTransform(UI 요소 기준)의 로컬 좌표(Local Space)로 변환하는 함수

        // 캔버스 내부에서만 움직이도록 제한
        float clampedX = Mathf.Clamp(mousePos.x, -canvas.rect.width / 2, canvas.rect.width / 2);
        float clampedY = Mathf.Clamp(mousePos.y, -canvas.rect.height / 2, canvas.rect.height / 2);

        cursor.localPosition = new Vector2(clampedX, clampedY);
    }
}
