using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

[CreateAssetMenu(fileName = "PoolService", menuName = "Pooling/Pool Service")]
public class PoolServiceSO : ScriptableObject
{
    private class PoolEntry
    {
        public ObjectPool<PooledObject> Pool;
        public Transform ActiveContainer;
        public Transform InactiveContainer;
    }

    [SerializeField] private int _defaultCapacity = 16;
    [SerializeField] private int _maxSize = 512;

    private readonly Dictionary<GameObject, PoolEntry> _entries = new Dictionary<GameObject, PoolEntry>();
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
        foreach (PoolEntry entry in _entries.Values)
            entry.Pool.Clear();

        _entries.Clear();
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

    public void Prewarm<T>(T prefab, int count) where T : Component
    {
        Prewarm(prefab.gameObject, count);
    }

    public void Prewarm(GameObject prefab, int count)
    {
        PoolEntry entry = GetEntry(prefab);
        int missing = count - entry.Pool.CountInactive;

        for (int i = 0; i < missing; i++)
            entry.Pool.Release(CreateInstance(prefab, entry));
    }

    public void Release(PooledObject instance)
    {
        if (instance == null)
            return;

        if (!_entries.TryGetValue(instance.Prefab, out PoolEntry entry))
        {
            Destroy(instance.gameObject);
            return;
        }

        entry.Pool.Release(instance);
    }

    private PooledObject SpawnInternal(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent)
    {
        PoolEntry entry = GetEntry(prefab);
        PooledObject instance = entry.Pool.Get();
        Transform instanceTransform = instance.transform;

        instanceTransform.SetParent(parent != null ? parent : entry.ActiveContainer, false);
        instanceTransform.SetPositionAndRotation(position, rotation);

        instance.gameObject.SetActive(true);
        instance.NotifySpawned();

        return instance;
    }

    private PoolEntry GetEntry(GameObject prefab)
    {
        if (_entries.TryGetValue(prefab, out PoolEntry entry))
            return entry;

        entry = new PoolEntry
        {
            ActiveContainer = CreateContainer(prefab.name, GetActiveRoot()),
            InactiveContainer = CreateContainer(prefab.name, GetInactiveRoot())
        };

        entry.Pool = new ObjectPool<PooledObject>(
            () => CreateInstance(prefab, entry),
            null,
            instance => OnReleaseInstance(instance, entry),
            OnDestroyInstance,
            false,
            _defaultCapacity,
            _maxSize);

        _entries.Add(prefab, entry);
        return entry;
    }

    private PooledObject CreateInstance(GameObject prefab, PoolEntry entry)
    {
        GameObject instance = Instantiate(prefab, entry.InactiveContainer);
        instance.name = prefab.name;
        instance.SetActive(false);

        if (!instance.TryGetComponent(out PooledObject pooled))
            pooled = instance.AddComponent<PooledObject>();

        pooled.Bind(this, prefab);
        return pooled;
    }

    private void OnReleaseInstance(PooledObject instance, PoolEntry entry)
    {
        instance.NotifyReleased();
        instance.gameObject.SetActive(false);
        instance.transform.SetParent(entry.InactiveContainer, false);
    }

    private void OnDestroyInstance(PooledObject instance)
    {
        if (instance != null)
            Destroy(instance.gameObject);
    }

    private static Transform CreateContainer(string containerName, Transform parent)
    {
        Transform container = new GameObject(containerName).transform;
        container.SetParent(parent, false);
        return container;
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