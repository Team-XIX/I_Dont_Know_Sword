using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathHand : MonoBehaviour
{
    [SerializeField] LayerMask playerLayer;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if((playerLayer.value & (1 << collision.gameObject.layer)) > 0)//플레이어 레이어에 닿았을 경우
        {
            Debug.Log("Player attack");//idamageable 인터페이스를 구현한 플레이어에게 데미지를 입히는 코드로 추후 변경.
        }
    }

    public void AnimEventTurnOnHit()// 이펙트 도중 피격 판정 활성화
    {
        GetComponent<Collider2D>().enabled = true;
    }
    public void AnimEventTurnOffHit()// 이펙트 종료시 피격 판정 비활성화
    { 
        GetComponent<Collider2D>().enabled = false;
    }
}
