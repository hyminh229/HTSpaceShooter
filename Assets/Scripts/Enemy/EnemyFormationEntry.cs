using UnityEngine;

public class EnemyFormationEntry : MonoBehaviour
{
    [SerializeField] private float entrySpeed = 6f;
    [SerializeField] private float arrivalThreshold = 0.05f;

    private EnemyController enemyController;
    private Vector3 targetPosition;
    private bool isEntering;

    private void Awake()
    {
        enemyController = GetComponent<EnemyController>();
    }

    public void BeginEntry(Vector3 spawnPosition, Vector3 formationTargetPosition)
    {
        transform.position = spawnPosition;
        targetPosition = formationTargetPosition;
        isEntering = true;

        // Tạm khoá pattern di chuyển bình thường trong lúc đang bay vào vị trí,
        // tránh 2 hệ chuyển động (entry + pattern) đánh nhau.
        if (enemyController != null)
        {
            enemyController.enabled = false;
        }
    }

    private void Update()
    {
        if (!isEntering) return;

        transform.position = Vector3.MoveTowards(transform.position, targetPosition, entrySpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPosition) <= arrivalThreshold)
        {
            transform.position = targetPosition;
            isEntering = false;

            // Mở lại pattern — EnemyController.Start() sẽ chạy đúng lúc này,
            // khởi tạo state dựa trên vị trí ĐÃ ổn định, không phải vị trí spawn.
            if (enemyController != null)
            {
                enemyController.enabled = true;
            }
        }
    }
}