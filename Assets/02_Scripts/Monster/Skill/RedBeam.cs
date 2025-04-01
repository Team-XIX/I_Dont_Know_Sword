using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class RedBeam : MonoBehaviour
{
    LayerMask playerLayer;

    // 활성화 되면, 1초 후 (콜라이더 활성화 + scale.y 가 0.01에서 0.3로 dotween으로 0.5초동안 커짐) 이후  0.5초후 0.5초의 시간동안 크기 복구.
    private void Awake()
    {
        playerLayer = LayerMask.NameToLayer("Player");
    }
    private void OnEnable()
    {
        StartCoroutine(ActivateBeam());
    }
    private void OnDisable()
    {
        GetComponent<BoxCollider2D>().enabled = false;
    }// 레이저 종료시 콜라이더 다시 비활성화
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == playerLayer)
        {
            other.GetComponent<PlayerController>().TakeDamage(1);
        }
    }// 피격시 데미지
    IEnumerator ActivateBeam()
    {
        yield return new WaitForSeconds(1f);
        GetComponent<BoxCollider2D>().enabled = true;
        transform.DOScaleY(0.3f, 0.5f);
        yield return new WaitForSeconds(0.5f);
        transform.DOScaleY(0.01f, 0.5f);
    }
}
