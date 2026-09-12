using System;
using System.Collections;
using UnityEngine;

// Script DUY NHẤT cho 1 wave dạng lưới kiểu "Chicken Invasion" — thay toàn bộ
// WaveManager/WaveDefinition/WaveFormation cũ. Chỉ có field Grid thật sự cần,
// không còn Shower/Growing/Zigzag/entryEffect dồn chung gây rối Inspector.
public class GridWaveSpawner : MonoBehaviour
{
    [Header("Formation")]
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private int rows = 5;
    [SerializeField] private int columns = 8;

    [Header("Layout (tự tính theo Camera lúc runtime — không phải toạ độ tuyệt đối)")]
    [SerializeField] private float horizontalMargin = 1f;
    [SerializeField] private float topMargin = 1.2f;
    [SerializeField] private float rowSpacing = 1f;

    [Header("Timing")]
    [Tooltip("Khoảng nghỉ giữa lúc 1 hàng xuất hiện xong và hàng kế tiếp bắt đầu.")]
    [SerializeField] private float rowRevealDelay = 0.35f;

    [Header("Shooting (bỏ qua nếu enemyPrefab không có EnemyShooting)")]
    [Range(0f, 1f)][SerializeField] private float shooterChance = 0.25f;

    public event Action OnWaveCleared;

    private int aliveCount;

    private void Start()
    {
        StartCoroutine(SpawnGrid());
    }

    private IEnumerator SpawnGrid()
    {
        aliveCount = rows * columns;

        ComputeLayout(out float startX, out float colSpacing, out float topRowY);

        for (int row = 0; row < rows; row++)
        {
            ElementColor rowColor = GetRandomColor();
            float y = topRowY - row * rowSpacing;

            for (int col = 0; col < columns; col++)
            {
                float x = startX + col * colSpacing;
                SpawnOne(new Vector3(x, y, 0f), rowColor);
            }

            if (row < rows - 1 && rowRevealDelay > 0f)
            {
                yield return new WaitForSeconds(rowRevealDelay);
            }
        }
    }

    // Toạ độ suy ra từ Camera.main NGAY LÚC SPAWN — không còn số tuyệt đối
    // nào phải tự canh tay theo từng scene/camera khác nhau nữa.
    private void ComputeLayout(out float startX, out float colSpacing, out float topRowY)
    {
        Camera cam = Camera.main;
        Vector3 bottomLeft = cam.ViewportToWorldPoint(new Vector3(0f, 0f, cam.nearClipPlane));
        Vector3 topRight = cam.ViewportToWorldPoint(new Vector3(1f, 1f, cam.nearClipPlane));

        float usableWidth = (topRight.x - bottomLeft.x) - 2f * horizontalMargin;
        colSpacing = columns > 1 ? usableWidth / (columns - 1) : 0f;
        startX = bottomLeft.x + horizontalMargin;
        topRowY = topRight.y - topMargin;
    }

    private void SpawnOne(Vector3 position, ElementColor color)
    {
        if (enemyPrefab == null)
        {
            Debug.LogWarning("GridWaveSpawner is missing enemyPrefab.");
            return;
        }

        GameObject instance = Instantiate(enemyPrefab, position, Quaternion.identity);

        if (instance.TryGetComponent(out ChromaPolarityBase polarity))
        {
            polarity.SetColor(color);
        }

        // FIX bug "rơi xuống giữa màn hình": ép cứng Stationary tại đây, không
        // phụ thuộc giá trị mặc định (LinearDown) khai báo sẵn trên EnemyController.
        if (instance.TryGetComponent(out EnemyController controller))
        {
            controller.ConfigureMovement(MovementPattern.Stationary, 0f, float.NegativeInfinity, float.PositiveInfinity);
        }

        if (instance.TryGetComponent(out EnemyShooting shooting))
        {
            shooting.enabled = UnityEngine.Random.value < shooterChance;
        }

        if (instance.TryGetComponent(out EnemyHealth health))
        {
            health.OnDeath += HandleEnemyDeath;
        }
        else
        {
            Debug.LogWarning("GridWaveSpawner: enemyPrefab thiếu EnemyHealth, không đếm được khi chết.");
        }
    }

    private void HandleEnemyDeath()
    {
        aliveCount--;

        if (aliveCount <= 0)
        {
            Debug.Log("Wave cleared!");
            OnWaveCleared?.Invoke();
        }
    }

    private ElementColor GetRandomColor()
    {
        return UnityEngine.Random.value < 0.5f ? ElementColor.BLUE : ElementColor.RED;
    }
}