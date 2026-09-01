using UnityEngine;

public class EnemyController : MonoBehaviour, IDestroyable
{
    [SerializeField] private float moveSpeed = 4f;

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        transform.Translate(Vector2.down * moveSpeed * Time.deltaTime);
    }

    public void DestroyObject()
    {
        Destroy(gameObject);
    }
}