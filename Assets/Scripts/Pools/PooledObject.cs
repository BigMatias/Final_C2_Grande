using System.Collections;
using UnityEngine;

public class PooledObject : MonoBehaviour
{
    private PoolServiceSO _owner;
    private IPoolable[] _poolables;
    private Coroutine _releaseRoutine;
    private bool _isInPool;

    public GameObject Prefab { get; private set; }

    public void Bind(PoolServiceSO owner, GameObject prefab)
    {
        _owner = owner;
        Prefab = prefab;
        _poolables = GetComponentsInChildren<IPoolable>(true);
        _isInPool = true;
    }

    public void NotifySpawned()
    {
        _isInPool = false;

        foreach (IPoolable poolable in _poolables)
            poolable.OnSpawnedFromPool();
    }

    public void NotifyReleased()
    {
        _isInPool = true;

        if (_releaseRoutine != null)
        {
            StopCoroutine(_releaseRoutine);
            _releaseRoutine = null;
        }

        foreach (IPoolable poolable in _poolables)
            poolable.OnReturnedToPool();
    }

    public void Release()
    {
        if (_isInPool)
            return;

        if (_owner != null)
            _owner.Release(this);
        else
            Destroy(gameObject);
    }

    public void ReleaseAfter(float delay)
    {
        if (_isInPool)
            return;

        if (_releaseRoutine != null)
            StopCoroutine(_releaseRoutine);

        _releaseRoutine = StartCoroutine(ReleaseRoutine(delay));
    }

    private IEnumerator ReleaseRoutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        _releaseRoutine = null;
        Release();
    }

    public static void Despawn(GameObject target)
    {
        if (target.TryGetComponent(out PooledObject pooled))
            pooled.Release();
        else
            Destroy(target);
    }

    public static void Despawn(GameObject target, float delay)
    {
        if (target.TryGetComponent(out PooledObject pooled))
            pooled.ReleaseAfter(delay);
        else
            Destroy(target, delay);
    }
}