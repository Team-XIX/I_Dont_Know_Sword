using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterDetection : MonoBehaviour
{
    [SerializeField] MonsterBase monsterBase;
    int playerLayerMask;
    private void Start()
    {
        playerLayerMask = LayerMask.NameToLayer("Player");
        this.GetComponent<CircleCollider2D>().radius = monsterBase.detectRange;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Collision");
        if (collision.gameObject.layer == playerLayerMask)
        {
            monsterBase.target = collision.gameObject.GetComponent<PlayerController>();
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == playerLayerMask)
        {
            monsterBase.target = null;
        }
    }
}
