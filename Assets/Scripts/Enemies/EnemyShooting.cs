using UnityEngine;

public class EnemyShooting : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireRate = 1.5f;
    [SerializeField] private Transform player;
    [SerializeField] private float aimRotationOffset = -90f;
    private EnemyController enemyController;
    private float timer;

    private void Awake()
    {
        enemyController = GetComponent<EnemyController>();
    }

    private void Update()
    {
        if (!enemyController.HasStopped)
        {
            return;
        }
        timer += Time.deltaTime;

        if (timer >= fireRate)
        {
            Shoot();
            timer = 0f;
        }
    }

    private void Shoot()
    {
        Vector2 direction = player.position - firePoint.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        float aimAngle = angle + aimRotationOffset;

        firePoint.rotation = Quaternion.Euler(0f, 0f, aimAngle);

        Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
    }
}
