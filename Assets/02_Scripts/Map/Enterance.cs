using UnityEngine;

public class Enterance : MonoBehaviour
{
    [Header("입구의 방향")]
    [SerializeField]
    private Direction dir;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            MapManager.Instance.mapCreator.Teleport(dir);
        }
    }
}
