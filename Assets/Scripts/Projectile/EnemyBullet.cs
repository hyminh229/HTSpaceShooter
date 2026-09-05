using UnityEngine;

public class EnemyBullet : MonoBehaviour, IDestroyable
{
    [SerializeField] private float speed = 7f;
    [SerializeField] private int damage = 1;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Color blueColor = Color.blue;
    [SerializeField] private Color redColor = Color.red;

    public ElementColor ColorType { get; private set; }

    private void Awake()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
    }

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        transform.Translate(
            Vector2.up * speed * Time.deltaTime
        );
    }

    public void SetColor(ElementColor newColor)
    {
        ColorType = newColor;

        UpdateVisual();
    }

    private void UpdateVisual()
    {
        if (spriteRenderer == null)
        {
            return;
        }

        if (ColorType == ElementColor.BLUE)
        {
            spriteRenderer.color = blueColor;
        }
        else
        {
            spriteRenderer.color = redColor;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerHealth playerHealth =
            collision.GetComponent<PlayerHealth>();

        if (playerHealth == null)
        {
            return;
        }

        PlayerColorController playerColor =
            collision.GetComponent<PlayerColorController>();

        if (playerColor == null)
        {
            return;
        }

        if (ColorType == playerColor.CurrentColor)
        {
            // Same color = absorb
            Debug.Log(
                "Enemy bullet absorbed! Same color."
            );

            // Energy +10% sẽ được thêm ở Phase 3.
        }
        else
        {
            // Different color = damage x2
            int finalDamage = damage * 2;

            Debug.Log(
                "Enemy bullet hit player! Damage x2 = "
                + finalDamage
            );

            playerHealth.TakeDamage(finalDamage);
        }

        DestroyObject();
    }

    public void DestroyObject()
    {
        Destroy(gameObject);
    }
}