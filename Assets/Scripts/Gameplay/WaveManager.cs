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
    public enum BulletColorMode
    {
        SameAsBody,      // Đạn cùng màu thân — dễ đọc, dùng cho wave sớm
        OppositeOfBody,  // Đạn luôn khác màu thân — bắt buộc nhìn đạn, không nhìn thân
        IndependentRandom
    }

    public string waveName = "Wave";
    public WaveFormation formation = WaveFormation.Grid;
    public GameObject enemyPrefab;

    [Header("Color (thân)")]
    public bool useRandomColor = true;
    public ElementColor color = ElementColor.BLUE;

    [Header("Bullet Color (chỉ áp dụng nếu enemy có EnemyShooting)")]
    public BulletColorMode bulletColorMode = BulletColorMode.SameAsBody;

    [Header("Movement Override")]
    public MovementPattern movementPattern = MovementPattern.LinearDown;
    public float moveSpeed = 4f;

    [Header("Shooting")]
    [Range(0f, 1f)] public float shooterChance = 0f;

    [Header("Grid")]
    public int rows = 5;
    public int columns = 8;
    public float gridSpacingX = 1.5f;
    public float gridSpacingY = 1.2f;
    [Tooltip("Vị trí Y của hàng TRÊN CÙNG sau khi đã vào đội hình — phải nằm trong tầm nhìn camera (VD ~3.5), KHÔNG phải vùng spawn ngoài màn hình.")]
    public float formationTopY = 3.5f;

    [Header("Diagonal Shower")]
    public int showerCount = 20;
    public float showerSpawnInterval = 0.2f;

    [Header("Growing (Orbit / Approach)")]
    public int startCount = 5;
    public int maxCount = 25;
    public float growInterval = 3f;

    [Header("Zigzag Lines")]
    public int zigzagLineCount = 6;

    [Header("Entry Effect (khuyến nghị chỉ bật cho Grid)")]
    public bool useEntryEffect = false;
    public float entryDropHeight = 3f;
    public float entryStaggerDelay = 0.15f;
}

public class WaveManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private EnemySpawner enemySpawner;
    [SerializeField] private List<WaveDefinition> waves = new List<WaveDefinition>();
    [SerializeField] private float delayBetweenWaves = 2f;

    [Header("Power-Up Drop (khi wave clear)")]
    [SerializeField] private GameObject[] powerUpPrefabs;
    [SerializeField][Range(0f, 1f)] private float powerUpDropChance = 0.6f;

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

        if (wave.formation == WaveFormation.Boss)
        {
            Debug.LogWarning(wave.waveName + ": Boss chưa được cài đặt (Phase 7).");
            StartNextWave();
            return;
        }

        int totalCount = GetTotalEnemyCount(wave);

        if (totalCount <= 0)
        {
            Debug.LogWarning(wave.waveName + " có tổng số địch = 0, bỏ qua wave này.");
            StartNextWave();
            return;
        }

        // Chốt tổng số NGAY LẬP TỨC, trước khi spawn — tránh race condition
        // giữa lúc coroutine đang spawn dở và player giết nhanh hơn tốc độ spawn.
        aliveCount = totalCount;

        Debug.Log("Bắt đầu " + wave.waveName + " — tổng " + totalCount + " địch.");
        StartCoroutine(RunWave(wave));
    }

    private int GetTotalEnemyCount(WaveDefinition wave)
    {
        switch (wave.formation)
        {
            case WaveFormation.Grid: return wave.rows * wave.columns;
            case WaveFormation.DiagonalShower: return wave.showerCount;
            case WaveFormation.OrbitGrowing:
            case WaveFormation.ApproachGrowing: return wave.maxCount;
            case WaveFormation.ZigzagLines: return wave.zigzagLineCount * wave.columns;
            default: return 0;
        }
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
        }
    }

    private IEnumerator SpawnGrid(WaveDefinition wave)
    {
        float startX = -(wave.columns - 1) * wave.gridSpacingX / 2f;

        for (int row = 0; row < wave.rows; row++)
        {
            ElementColor rowColor = wave.useRandomColor ? GetRandomColor() : wave.color;
            // Hàng 0 = gần player nhất (dưới đội hình); hàng cuối = formationTopY (trên đội hình).
            float targetY = wave.formationTopY - (wave.rows - 1 - row) * wave.gridSpacingY;

            for (int col = 0; col < wave.columns; col++)
            {
                Vector3 targetPos = new Vector3(startX + col * wave.gridSpacingX, targetY, 0f);
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
        for (int line = 0; line < wave.zigzagLineCount; line++)
        {
            bool fromLeft = line % 2 == 0;
            float startX = fromLeft ? enemySpawner.MinX : enemySpawner.MaxX;
            ElementColor lineColor = wave.useRandomColor ? GetRandomColor() : wave.color;

            for (int col = 0; col < wave.columns; col++)
            {
                Vector3 pos = new Vector3(startX, enemySpawner.SpawnY - col * wave.gridSpacingY, 0f);
                SpawnEnemyInstance(wave.enemyPrefab, pos, lineColor, wave);
            }

            yield return new WaitForSeconds(wave.growInterval);
        }
    }

    private void SpawnEnemyInstance(GameObject prefab, Vector3 targetPosition, ElementColor color, WaveDefinition wave)
    {
        Vector3 spawnPosition = wave.useEntryEffect
            ? new Vector3(targetPosition.x, enemySpawner.SpawnY + wave.entryDropHeight, 0f)
            : targetPosition;

        GameObject instance = enemySpawner.SpawnAt(prefab, spawnPosition, color);

        if (instance == null)
        {
            aliveCount--; // spawn thất bại, không tính vào tổng nữa
            return;
        }

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

            if (shooting.enabled)
            {
                shooting.SetBulletColor(ResolveBulletColor(wave.bulletColorMode, color));
            }
        }

        if (instance.TryGetComponent(out EnemyHealth enemyHealth))
        {
            enemyHealth.OnDeath += HandleEnemyCleared;
        }
        else if (instance.TryGetComponent(out MeteorHealth meteorHealth))
        {
            meteorHealth.OnDeath += HandleEnemyCleared;
        }
    }

    private ElementColor ResolveBulletColor(WaveDefinition.BulletColorMode mode, ElementColor bodyColor)
    {
        switch (mode)
        {
            case WaveDefinition.BulletColorMode.OppositeOfBody:
                return bodyColor == ElementColor.BLUE ? ElementColor.RED : ElementColor.BLUE;
            case WaveDefinition.BulletColorMode.IndependentRandom:
                return GetRandomColor();
            default:
                return bodyColor;
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
            TryDropPowerUp();
            Invoke(nameof(StartNextWave), delayBetweenWaves);
        }
    }

    private void TryDropPowerUp()
    {
        if (powerUpPrefabs == null || powerUpPrefabs.Length == 0) return;
        if (UnityEngine.Random.value > powerUpDropChance) return;

        GameObject prefab = powerUpPrefabs[UnityEngine.Random.Range(0, powerUpPrefabs.Length)];
        Instantiate(prefab, new Vector3(0f, 3f, 0f), Quaternion.identity);

        Debug.Log("Power-up dropped after " + waves[currentWaveIndex].waveName);
    }
}