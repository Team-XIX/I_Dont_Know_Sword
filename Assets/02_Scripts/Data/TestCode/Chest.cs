using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour, IDamageable
{
    public GameObject[] dropItems;
    private Animator _animator;

    private void Start()
    {
        _animator = GetComponent<Animator>();

        if (dropItems.Length == 0)
            dropItems = Resources.LoadAll<GameObject>("PrefabItem");
    }

    public void TakeDamage(int damage)
    {
        _animator.SetTrigger("Open");
    }

    private void CreateRandomItem() //call at Open Animation Event
    {
        int randNum = Random.Range(0, 101);
        int selectNum;
        if (randNum > 90)
            selectNum = 2;
        else if (randNum > 60)
            selectNum = 0;
        else
            selectNum = 1;
        Instantiate(dropItems[selectNum],transform.position,Quaternion.identity);
        Destroy(gameObject);
    }
}
