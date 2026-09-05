using UnityEngine;

public class Bullet : MonoBehaviour, IDestroyable
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private int damage = 1;
    [SerializeField] private bool isPlayerBullet = true;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Color blueColor = Color.blue;
    [SerializeField] private Color redColor = Color.red;

    public ElementColor ColorType { get; private set; }

    public int Damage => damage;

    public bool IsPlayerBullet => isPlayerBullet;

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
        transform.Translate(Vector2.up * speed * Time.deltaTime);
    }

    public void SetColor(ElementColor newColor)
    {
        ColorType = newColor;

        UpdateVisual();
    }

    public void SetDamage(int newDamage)
    {
        damage = newDamage;
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
        if (!isPlayerBullet)
        {
            return;
        }

        EnemyHealth enemyHealth = collision.GetComponent<EnemyHealth>();

        if (enemyHealth != null)
        {
            EnemyPolarity enemyPolarity =
                collision.GetComponent<EnemyPolarity>();

            int finalDamage = damage;

            if (enemyPolarity != null)
            {
                if (ColorType == enemyPolarity.CurrentColor)
                {
                    finalDamage = damage * 2;

                    Debug.Log(
                        "Same color hit! Damage x2 = " + finalDamage
                    );
                }
                else
                {
                    Debug.Log(
                        "Different color hit! Damage = " + finalDamage
                    );
                }
            }

            enemyHealth.TakeDamage(finalDamage);

            DestroyObject();
        }
    }

    public void DestroyObject()
    {
        Destroy(gameObject);
    }
}