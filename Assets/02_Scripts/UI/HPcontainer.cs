using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using UnityEngine.Pool;

public class HPcontainer : MonoBehaviour
{
    public GameObject heartPrefab;
    public Transform heartParent;

    private List<GameObject> hearts = new List<GameObject>(); // 현재 활성화 된 하트풀
    private List<GameObject> availableHearts = new List<GameObject>(); // 현재 비활성화 되어있는 하트풀
    private bool isQuitting = false;
    private void Start()
    {
        StatHandler.Instance.OnHealthChanged += UpdateHearts;
        UpdateHearts();

        StartCoroutine(InitializeStatHandler());
    }

    private void OnDisable()
    {
        if (!isQuitting)
        {
           StatHandler.Instance.OnHealthChanged -= UpdateHearts;
        }
    }

    void OnApplicationQuit()
    {
        isQuitting = true;
    }

    private IEnumerator InitializeStatHandler()
    {
        while(StatHandler.Instance == null)
        {
            Debug.Log("missing stathandler");
            yield return new WaitForSeconds(0.5f);
        }

        StatHandler.Instance.OnHealthChanged += UpdateHearts;
        Debug.Log("complete event");
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
