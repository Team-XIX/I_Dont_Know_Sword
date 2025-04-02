using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingRay : MonoBehaviour
{
    public float rotationSpeed = 30f; // 5초에 한 바퀴(360도/12초 = 30도/초)

    void Update()
    {
        if (gameObject.activeSelf) // 오브젝트가 활성화된 경우
        {
            transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
        }
    }
}
