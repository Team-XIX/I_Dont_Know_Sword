using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class HPcontainer : MonoBehaviour
{
    public GameObject heartPrefab;
    public Transform heartParent;

    private List<GameObject> hearts = new List<GameObject>(); // 현재 활성화 된 하트풀
    private List<GameObject> availableHearts = new List<GameObject>(); // 현재 비활성화 되어있는 하트풀
    private bool isQuitting = false; // 게임이 끝난 후에 널참조 해결
    private void Start() // 기존에 등록되어 있는 함수 구독
    {
        StatHandler.Instance.OnHealthChanged += UpdateHearts;
        UpdateHearts();

        StartCoroutine(InitializeStatHandler()); 
    }

    private void OnDisable() // 비활성화시 구독 취소
    {
        if (!isQuitting)
        {
           StatHandler.Instance.OnHealthChanged -= UpdateHearts;
        }
    }

    private IEnumerator InitializeStatHandler() // 코루틴을 통해서 기존에 있는 스탯핸들러 찾아서 구독시켜주기 
    {
        while(StatHandler.Instance == null)
        {
            Debug.Log("missing stathandler");
            yield return new WaitForSeconds(0.5f);
        }

        StatHandler.Instance.OnHealthChanged += UpdateHearts;
    }
    public void UpdateHearts()
    {
        int curHealth = StatHandler.Instance.CurrentHealth;
    
        while (hearts.Count > curHealth) // 기존에 활성화 되있는 하트 비활후 풀에 추가
        {
            int lastIndex = hearts.Count - 1;
            GameObject heart = hearts[lastIndex];
            hearts.RemoveAt(hearts.Count - 1);
            heart.SetActive(false);
            availableHearts.Add(heart);
        }

        while(hearts.Count < curHealth) // 실질적으로 게임에서 나타나는 하트 UI부분
        {
            GameObject heart;
            if (hearts.Count > curHealth)
            {
                heart = availableHearts[0];
                availableHearts.RemoveAt(0);
                heart.SetActive(true);
            }
            else
            {
                heart = Instantiate(heartPrefab, heartParent);
            }
            hearts.Add(heart);
        }
    }
}
