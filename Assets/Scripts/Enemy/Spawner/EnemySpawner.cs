using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private PoolServiceSO _pool;
    [SerializeField] private Transform _target;
    [SerializeField] private Transform[] _spawnPoints;
    [SerializeField] private EnemyAI[] _enemyPrefabs;
    [SerializeField] private float _spawnInterval = 2f;
    [SerializeField] private int _maxAlive = 12;
    [SerializeField] private float _navMeshSampleRadius = 2f;

    private readonly HashSet<EnemyAI> _alive = new HashSet<EnemyAI>();
    private readonly HashSet<EnemyHitReceiver> _tracked = new HashSet<EnemyHitReceiver>();
    private float _nextSpawnTime;

    public int AliveCount => _alive.Count;

    private void Update()
    {
        if (Time.time < _nextSpawnTime || _alive.Count >= _maxAlive)
            return;

        _nextSpawnTime = Time.time + _spawnInterval;
        Spawn();
    }

    private void Spawn()
    {
        if (_spawnPoints.Length == 0 || _enemyPrefabs.Length == 0)
            return;

        Transform point = _spawnPoints[Random.Range(0, _spawnPoints.Length)];

        if (!NavMesh.SamplePosition(point.position, out NavMeshHit hit, _navMeshSampleRadius, NavMesh.AllAreas))
            return;

        EnemyAI prefab = _enemyPrefabs[Random.Range(0, _enemyPrefabs.Length)];
        EnemyAI enemy = _pool.Spawn(prefab, hit.position, point.rotation);
        enemy.Initialize(_target);

        _alive.Add(enemy);
        Track(enemy);
    }

    private void Track(EnemyAI enemy)
    {
        if (!enemy.TryGetComponent(out EnemyHitReceiver receiver))
            return;

        if (_tracked.Add(receiver))
            receiver.OnDied += _ => _alive.Remove(enemy);
    }
}