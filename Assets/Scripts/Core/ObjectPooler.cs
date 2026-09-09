using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    public static ObjectPooler Instance { get; private set; }

    private readonly Dictionary<GameObject, Queue<GameObject>> pools = new Dictionary<GameObject, Queue<GameObject>>();
    private readonly Dictionary<GameObject, GameObject> instanceToPrefab = new Dictionary<GameObject, GameObject>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        if (prefab == null)
        {
            Debug.LogWarning("ObjectPooler: prefab is null.");
            return null;
        }

        GameObject instance;

        if (pools.TryGetValue(prefab, out Queue<GameObject> pool) && pool.Count > 0)
        {
            instance = pool.Dequeue();
            instance.transform.SetPositionAndRotation(position, rotation);
            instance.SetActive(true);
        }
        else
        {
            instance = Instantiate(prefab, position, rotation);
            instanceToPrefab[instance] = prefab;
        }

        return instance;
    }

    public void Despawn(GameObject instance)
    {
        if (instance == null) return;

        if (!instanceToPrefab.TryGetValue(instance, out GameObject prefab))
        {
            // Object không do pooler tạo (VD: đặt tay sẵn trong scene) -> huỷ bình thường.
            Destroy(instance);
            return;
        }

        instance.SetActive(false);

        if (!pools.TryGetValue(prefab, out Queue<GameObject> pool))
        {
            pool = new Queue<GameObject>();
            pools[prefab] = pool;
        }

        pool.Enqueue(instance);
    }
}