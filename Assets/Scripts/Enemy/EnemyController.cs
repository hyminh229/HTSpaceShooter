using UnityEngine;

public class EnemyController : MonoBehaviour, IDestroyable
{
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private bool shouldStop;
    [SerializeField] private float stopY = -3f;

    public bool HasStopped { get; private set; }
    private void Start()
    {
        HasStopped = false;
    }

    private void Update()
    {
        Move();
        if (shouldStop && transform.position.y <= stopY)
        {
            moveSpeed = 0f;
            HasStopped = true;
        }

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