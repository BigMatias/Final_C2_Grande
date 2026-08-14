using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MyPoolManager : MonoBehaviour
{
    private PoolSettingsSO _settings;
    private Dictionary<Type, List<IPooleable>> _pooleablesDictionary = new Dictionary<Type, List<IPooleable>>();

    private void Awake()
    {
        SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
    }

    private void OnDestroy()
    {
        SceneManager.activeSceneChanged -= SceneManager_activeSceneChanged;
    }

    public void Init(PoolSettingsSO settings)
    {
        _settings = settings;
        InitializePool();
    }

    private void InitializePool()
    {
        foreach (PoolSetting pool in _settings.poolSettings)
        {
            IPooleable pooleable = pool.prefab.GetComponent<IPooleable>();

            Type type = pooleable.GetType();

            if (!_pooleablesDictionary.ContainsKey(type))
            {
                _pooleablesDictionary[type] = new List<IPooleable>();
                GameObject parent = new(pool.prefab.name);
                parent.transform.SetParent(transform);
                CreatePool(pool.prefab.gameObject, parent.transform, pool.quantity, _pooleablesDictionary[type]);
            }
        }
    }

    private void CreatePool(GameObject prefab, Transform parent, int quantity, List<IPooleable> list)
    {
        for (int i = 0; i < quantity; i++)
        {
            GameObject go = Instantiate(prefab, parent);
            IPooleable pooleable = go.GetComponent<IPooleable>();
            pooleable.DeActivate();
            list.Add(pooleable);
        }
    }

    public T GetInstanceFromPool<T>() where T : MonoBehaviour, IPooleable
    {
        Type type = typeof(T);

        if (!_pooleablesDictionary.ContainsKey(type))
            return null;

        List<IPooleable> list = _pooleablesDictionary[type];

        for (int i = 0; i < list.Count; i++)
        {
            if (!list[i].IsActive)
                return list[i] as T;
        }

        return null;
    }

    public List<T> GetPool<T>() where T : MonoBehaviour, IPooleable
    {
        Type type = typeof(T);

        if (!_pooleablesDictionary.ContainsKey(type))
            return null;

        List<IPooleable> list = _pooleablesDictionary[type];

        return list.Cast<T>().ToList();
    }

    public int GetPoolSize<T>() where T : MonoBehaviour, IPooleable
    {
        Type type = typeof(T);

        if (!_pooleablesDictionary.ContainsKey(type))
            return 0;

        return _pooleablesDictionary[type].Count;
    }

    private void SceneManager_activeSceneChanged(Scene arg0, Scene arg1)
    {
        foreach (KeyValuePair<Type, List<IPooleable>> item in _pooleablesDictionary)
            foreach (IPooleable pooleable in item.Value)
                pooleable.DeActivate();
    }
}