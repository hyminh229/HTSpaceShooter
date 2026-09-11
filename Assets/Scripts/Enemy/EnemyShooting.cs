using UnityEngine;

public class EnemyShooting : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireCheckInterval = 1.5f;
    [SerializeField][Range(0f, 1f)] private float fireChance = 0.5f;
    [SerializeField] private float aimRotationOffset = -90f;
    [SerializeField] private ElementColor bulletColor = ElementColor.BLUE;
    [SerializeField] private bool aimAtPlayer = true;

    private Transform player;
    private float timer;

    private void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
            Debug.LogWarning(
                "EnemyShooting could not find Player. " +
                "Make sure Player has the 'Player' tag."
            );
        }

        // Lệch pha ngẫu nhiên, tránh cả đám vài chục con "roll xúc xắc"
        // đúng cùng 1 thời điểm.
        timer = Random.Range(0f, fireCheckInterval);
    }

    private void Update()
    {
        if (aimAtPlayer && player == null) return;

        timer += Time.deltaTime;

        if (timer >= fireCheckInterval)
        {
            timer = 0f;

            // Không bắn đều tay: mỗi mốc fireCheckInterval chỉ có fireChance
            // cơ hội thực sự bắn. Kết hợp với EnemyShooting.enabled bị
            // WaveManager tắt ngẫu nhiên trên nhiều con, đạn sẽ rơi thưa
            // và bất ngờ thay vì thành bức tường đạn.
            if (Random.value <= fireChance)
            {
                Shoot();
            }
        }
    }

    private void Shoot()
    {
        if (bulletPrefab == null || firePoint == null)
        {
            Debug.LogWarning("EnemyShooting is missing references.");
            return;
        }

        if (aimAtPlayer)
        {
            AimAtPlayer();
        }
        else
        {
            firePoint.rotation = Quaternion.Euler(0f, 0f, 180f);
        }

        GameObject bulletObject = ObjectPooler.Instance != null
            ? ObjectPooler.Instance.Spawn(bulletPrefab, firePoint.position, firePoint.rotation)
            : Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        if (bulletObject == null) return;

        if (!bulletObject.TryGetComponent(out EnemyBullet enemyBullet))
        {
            Debug.LogError("Enemy bullet prefab does not contain EnemyBullet.");
            return;
        }

        enemyBullet.SetColor(bulletColor);
    }

    private void AimAtPlayer()
    {
        Vector2 direction = player.position - firePoint.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        float aimAngle = angle + aimRotationOffset;
        firePoint.rotation = Quaternion.Euler(0f, 0f, aimAngle);
    }
}