using UnityEngine;

public class EnemyShooting : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireRate = 1.5f;
    [SerializeField] private float aimRotationOffset = -90f;
    [SerializeField] private ElementColor bulletColor = ElementColor.BLUE;

    private EnemyController enemyController;
    private Transform player;

    private float timer;

    private void Awake()
    {
        enemyController = GetComponent<EnemyController>();
    }

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
    }

    private void Update()
    {
        if (enemyController == null) return;
        if (!enemyController.HasStopped) return;
        if (player == null) return;

        timer += Time.deltaTime;

        if (timer >= fireRate)
        {
            Shoot();
            timer = 0f;
        }
    }

    private void Shoot()
    {
        if (bulletPrefab == null || firePoint == null)
        {
            Debug.LogWarning("EnemyShooting is missing references.");
            return;
        }

        AimAtPlayer();

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