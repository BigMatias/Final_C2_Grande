using System;
using UnityEngine;

public class PoolServiceHost : MonoBehaviour
{
    [Serializable]
    private struct PrewarmEntry
    {
        public GameObject prefab;
        public int count;
    }

    [SerializeField] private PoolServiceSO _poolService;
    [SerializeField] private PrewarmEntry[] _prewarm;

    private void Awake()
    {
        Transform activeRoot = new GameObject("Pool_Active").transform;
        activeRoot.SetParent(transform, false);

        GameObject inactiveContainer = new GameObject("Pool_Inactive");
        inactiveContainer.SetActive(false);
        inactiveContainer.transform.SetParent(transform, false);

        _poolService.Initialize(activeRoot, inactiveContainer.transform);

        foreach (PrewarmEntry entry in _prewarm)
        {
            if (entry.prefab != null && entry.count > 0)
                _poolService.Prewarm(entry.prefab, entry.count);
        }
    }

    private void OnDestroy()
    {
        _poolService.Clear();
    }
}