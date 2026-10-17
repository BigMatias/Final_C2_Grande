using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ProjectilePool : MonoBehaviour
{
    [SerializeField] private int _defaultCapacity = 32;
    [SerializeField] private int _maxSize = 256;

    private readonly Dictionary<Projectile, ObjectPool<Projectile>> _pools = new Dictionary<Projectile, ObjectPool<Projectile>>();
    private Transform _container;

    private void Awake()
    {
        _container = new GameObject("Projectiles").transform;
    }

    public Projectile Get(Projectile prefab)
    {
        if (!_pools.TryGetValue(prefab, out ObjectPool<Projectile> pool))
        {
            pool = CreatePool(prefab);
            _pools.Add(prefab, pool);
        }

        return pool.Get();
    }

    private ObjectPool<Projectile> CreatePool(Projectile prefab)
    {
        ObjectPool<Projectile> pool = null;

        pool = new ObjectPool<Projectile>(
            () =>
            {
                Projectile instance = Instantiate(prefab, _container);
                instance.SetRelease(released => pool.Release(released));
                return instance;
            },
            instance => instance.gameObject.SetActive(true),
            instance => instance.gameObject.SetActive(false),
            instance => Destroy(instance.gameObject),
            false,
            _defaultCapacity,
            _maxSize);

        return pool;
    }
}