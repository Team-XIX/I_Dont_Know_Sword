using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPlayer : MonoBehaviour
{
    public float moveSpeed = 5f; // �̵� �ӵ�
    private Rigidbody rb; // Rigidbody ������Ʈ

    private void Awake()
    {
        rb = GetComponent<Rigidbody>(); // Rigidbody �ʱ�ȭ
    }

    private void Update()
    {
        // ����� �Է� ó��
        float moveX = Input.GetAxis("Horizontal"); // �¿� �Է�
        float moveZ = Input.GetAxis("Vertical");   // �յ� �Է�

        // ������ ���� ���
        Vector3 movement = new Vector3(moveX, 0f, moveZ) * moveSpeed;

        // Rigidbody�� ����� �̵�
        rb.velocity = movement;
    }

}
