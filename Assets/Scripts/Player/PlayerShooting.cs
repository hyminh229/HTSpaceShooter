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
    private bool isChanneling;

    public bool IsChanneling => isChanneling;

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
        if (isChanneling) return;

        timer += Time.deltaTime;

        if (Input.GetMouseButton(0) && timer >= fireRate)
        {
            Shoot();
            timer = 0f;
        }
    }

    private void HandleMegaBeam()
    {
        if (isChanneling) return;

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

        // Bắn ra từ firePoint theo hướng lên trên, lùi ra khỏi player để không đè lên tàu.
        Vector3 spawnPos = firePoint.position + firePoint.up * megaBeamSpawnOffset;
        GameObject beamObject = Instantiate(megaBeamPrefab, spawnPos, firePoint.rotation);

        if (beamObject.TryGetComponent(out MegaBeam beam))
        {
            beam.Init(this);
        }

        isChanneling = true;
        Debug.Log("MEGA BEAM FIRED!");
    }

    // Được MegaBeam gọi lại khi nó tự huỷ, để mở khoá di chuyển/bắn thường.
    public void EndChanneling()
    {
        isChanneling = false;
    }
}