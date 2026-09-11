using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WaveFormation
{
    Grid,
    DiagonalShower,
    OrbitGrowing,
    ApproachGrowing,
    ZigzagLines,
    Boss
}

[Serializable]
public class WaveDefinition
{
    public string waveName = "Wave";
    public WaveFormation formation = WaveFormation.Grid;
    public GameObject enemyPrefab;

    [Header("Color")]
    public bool useRandomColor = true;
    public ElementColor color = ElementColor.BLUE; // dùng khi useRandomColor = false

    [Header("Movement Override (áp dụng qua EnemyController.ConfigureMovement)")]
    public MovementPattern movementPattern = MovementPattern.LinearDown;
    public float moveSpeed = 4f;

    [Header("Shooting")]
    [Range(0f, 1f)] public float shooterChance = 0f; // % số quái trong wave này được phép bắn

    [Header("Grid")]
    public int rows = 5;
    public int columns = 8;
    public float gridSpacingX = 1.5f;
    public float gridSpacingY = 1.2f;

    [Header("Diagonal Shower")]
    public int showerCount = 20;
    public float showerSpawnInterval = 0.2f;

    [Header("Growing (Orbit / Approach)")]
    public int startCount = 5;
    public int maxCount = 25;
    public float growInterval = 3f;

    [Header("Entry Effect (khuyến nghị chỉ bật cho Grid)")]
    public bool useEntryEffect = false;
    public float entryDropHeight = 3f;
    public float entryStaggerDelay = 0.15f;
}

public class WaveManager : MonoBehaviour
{
    [SerializeField] private EnemySpawner enemySpawner;
    [SerializeField] private List<WaveDefinition> waves = new List<WaveDefinition>();

    private int currentWaveIndex = -1;
    private int aliveCount;

    private void Start()
    {
        StartNextWave();
    }

    private void StartNextWave()
    {
        currentWaveIndex++;

        if (currentWaveIndex >= waves.Count)
        {
            Debug.Log("Tất cả wave đã hoàn thành!");
            return;
        }

        WaveDefinition wave = waves[currentWaveIndex];
        Debug.Log("Bắt đầu " + wave.waveName);

        StartCoroutine(RunWave(wave));
    }

    private IEnumerator RunWave(WaveDefinition wave)
    {
        switch (wave.formation)
        {
            case WaveFormation.Grid:
                yield return StartCoroutine(SpawnGrid(wave));
                break;

            case WaveFormation.DiagonalShower:
                yield return StartCoroutine(SpawnDiagonalShower(wave));
                break;

            case WaveFormation.OrbitGrowing:
            case WaveFormation.ApproachGrowing:
                yield return StartCoroutine(SpawnGrowing(wave));
                break;

            case WaveFormation.ZigzagLines:
                yield return StartCoroutine(SpawnZigzagLines(wave));
                break;

            case WaveFormation.Boss:
                Debug.LogWarning(wave.waveName + ": Boss chưa được cài đặt (Phase 7).");
                StartNextWave();
                break;
        }
    }

    private IEnumerator SpawnGrid(WaveDefinition wave)
    {
        float startX = -(wave.columns - 1) * wave.gridSpacingX / 2f;

        for (int row = 0; row < wave.rows; row++)
        {
            ElementColor rowColor = wave.useRandomColor ? GetRandomColor() : wave.color;

            for (int col = 0; col < wave.columns; col++)
            {
                Vector3 targetPos = new Vector3(
                    startX + col * wave.gridSpacingX,
                    enemySpawner.SpawnY + row * wave.gridSpacingY,
                    0f
                );

                SpawnEnemyInstance(wave.enemyPrefab, targetPos, rowColor, wave);

                if (wave.entryStaggerDelay > 0f)
                {
                    yield return new WaitForSeconds(wave.entryStaggerDelay);
                }
            }
        }
    }

