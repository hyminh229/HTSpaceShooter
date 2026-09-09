using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EnemyTier
{
    public string tierName = "Tier";
    public List<GameObject> enemyPrefabs = new List<GameObject>();
    public float unlockTime = 0f; // giây kể từ lúc bắt đầu game, tier này được mở khoá
}

public class EnemyDirector : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private EnemySpawner enemySpawner;

    [Header("Enemy Tiers (giới thiệu tăng dần theo thời gian)")]
    [SerializeField] private List<EnemyTier> tiers = new List<EnemyTier>();

    [Header("Spawn Timing")]
    [SerializeField] private float initialSpawnInterval = 2.5f;
    [SerializeField] private float minSpawnInterval = 0.8f;
    [SerializeField] private float spawnIntervalDecreasePerMinute = 0.3f;

    [Header("Chroma Pattern")]
    [SerializeField] private int minColorClusterSize = 2;
    [SerializeField] private int maxColorClusterSize = 4;

    [Header("Power-Up Drop")]
    [SerializeField] private GameObject[] powerUpPrefabs;
    [SerializeField][Range(0f, 1f)] private float powerUpDropChancePerKill = 0.05f;

    private readonly List<GameObject> unlockedPrefabs = new List<GameObject>();
    private int nextTierIndex;
    private float elapsedTime;
    private float spawnTimer;
    private ChromaClusterState colorState;

    private void Start()
    {
        colorState = new ChromaClusterState(minColorClusterSize, maxColorClusterSize);
        UnlockTiersDueAt(0f);
    }

    private void Update()
    {
        elapsedTime += Time.deltaTime;
        UnlockTiersDueAt(elapsedTime);

        spawnTimer += Time.deltaTime;

        if (spawnTimer >= GetCurrentSpawnInterval())
        {
            spawnTimer = 0f;
            SpawnRandomEnemy();
        }
    }

    private void UnlockTiersDueAt(float time)
    {
        while (nextTierIndex < tiers.Count && tiers[nextTierIndex].unlockTime <= time)
        {
            EnemyTier tier = tiers[nextTierIndex];
            unlockedPrefabs.AddRange(tier.enemyPrefabs);

            Debug.Log("Enemy tier unlocked: " + tier.tierName);

            nextTierIndex++;
        }
    }

    private float GetCurrentSpawnInterval()
    {
        float minutesElapsed = elapsedTime / 60f;
        float interval = initialSpawnInterval - (spawnIntervalDecreasePerMinute * minutesElapsed);
        return Mathf.Max(minSpawnInterval, interval);
    }

    private void SpawnRandomEnemy()
    {
        if (unlockedPrefabs.Count == 0) return;

        GameObject prefab = unlockedPrefabs[UnityEngine.Random.Range(0, unlockedPrefabs.Count)];
        ElementColor color = colorState.Next();

        EnemyController enemy = enemySpawner.SpawnEnemy(prefab, color);

        if (enemy != null && enemy.TryGetComponent(out EnemyHealth enemyHealth))
        {
            // Capture vị trí lúc chết qua transform của chính con enemy này (closure),
            // không cần đổi signature OnDeath trong EnemyHealth (event OnDeath fire trước Destroy()).
            Transform enemyTransform = enemy.transform;
            enemyHealth.OnDeath += () => HandleEnemyDeath(enemyTransform.position);
        }
    }

    private void HandleEnemyDeath(Vector3 deathPosition)
    {
        if (powerUpPrefabs == null || powerUpPrefabs.Length == 0) return;
        if (UnityEngine.Random.value > powerUpDropChancePerKill) return;

        GameObject prefab = powerUpPrefabs[UnityEngine.Random.Range(0, powerUpPrefabs.Length)];
        Instantiate(prefab, deathPosition, Quaternion.identity);

        Debug.Log("Power-up dropped at " + deathPosition);
    }
}