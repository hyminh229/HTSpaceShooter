using UnityEngine;

public class EnemyBullet : ProjectileBase
{
    [SerializeField] private int absorbEnergy = 10;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.TryGetComponent(out PlayerHealth playerHealth)) return;
        if (!collision.TryGetComponent(out PlayerColorController playerColor)) return;

        if (ColorType == playerColor.CurrentColor)
        {
            Absorb(playerHealth);
        }
        else
        {
            DamagePlayer(playerHealth);
        }

        DestroyObject();
    }

    private void Absorb(PlayerHealth playerHealth)
    {
        if (!playerHealth.TryGetComponent(out PlayerEnergy playerEnergy))
        {
            Debug.LogWarning("Player does not have PlayerEnergy component.");
            return;
        }

        playerEnergy.AddEnergy(absorbEnergy);
        Debug.Log("Enemy bullet absorbed! +" + absorbEnergy + " Energy.");
    }

    private void DamagePlayer(PlayerHealth playerHealth)
    {
        int finalDamage = damage * 2;
        playerHealth.TakeDamage(finalDamage);
        Debug.Log("Enemy bullet hit player! Damage x2 = " + finalDamage);
    }
}