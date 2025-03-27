using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public Transform target;
    float offsetX, offsetY;
    public float smoothSpeed = 0.75f; // 카메라 이동 속도
    public float followThreshold = 2.0f; // 타겟과 카메라의 거리 임계값

    private void Start()
    {
        if (target == null) return;

        offsetX = transform.position.x - target.position.x;
        offsetY = transform.position.y - target.position.y;
    }

    /// <summary>
    /// 카메라가 부드럽게 움직이는 연출
    /// </summary>
    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = new Vector3(target.position.x + offsetX, target.position.y + offsetY, transform.position.z);
        float distance = Vector3.Distance(transform.position, desiredPosition);

        if (distance > followThreshold) // target과 카메라 중심의 거리가 멀어지면
        {
            float dynamicSmoothSpeed = smoothSpeed * (distance / followThreshold); // 거리 비례 속도 조절(멀수록 빨라진다)
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, dynamicSmoothSpeed * Time.deltaTime);
            transform.position = smoothedPosition;
        }
        else // 두 개체 간의 거리가 가까울 때 (보통 플레이어가 멈춰 있을 때)
        {
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
            transform.position = smoothedPosition;
        }
    }
}