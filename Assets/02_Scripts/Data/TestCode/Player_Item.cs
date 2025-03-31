using UnityEngine;

public class Player_Item : MonoBehaviour
{
    

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == 9)
        {
            ItemData item = DataManager.Instance.GetItemById(1);
            Debug.Log(item.value);
        }
    }
}
