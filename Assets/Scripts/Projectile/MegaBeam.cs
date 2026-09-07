using UnityEngine;

public class MegaBeam : MonoBehaviour, IDestroyable
{
    [SerializeField] private float beamDuration = 1f;
    [SerializeField] private int damage = 10;

    private void Start()
    {
        Invoke(nameof(DestroyObject), beamDuration);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out EnemyHealth enemyHealth))
        {
            enemyHealth.TakeDamage(damage);
        }
    }

    public void DestroyObject()
    {
        Destroy(gameObject);
    }
}