    private IEnumerator SpawnDiagonalShower(WaveDefinition wave)
    {
        for (int i = 0; i < wave.showerCount; i++)
        {
            float t = (float)i / wave.showerCount;
            float x = Mathf.Lerp(enemySpawner.MinX, enemySpawner.MinX + 3f, t % 1f);
            Vector3 pos = new Vector3(x, enemySpawner.SpawnY, 0f);
            ElementColor spawnColor = wave.useRandomColor ? GetRandomColor() : wave.color;

            SpawnEnemyInstance(wave.enemyPrefab, pos, spawnColor, wave);

            yield return new WaitForSeconds(wave.showerSpawnInterval);
        }
    }

    private IEnumerator SpawnGrowing(WaveDefinition wave)
    {
        int spawned = 0;

        while (spawned < wave.maxCount)
        {
            int batchTarget = Mathf.Min(wave.startCount + spawned, wave.maxCount);

            while (spawned < batchTarget)
            {
                float randomX = UnityEngine.Random.Range(enemySpawner.MinX, enemySpawner.MaxX);
                Vector3 pos = new Vector3(randomX, enemySpawner.SpawnY, 0f);
                ElementColor spawnColor = wave.useRandomColor ? GetRandomColor() : wave.color;

                SpawnEnemyInstance(wave.enemyPrefab, pos, spawnColor, wave);
                spawned++;
            }

            yield return new WaitForSeconds(wave.growInterval);
        }
    }

    private IEnumerator SpawnZigzagLines(WaveDefinition wave)
    {
        int linesSpawned = 0;

        while (linesSpawned < wave.maxCount)
        {
            bool fromLeft = linesSpawned % 2 == 0;
            float startX = fromLeft ? enemySpawner.MinX : enemySpawner.MaxX;
            ElementColor lineColor = wave.useRandomColor ? GetRandomColor() : wave.color;

            for (int col = 0; col < wave.columns; col++)
            {
                Vector3 pos = new Vector3(startX, enemySpawner.SpawnY - col * wave.gridSpacingY, 0f);
                SpawnEnemyInstance(wave.enemyPrefab, pos, lineColor, wave);

                if (wave.entryStaggerDelay > 0f)
                {
                    yield return new WaitForSeconds(wave.entryStaggerDelay);
                }
            }

            linesSpawned++;
            yield return new WaitForSeconds(wave.growInterval);
        }
    }

    private void SpawnEnemyInstance(GameObject prefab, Vector3 targetPosition, ElementColor color, WaveDefinition wave)
    {
        Vector3 spawnPosition = wave.useEntryEffect
            ? targetPosition + Vector3.up * wave.entryDropHeight
            : targetPosition;

        GameObject instance = enemySpawner.SpawnAt(prefab, spawnPosition, color);
        if (instance == null) return;

        if (wave.useEntryEffect && instance.TryGetComponent(out EnemyFormationEntry entry))
        {
            entry.BeginEntry(spawnPosition, targetPosition);
        }

        if (instance.TryGetComponent(out EnemyController enemyController))
        {
            enemyController.ConfigureMovement(wave.movementPattern, wave.moveSpeed);
        }

        if (instance.TryGetComponent(out EnemyShooting shooting))
        {
            shooting.enabled = UnityEngine.Random.value < wave.shooterChance;
        }

        aliveCount++;

        if (instance.TryGetComponent(out EnemyHealth enemyHealth))
        {
            enemyHealth.OnDeath += HandleEnemyCleared;
        }
        else if (instance.TryGetComponent(out MeteorHealth meteorHealth))
        {
            meteorHealth.OnDeath += HandleEnemyCleared;
        }
    }

    private ElementColor GetRandomColor()
    {
        return UnityEngine.Random.value < 0.5f ? ElementColor.BLUE : ElementColor.RED;
    }

    private void HandleEnemyCleared()
    {
        aliveCount--;

        if (aliveCount <= 0)
        {
            StartNextWave();
        }
    }
}