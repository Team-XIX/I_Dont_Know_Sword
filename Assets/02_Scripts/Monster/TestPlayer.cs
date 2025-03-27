using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPlayer : MonoBehaviour
{
    public float moveSpeed = 5f; // 이동 속도
    private Rigidbody2D rb; // Rigidbody 컴포넌트

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>(); // Rigidbody 초기화
    }

    private void Update()
    {
        // 사용자 입력 처리
        float moveX = Input.GetAxis("Horizontal"); // 좌우 입력
        float moveY = Input.GetAxis("Vertical");   // 앞뒤 입력

        // 움직임 벡터 계산
        Vector2 movement = new Vector2(moveX, moveY) * moveSpeed;


        // Rigidbody를 사용한 이동
        rb.velocity = movement;
    }

}
