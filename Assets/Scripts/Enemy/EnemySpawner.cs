using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Bounds Margin")]
    [SerializeField] private float horizontalMargin = 1f;
    [SerializeField] private float topSpawnMargin = 1.5f;

    private Camera mainCamera;
    private float minX;
    private float maxX;
    private float screenTopY;
    private float screenBottomY;

    // Vị trí spawn NGOÀI tầm nhìn phía trên — dùng cho spawn rơi liên tục
    // (Shower / Growing / ZigzagLines).
    public float SpawnY => screenTopY + topSpawnMargin;

    // Mép trên/dưới THẬT của khung hình — dùng để đặt Grid vào TRONG tầm nhìn.
    public float ScreenTopY => screenTopY;
    public float ScreenBottomY => screenBottomY;
    public float MinX => minX;
    public float MaxX => maxX;

    private void Awake()
    {
        mainCamera = Camera.main;
        RecalculateBounds();
    }

    // Tính toàn bộ bounds trực tiếp từ Camera — không phụ thuộc vị trí
    // DestroyZone hay bất kỳ Collider2D nào khác, tránh 2 hệ thống lệch pha.
    private void RecalculateBounds()
    {
        Vector3 bottomLeft = mainCamera.ViewportToWorldPoint(new Vector3(0f, 0f, mainCamera.nearClipPlane));
        Vector3 topRight = mainCamera.ViewportToWorldPoint(new Vector3(1f, 1f, mainCamera.nearClipPlane));

        minX = bottomLeft.x + horizontalMargin;
        maxX = topRight.x - horizontalMargin;
        screenBottomY = bottomLeft.y;
        screenTopY = topRight.y;
    }

    public GameObject SpawnAt(GameObject prefab, Vector3 position, ElementColor color)
    {
        if (prefab == null)
        {
            Debug.LogWarning("EnemySpawner is missing prefab.");
            return null;
        }

        GameObject instance = Instantiate(prefab, position, Quaternion.identity);

        // Meteor không còn ChromaPolarityBase nên dòng này tự bỏ qua với Meteor,
        // chỉ có tác dụng với Enemy.
        if (instance.TryGetComponent(out ChromaPolarityBase polarity))
        {
            polarity.SetColor(color);
        }

        return instance;
    }
}