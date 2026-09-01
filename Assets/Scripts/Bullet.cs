using UnityEngine;

public class Bullet : MonoBehaviour, IDestroyable
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private int damage = 1;

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        transform.Translate(Vector2.up * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IDamageable damageable))
        {
            damageable.TakeDamage(damage);
            DestroyObject();

            Debug.Log("Bullet hit damageable and dealt " + damage + " damage.");
        }
    }

    public void DestroyObject()
    {
        Destroy(gameObject);
    }
}