using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public abstract class PowerUp : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.TryGetComponent(out PlayerHealth playerHealth)) return;

        ApplyEffect(collision.gameObject);
        Destroy(gameObject);
    }

    protected abstract void ApplyEffect(GameObject player);
}