using UnityEngine;

public class Bullet : ProjectileBase
{
    [SerializeField] private bool isPlayerBullet = true;
    public bool IsPlayerBullet => isPlayerBullet;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isPlayerBullet) return;
        if (!collision.TryGetComponent(out EnemyHealth enemyHealth)) return;

        int finalDamage = damage;

        if (collision.TryGetComponent(out EnemyPolarity enemyPolarity))
        {
            bool sameColor = ColorType == enemyPolarity.CurrentColor;
            finalDamage = sameColor ? damage * 2 : damage;

            Debug.Log(sameColor
                ? "Same color hit! Damage x2 = " + finalDamage
                : "Different color hit! Damage = " + finalDamage);
        }

        enemyHealth.TakeDamage(finalDamage);
        DestroyObject();
    }
}