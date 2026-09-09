using UnityEngine;

public class HealPowerUp : PowerUp
{
    [SerializeField] private int healAmount = 3;

    protected override void ApplyEffect(GameObject player)
    {
        if (!player.TryGetComponent(out PlayerHealth playerHealth))
        {
            Debug.LogWarning("HealPowerUp: Player is missing PlayerHealth component.");
            return;
        }

        playerHealth.Heal(healAmount);
    }
}