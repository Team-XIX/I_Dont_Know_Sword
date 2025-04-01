using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SummonMonster : MonoBehaviour
{
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] int summonCount = 3;// 소환할 몬스터 수
    [SerializeField] GameObject monsterPrefab;// 소환할 몬스터 프리팹
    public PlayerController playerController;// 플레이어 (타겟) 의 위치를 받기 위한 변수.

    private void OnEnable()
    {
        StartCoroutine(Summon());
    }
    void OnDestroy()
    {
        StopAllCoroutines();
    }
    IEnumerator Summon()
    {
        //spriterenderer의 알파값을 1.5초동안 1로 변경
        float alpha = 0;
        while (alpha < 1)
        {
            alpha += 0.05f;
            spriteRenderer.color = new Color(0, 0, 0, alpha);
            yield return new WaitForSeconds(0.1f);
        }
        for (int i = 0; i < summonCount; i++)
        {
            float randomX = Random.Range(-1f, 1f);
            float randomY = Random.Range(-1f, 1f);

            GameObject monster = Instantiate(monsterPrefab, transform.position + new Vector3 (randomX,randomY,0), Quaternion.identity);
            monster.GetComponent<EyeOfDeath>().target = playerController;
        }
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }
}
