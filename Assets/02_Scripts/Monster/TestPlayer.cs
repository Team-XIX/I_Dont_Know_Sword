using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPlayer : MonoBehaviour
{
    public float moveSpeed = 5f; // �̵� �ӵ�
    private Rigidbody2D rb; // Rigidbody ������Ʈ

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>(); // Rigidbody �ʱ�ȭ
    }

    private void Update()
    {
        // ����� �Է� ó��
        float moveX = Input.GetAxis("Horizontal"); // �¿� �Է�
        float moveY = Input.GetAxis("Vertical");   // �յ� �Է�

        // ������ ���� ���
        Vector2 movement = new Vector2(moveX, moveY) * moveSpeed;


        // Rigidbody�� ����� �̵�
        rb.velocity = movement;
    }

}
