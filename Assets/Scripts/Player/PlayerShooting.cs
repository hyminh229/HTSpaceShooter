using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    [Header("Normal Shooting")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireRate = 0.2f;

    [Header("Mega Beam")]
    [SerializeField] private GameObject megaBeamPrefab;
    [SerializeField] private float megaBeamSpawnOffset = 1.5f; // = nửa chiều dài beam, chỉnh trong Inspector

    private PlayerColorController colorController;
    private PlayerEnergy playerEnergy;

    private float timer;

    private void Awake()
    {
        colorController = GetComponent<PlayerColorController>();
        playerEnergy = GetComponent<PlayerEnergy>();
    }

    private void Update()
    {
        HandleShooting();
        HandleMegaBeam();
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

    private void HandleMegaBeam()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            ShootMegaBeam();
        }
    }

    private void Shoot()
    {
        if (bulletPrefab == null || firePoint == null)
        {
            Debug.LogWarning("PlayerShooting is missing bullet references.");
            return;
        }

        GameObject bulletObject = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        if (!bulletObject.TryGetComponent(out Bullet bullet))
        {
            Debug.LogError("Bullet prefab does not contain Bullet component.");
            return;
        }

        if (colorController != null)
        {
            bullet.SetColor(colorController.CurrentColor);
        }
    }

    private void ShootMegaBeam()
    {
        if (megaBeamPrefab == null || firePoint == null)
        {
            Debug.LogWarning("PlayerShooting is missing Mega Beam references.");
            return;
        }

        if (playerEnergy == null)
        {
            Debug.LogWarning("Player does not have PlayerEnergy component.");
            return;
        }

        if (!playerEnergy.UseMegaBeam())
        {
            Debug.Log("Energy is not full. Cannot use Mega Beam.");
            return;
        }

        // Bắn ra từ firePoint theo hướng lên trên (firePoint.up), lùi ra khỏi player một khoảng
        // = nửa chiều dài beam để không đè lên tàu. Nếu đổi pivot sprite beam sang "Bottom"
        // thì có thể bỏ offset này và Instantiate thẳng tại firePoint.position.
        Vector3 spawnPos = firePoint.position + firePoint.up * megaBeamSpawnOffset;
        Instantiate(megaBeamPrefab, spawnPos, firePoint.rotation);

        Debug.Log("MEGA BEAM FIRED!");
    }
}