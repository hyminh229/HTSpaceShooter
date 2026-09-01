using UnityEngine;

public class DestroyZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IDestroyable destroyable))
        {
            destroyable.DestroyObject();
        }
    }
}