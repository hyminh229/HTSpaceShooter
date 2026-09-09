using UnityEngine;

public class ShieldPowerUp : PowerUp
{
    [SerializeField] private int shieldCharges = 3;

    protected override void ApplyEffect(GameObject player)
    {
        if (!player.TryGetComponent(out PlayerShield shield))
        {
            Debug.LogWarning("ShieldPowerUp: Player is missing PlayerShield component.");
            return;
        }

        shield.Activate(shieldCharges);
    }
}