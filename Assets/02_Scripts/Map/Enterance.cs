using UnityEngine;

public class Enterance : MonoBehaviour
{
    [SerializeField]
    private Direction dir;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            MapCreator.teleportAction?.Invoke(dir);
        }
    }
}
