using UnityEngine;

[RequireComponent(typeof(MeteorHealth))]
[RequireComponent(typeof(MeteorPolarity))]
public class MeteorController : MonoBehaviour, IDestroyable
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 1.5f;

    [Header("Chroma Reaction")]
    [SerializeField] private MeteorSize meteorSize = MeteorSize.LARGE;
    [SerializeField] private int correctColorDamageMultiplier = 2;
    [SerializeField] private float bounceNudgeDistance = 0.3f;

    [Header("Explosion (chỉ dùng khi meteorSize = SMALL)")]
    [SerializeField] private float explosionRadius = 2f;
    [SerializeField] private int explosionDamage = 2;

    private MeteorHealth meteorHealth;
    private MeteorPolarity meteorPolarity;

    private void Awake()
    {
        meteorHealth = GetComponent<MeteorHealth>();
        meteorPolarity = GetComponent<MeteorPolarity>();
    }

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        transform.Translate(Vector2.down * moveSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.TryGetComponent(out Bullet bullet)) return;
        if (!bullet.IsPlayerBullet) return;

        bool sameColor = bullet.ColorType == meteorPolarity.CurrentColor;

        if (sameColor)
        {
            meteorHealth.TakeDamage(bullet.Damage * correctColorDamageMultiplier);
            bullet.DestroyObject();

            Debug.Log("Correct color hit! Meteor takes bonus damage.");
        }
        else
        {
            HandleWrongColorHit(bullet);
        }
    }

    private void HandleWrongColorHit(Bullet bullet)
    {
        if (meteorSize == MeteorSize.LARGE)
        {
            BounceBullet(bullet);
        }
        else
        {
            bullet.DestroyObject();
            Explode();
        }
    }

    private void BounceBullet(Bullet bullet)
    {
        // Đảo hướng bay của đạn 180 độ, đẩy nhẹ ra khỏi collider để tránh trigger lại ngay lập tức.
        bullet.transform.Rotate(0f, 0f, 90f);
        bullet.transform.position += bullet.transform.up * bounceNudgeDistance;

        Debug.Log("Meteor bounced wrong-color bullet.");
    }

    private void Explode()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explosionRadius);

        foreach (Collider2D hit in hits)
        {
            if (hit.TryGetComponent(out PlayerHealth playerHealth))
            {
                playerHealth.TakeDamage(explosionDamage);
                Debug.Log("Meteor exploded! Player took " + explosionDamage + " AOE damage.");
            }
        }

        meteorHealth.Kill();
    }

    public void DestroyObject()
    {
        Destroy(gameObject);
    }
}