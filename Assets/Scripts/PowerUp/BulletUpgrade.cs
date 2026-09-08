using UnityEngine;

public class BulletUpgrade : PowerUp
{
    protected override void ApplyEffect(GameObject player)
    {
        if (!player.TryGetComponent(out PlayerShooting playerShooting))
        {
            Debug.LogWarning("BulletUpgrade: Player is missing PlayerShooting component.");
            return;
        }

        playerShooting.UpgradeShot();
    }
}