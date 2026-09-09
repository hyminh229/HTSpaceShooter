using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private Transform[] spawnPoints;

    // Enemy không nằm trong phạm vi pooling đã chốt -> Instantiate bình thường.
    public EnemyController SpawnEnemy(GameObject enemyPrefab, ElementColor bodyColor)
    {
        if (enemyPrefab == null || spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogWarning("EnemySpawner is missing prefab or spawn points.");
            return null;
        }

        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        GameObject enemyObject = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);

        if (enemyObject.TryGetComponent(out EnemyPolarity enemyPolarity))
        {
            enemyPolarity.SetColor(bodyColor);
        }

        return enemyObject.GetComponent<EnemyController>();
    }
}