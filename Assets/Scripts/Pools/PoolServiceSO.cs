using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

[CreateAssetMenu(fileName = "PoolService", menuName = "Pooling/Pool Service")]
public class PoolServiceSO : ScriptableObject
{
    [SerializeField] private int _defaultCapacity = 16;
    [SerializeField] private int _maxSize = 512;

    private readonly Dictionary<GameObject, ObjectPool<PooledObject>> _pools = new Dictionary<GameObject, ObjectPool<PooledObject>>();
    private Transform _activeRoot;
    private Transform _inactiveRoot;

    public void Initialize(Transform activeRoot, Transform inactiveRoot)
    {
        Clear();
        _activeRoot = activeRoot;
        _inactiveRoot = inactiveRoot;
    }

    public void Clear()
    {
        foreach (ObjectPool<PooledObject> pool in _pools.Values)
            pool.Clear();

        _pools.Clear();
        _activeRoot = null;
        _inactiveRoot = null;
    }

    public T Spawn<T>(T prefab, Vector3 position, Quaternion rotation, Transform parent = null) where T : Component
    {
        PooledObject instance = SpawnInternal(prefab.gameObject, position, rotation, parent);
        return instance.GetComponent<T>();
    }

    public GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent = null)
    {
        return SpawnInternal(prefab, position, rotation, parent).gameObject;
    }

    public void Prewarm(GameObject prefab, int count)
    {
        ObjectPool<PooledObject> pool = GetPool(prefab);
        List<PooledObject> instances = new List<PooledObject>(count);

        for (int i = 0; i < count; i++)
            instances.Add(pool.Get());

        foreach (PooledObject instance in instances)
            pool.Release(instance);
    }

    public void Release(PooledObject instance)
    {
        if (instance == null)
            return;

        if (!_pools.TryGetValue(instance.Prefab, out ObjectPool<PooledObject> pool))
        {
            Destroy(instance.gameObject);
            return;
        }

        pool.Release(instance);
    }

    private PooledObject SpawnInternal(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent)
    {
        PooledObject instance = GetPool(prefab).Get();
        Transform instanceTransform = instance.transform;

        instanceTransform.SetParent(parent != null ? parent : GetActiveRoot(), false);
        instanceTransform.SetPositionAndRotation(position, rotation);

        instance.gameObject.SetActive(true);
        instance.NotifySpawned();

        return instance;
    }

    private ObjectPool<PooledObject> GetPool(GameObject prefab)
    {
        if (_pools.TryGetValue(prefab, out ObjectPool<PooledObject> pool))
            return pool;

        pool = new ObjectPool<PooledObject>(
            () => CreateInstance(prefab),
            null,
            OnReleaseInstance,
            OnDestroyInstance,
            false,
            _defaultCapacity,
            _maxSize);

        _pools.Add(prefab, pool);
        return pool;
    }

    private PooledObject CreateInstance(GameObject prefab)
    {
        GameObject instance = Instantiate(prefab, GetInactiveRoot());
        instance.SetActive(false);

        if (!instance.TryGetComponent(out PooledObject pooled))
            pooled = instance.AddComponent<PooledObject>();

        pooled.Bind(this, prefab);
        return pooled;
    }

    private void OnReleaseInstance(PooledObject instance)
    {
        instance.NotifyReleased();
        instance.gameObject.SetActive(false);
        instance.transform.SetParent(GetInactiveRoot(), false);
    }

    private void OnDestroyInstance(PooledObject instance)
    {
        if (instance != null)
            Destroy(instance.gameObject);
    }

    private Transform GetActiveRoot()
    {
        if (_activeRoot == null)
            _activeRoot = new GameObject("Pool_Active").transform;

        return _activeRoot;
    }

    private Transform GetInactiveRoot()
    {
        if (_inactiveRoot == null)
        {
            GameObject container = new GameObject("Pool_Inactive");
            container.SetActive(false);
            _inactiveRoot = container.transform;
        }

        return _inactiveRoot;
    }
}