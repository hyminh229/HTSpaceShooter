using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Area")]
    [SerializeField] private Collider2D topBoundary;
    [SerializeField] private float edgeMargin = 1.5f;
    [SerializeField] private float spawnYMargin = 1.5f;

    private Camera mainCamera;
    private float spawnY;
    private float minX;
    private float maxX;

    public float SpawnY => spawnY;
    public float MinX => minX;
    public float MaxX => maxX;

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

    public GameObject SpawnAt(GameObject prefab, Vector3 position, ElementColor color)
    {
        if (prefab == null)
        {
            Debug.LogWarning("EnemySpawner is missing prefab.");
            return null;
        }

        GameObject instance = Instantiate(prefab, position, Quaternion.identity);

        if (instance.TryGetComponent(out ChromaPolarityBase polarity))
        {
            polarity.SetColor(color);
        }

        return instance;
    }
}