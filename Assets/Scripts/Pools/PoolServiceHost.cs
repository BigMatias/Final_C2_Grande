using UnityEngine;

public class PoolServiceHost : MonoBehaviour
{
    [SerializeField] private PoolServiceSO _poolService;

    private void Awake()
    {
        Transform activeRoot = new GameObject("Pool_Active").transform;
        activeRoot.SetParent(transform, false);

        GameObject inactiveContainer = new GameObject("Pool_Inactive");
        inactiveContainer.SetActive(false);
        inactiveContainer.transform.SetParent(transform, false);

        _poolService.Initialize(activeRoot, inactiveContainer.transform);
    }

    private void OnDestroy()
    {
        _poolService.Clear();
    }
}