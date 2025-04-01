using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EyeOfDeath : MonoBehaviour
{
    // 생성되면 플레이어를 빠른 속도로 따라가다 플레이어와 충돌하면 터지는 애니메이션.
    [SerializeField] float speed = 2f;
    public PlayerController target;
    SpriteRenderer spriteRenderer;
    LayerMask playerLayer;
    bool boom = false;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerLayer = LayerMask.NameToLayer("Player");
    }
    private void Start()
    {
        StartCoroutine(MoveToPlayer());
    }
    private void Update()
    {
        spriteRenderer.flipX = target.transform.position.x < transform.position.x;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == playerLayer)
        {
            this.GetComponent<Animator>().SetTrigger("OnCollision");
        }
    }
    IEnumerator MoveToPlayer()
    {
        if(target == null)
        {
            this.GetComponent<Animator>().SetTrigger("OnCollision");
            StopCoroutine(MoveToPlayer());
            yield break;
        }
        while(!boom)
        {
            Vector2 direction = ((Vector2)target.transform.position - (Vector2)transform.position).normalized;
            float moveDistance = speed * Time.deltaTime;
            Vector2 nextPosition = (Vector2)transform.position + direction * moveDistance;

            transform.position = nextPosition;
            yield return null;
        }
    }

    // 애니메이션 이벤트
    public void Bomb()// 플레이어와 충돌시 터지는 애니메이션의 이벤트 함수.
    {
        boom = true;
        if (target == null) return;
        if(Vector2.Distance(transform.position, target.transform.position) < 0.7f)
        {
            target.TakeDamage(1);
        }
        Destroy(gameObject);
    }
}
