using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireRate = 0.2f;

    private PlayerColorController colorController;

    private float timer;

    private void Awake()
    {
        colorController = GetComponent<PlayerColorController>();
    }

    private void Update()
    {
        HandleShooting();
    }

    private void HandleShooting()
    {
        timer += Time.deltaTime;

        if (Input.GetMouseButton(0) && timer >= fireRate)
        {
            Shoot();

            timer = 0f;
        }
    }

    private void Shoot()
    {
        if (bulletPrefab == null || firePoint == null)
        {
            Debug.LogWarning("PlayerShooting is missing references.");
            return;
        }

        GameObject bulletObject =
            Instantiate(
                bulletPrefab,
                firePoint.position,
                firePoint.rotation
            );

        Bullet bullet = bulletObject.GetComponent<Bullet>();

        if (bullet == null)
        {
            Debug.LogError(
                "Bullet prefab does not contain Bullet component."
            );

            return;
        }

        if (colorController != null)
        {
            bullet.SetColor(colorController.CurrentColor);
        }
    }
}