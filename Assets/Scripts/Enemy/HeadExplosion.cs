using UnityEngine;

public class HeadExplosion : MonoBehaviour
{
    [Header("Bones")]
    [SerializeField] private Transform _headBone;
    [SerializeField] private Transform _neckBone;

    [Header("Mesh")]
    [SerializeField] private SkinnedMeshRenderer[] _renderers;
    [SerializeField, Range(0.1f, 1f)] private float _headWeightThreshold = 0.5f;

    [Header("Pooling")]
    [SerializeField] private PoolServiceSO _pool;

    [Header("Effects")]
    [SerializeField] private ParticleSystem _explosionPrefab;
    [SerializeField] private ParticleSystem _neckBleedPrefab;
    [SerializeField] private GameObject _neckStump;

    [Header("Gibs")]
    [SerializeField] private GameObject[] _gibPrefabs;
    [SerializeField] private int _gibCount = 6;
    [SerializeField] private float _gibForce = 4f;
    [SerializeField] private float _gibLifetime = 8f;

    private EnemyHitReceiver _receiver;
    private Collider[] _headColliders;
    private Mesh[] _originalMeshes;
    private ParticleSystem _neckBleedInstance;
    private bool _exploded;

    private void Awake()
    {
        _receiver = GetComponent<EnemyHitReceiver>();
        _headColliders = _headBone.GetComponentsInChildren<Collider>(true);

        _originalMeshes = new Mesh[_renderers.Length];
        for (int i = 0; i < _renderers.Length; i++)
            _originalMeshes[i] = _renderers[i].sharedMesh;
    }

    private void OnEnable()
    {
        Restore();
        _receiver.OnDecapitated += Explode;
    }

    private void OnDisable()
    {
        _receiver.OnDecapitated -= Explode;
    }

    private void Explode(DamageInfo info)
    {
        if (_exploded)
            return;

        _exploded = true;

        Vector3 headPosition = _headBone.position;

        SwapToHeadlessMeshes();

        foreach (Collider headCollider in _headColliders)
            headCollider.enabled = false;

        if (_neckStump != null)
            _neckStump.SetActive(true);

        SpawnBurst(headPosition, info.Direction);
        StartNeckBleed();
        SpawnGibs(headPosition, info.Direction);
    }

    private void SwapToHeadlessMeshes()
    {
        foreach (SkinnedMeshRenderer meshRenderer in _renderers)
            meshRenderer.sharedMesh = HeadlessMeshCache.Get(meshRenderer, _headBone, _headWeightThreshold);
    }

    private void SpawnBurst(Vector3 position, Vector3 direction)
    {
        if (_pool == null || _explosionPrefab == null)
            return;

        Quaternion rotation = direction.sqrMagnitude > 0.0001f ? Quaternion.LookRotation(direction) : Quaternion.identity;
        ParticleSystem burst = _pool.Spawn(_explosionPrefab, position, rotation);
        burst.Play(true);

        ParticleSystem.MainModule main = burst.main;
        PooledObject.Despawn(burst.gameObject, main.duration + main.startLifetime.constantMax);
    }

    private void StartNeckBleed()
    {
        if (_pool == null || _neckBleedPrefab == null || _neckBone == null)
            return;

        _neckBleedInstance = _pool.Spawn(_neckBleedPrefab, _neckBone.position, _neckBone.rotation, _neckBone);
        _neckBleedInstance.Play(true);
    }

    private void SpawnGibs(Vector3 position, Vector3 direction)
    {
        if (_pool == null || _gibPrefabs == null || _gibPrefabs.Length == 0)
            return;

        for (int i = 0; i < _gibCount; i++)
        {
            GameObject prefab = _gibPrefabs[Random.Range(0, _gibPrefabs.Length)];
            GameObject gib = _pool.Spawn(prefab, position + Random.insideUnitSphere * 0.1f, Random.rotation);

            if (gib.TryGetComponent(out Rigidbody body))
            {
                body.linearVelocity = Vector3.zero;
                body.angularVelocity = Vector3.zero;
                Vector3 launch = (direction * 0.5f + Random.onUnitSphere + Vector3.up * 0.5f).normalized;
                body.AddForce(launch * _gibForce, ForceMode.Impulse);
                body.AddTorque(Random.insideUnitSphere * _gibForce, ForceMode.Impulse);
            }

            PooledObject.Despawn(gib, _gibLifetime);
        }
    }

    private void Restore()
    {
        _exploded = false;

        if (_originalMeshes != null)
        {
            for (int i = 0; i < _renderers.Length; i++)
                _renderers[i].sharedMesh = _originalMeshes[i];
        }

        foreach (Collider headCollider in _headColliders)
            headCollider.enabled = true;

        if (_neckStump != null)
            _neckStump.SetActive(false);

        if (_neckBleedInstance != null)
        {
            PooledObject.Despawn(_neckBleedInstance.gameObject);
            _neckBleedInstance = null;
        }
    }
}