using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterProjectile : MonoBehaviour
{
    [SerializeField] LayerMask wallLayer;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent(out PlayerController player))
        {
            player.TakeDamage(1);
            this.gameObject.SetActive(false);
        }

        if((wallLayer.value & (1 << collision.gameObject.layer)) > 0)// 벽에 닿았을 경우
        {
            this.gameObject.SetActive(false);
        }
    }
}
