using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class WaveEntry
{
    public GameObject enemyPrefab;
    public int count = 3;
}

public class WaveManager : MonoBehaviour
{
    [SerializeField] private EnemySpawner enemySpawner;
    [SerializeField] private List<WaveEntry> waves = new List<WaveEntry>();
    [SerializeField] private float delayBetweenWaves = 2f;
    [SerializeField] private int minColorClusterSize = 2;
    [SerializeField] private int maxColorClusterSize = 4;

    private int currentWaveIndex = -1;
    private int aliveEnemyCount;

    private void Start()
    {
        StartNextWave();
    }

    private void StartNextWave()
    {
        currentWaveIndex++;

        if (currentWaveIndex >= waves.Count)
        {
            Debug.Log("All waves cleared!");
            return;
        }

        WaveEntry wave = waves[currentWaveIndex];
        ElementColor[] colorPattern = ChromaPatternGenerator.GenerateClusterPattern(
            wave.count, minColorClusterSize, maxColorClusterSize);

        aliveEnemyCount = wave.count;

        for (int i = 0; i < wave.count; i++)
        {
            EnemyController enemy = enemySpawner.SpawnEnemy(wave.enemyPrefab, colorPattern[i]);

            if (enemy != null && enemy.TryGetComponent(out EnemyHealth enemyHealth))
            {
                enemyHealth.OnDeath += HandleEnemyDeath;
            }
        }

        Debug.Log("Wave " + (currentWaveIndex + 1) + " started. Enemies: " + wave.count);
    }

    // Không polling — được gọi trực tiếp từ OnDeath event của EnemyHealth (Phase 3.5).
    private void HandleEnemyDeath()
    {
        aliveEnemyCount--;

        if (aliveEnemyCount <= 0)
        {
            Invoke(nameof(StartNextWave), delayBetweenWaves);
        }
    }
}