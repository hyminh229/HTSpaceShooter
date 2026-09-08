using UnityEngine;

public class EnemyBullet : ProjectileBase
{
    [SerializeField] private int absorbEnergy = 10;
    [SerializeField] private int perfectAbsorbEnergy = 15;
    [SerializeField] private float perfectParryWindow = 0.3f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.TryGetComponent(out PlayerHealth playerHealth)) return;
        if (!collision.TryGetComponent(out PlayerColorController playerColor)) return;

        if (ColorType == playerColor.CurrentColor)
        {
            Absorb(playerHealth, playerColor);
        }
        else if (collision.TryGetComponent(out PlayerShield shield) && shield.TryBlockHit())
        {
            // Khiên đã chặn cú va chạm sai màu này, không trừ máu.
        }
        else
        {
            DamagePlayer(playerHealth);
        }

        DestroyObject();
    }

    private void Absorb(PlayerHealth playerHealth, PlayerColorController playerColor)
    {
        if (!playerHealth.TryGetComponent(out PlayerEnergy playerEnergy))
        {
            Debug.LogWarning("Player does not have PlayerEnergy component.");
            return;
        }

        bool isPerfect = (Time.time - playerColor.LastSwitchTime) <= perfectParryWindow;
        int energyGained = isPerfect ? perfectAbsorbEnergy : absorbEnergy;

        playerEnergy.AddEnergy(ColorType, energyGained);

        Debug.Log(isPerfect
            ? "Perfect Absorb! +" + energyGained + " " + ColorType + " Energy."
            : "Enemy bullet absorbed! +" + energyGained + " " + ColorType + " Energy.");
    }

    private void DamagePlayer(PlayerHealth playerHealth)
    {
        int finalDamage = damage * 2;
        playerHealth.TakeDamage(finalDamage);
        Debug.Log("Enemy bullet hit player! Damage x2 = " + finalDamage);
    }
}