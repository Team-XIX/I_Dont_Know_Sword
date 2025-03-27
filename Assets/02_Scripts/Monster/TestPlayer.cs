using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPlayer : MonoBehaviour
{
    public float moveSpeed = 5f; // 이동 속도
    private Rigidbody rb; // Rigidbody 컴포넌트

    private void Awake()
    {
        rb = GetComponent<Rigidbody>(); // Rigidbody 초기화
    }

    private void Update()
    {
        // 사용자 입력 처리
        float moveX = Input.GetAxis("Horizontal"); // 좌우 입력
        float moveZ = Input.GetAxis("Vertical");   // 앞뒤 입력

        // 움직임 벡터 계산
        Vector3 movement = new Vector3(moveX, 0f, moveZ) * moveSpeed;

        // Rigidbody를 사용한 이동
        rb.velocity = movement;
    }

}
