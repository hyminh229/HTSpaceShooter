using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Area")]
    [SerializeField] private Collider2D topBoundary; // Kéo Collider2D của DestroyZoneTop vào đây
    [SerializeField] private float edgeMargin = 1.5f; // Cách 2 đầu thanh 1 khoảng, tránh spawn dính DestroyZoneLeft/Right
    [SerializeField] private float spawnYMargin = 1.5f; // Đệm phía trên mép camera, tránh chạm DestroyZoneTop khi vừa spawn

    private Camera mainCamera;
    private float spawnY;
    private float minX;
    private float maxX;

    private void Start()
    {
        mainCamera = Camera.main;
        CalculateSpawnBounds();
    }

    private void CalculateSpawnBounds()
    {
        Vector3 topWorld = mainCamera.ViewportToWorldPoint(new Vector3(0.5f, 1f, mainCamera.nearClipPlane));
        spawnY = topWorld.y + spawnYMargin;

        if (topBoundary != null)
        {
            Bounds bounds = topBoundary.bounds;
            minX = bounds.min.x + edgeMargin;
            maxX = bounds.max.x - edgeMargin;
        }
        else
        {
            Debug.LogWarning("EnemySpawner: topBoundary chưa gán, dùng mặc định -8..8.");
            minX = -8f;
            maxX = 8f;
        }
    }

    public EnemyController SpawnEnemy(GameObject enemyPrefab, ElementColor bodyColor)
    {
        if (enemyPrefab == null)
        {
            Debug.LogWarning("EnemySpawner is missing enemy prefab.");
            return null;
        }

        float randomX = Random.Range(minX, maxX);
        Vector3 spawnPosition = new Vector3(randomX, spawnY, 0f);

        GameObject enemyObject = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);

        if (enemyObject.TryGetComponent(out EnemyPolarity enemyPolarity))
        {
            enemyPolarity.SetColor(bodyColor);
        }

        return enemyObject.GetComponent<EnemyController>();
    }
}