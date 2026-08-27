using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed = 10f;


    private void Update()
    {
        transform.Translate(Vector2.up * speed * Time.deltaTime);
    }

    private void Awake()
    {
        BulletDestroyZone bulletDestroyZone = GetComponent<BulletDestroyZone>();
    }

    private void OnTriggerEnter2D(Collider2D collision2D)
    {
        if (collision2D.gameObject.TryGetComponent(out BulletDestroyZone bulletDestroyZone))
        {
            Destroy(gameObject);
        }
    }

}
