using UnityEngine;

public class EnemyBullet : MonoBehaviour,IDestroyable
{
    [SerializeField] private float speed = 6f;

    private void Update()
    {
        Move();
    }

    public void DestroyObject()
    {
        Destroy(gameObject);
    }

    private void Move()
    {
        transform.Translate(Vector2.up * speed * Time.deltaTime);
    }

    
}
