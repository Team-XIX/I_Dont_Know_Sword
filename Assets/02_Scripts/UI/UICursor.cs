using UnityEngine;
using UnityEngine.UI;

public class UICursor : MonoBehaviour
{
    public RectTransform cursor; // Ŀ�� UI �̹���
    public RectTransform canvas; // ĵ����

    Vector2 mousePos; // ���콺 ������

    void Update()
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas, Input.mousePosition, null, out mousePos);
        //��ũ�� ��ǥ(Screen Space)�� �ִ� ���콺 ��ġ�� Ư�� RectTransform(UI ��� ����)�� ���� ��ǥ(Local Space)�� ��ȯ�ϴ� �Լ�

        // ĵ���� ���ο����� �����̵��� ����
        float clampedX = Mathf.Clamp(mousePos.x, -canvas.rect.width / 2, canvas.rect.width / 2);
        float clampedY = Mathf.Clamp(mousePos.y, -canvas.rect.height / 2, canvas.rect.height / 2);

        cursor.localPosition = new Vector2(clampedX, clampedY);
    }
}
