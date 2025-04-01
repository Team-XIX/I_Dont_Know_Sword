using UnityEngine;

public class Player_Item : MonoBehaviour
{
    public PlayerController player;


    private void Start()
    {
        player = GetComponent<PlayerController>();
    }

}
