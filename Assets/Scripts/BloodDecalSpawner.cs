using UnityEngine;

public class BloodDecalSpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PoolServiceSO _pool;
    [SerializeField] private BloodDecal _decalPrefab;
    [SerializeField] private LayerMask _surfaceLayers;

    [Header("Atlas")]
    [SerializeField] private int _atlasColumns = 2;
    [SerializeField] private int _atlasRows = 2;

    [Header("Hits")]
    [SerializeField] private float _exitDistance = 3f;
    [SerializeField] private Vector2 _hitSizeRange = new Vector2(0.35f, 0.7f);
    [SerializeField, Range(0f, 1f)] private float _floorChance = 0.5f;
    [SerializeField] private Vector2 _floorSizeRange = new Vector2(0.25f, 0.5f);

    [Header("Decapitation")]
    [SerializeField] private Vector2 _headshotSizeRange = new Vector2(0.9f, 1.5f);
    [SerializeField] private int _headshotSplatCount = 3;
    [SerializeField] private float _headshotSpread = 0.35f;

    [Header("Projection")]
    [SerializeField] private float _floorRayDistance = 3f;
    [SerializeField] private float _depth = 0.5f;

    private EnemyHitReceiver _receiver;

    private void Awake()
    {
        _receiver = GetComponent<EnemyHitReceiver>();
    }

    private void OnEnable()
    {
        _receiver.OnHit += HandleHit;
        _receiver.OnDecapitated += HandleDecapitated;
    }

    private void OnDisable()
    {
        _receiver.OnHit -= HandleHit;
        _receiver.OnDecapitated -= HandleDecapitated;
    }

    private void HandleHit(DamageInfo info)
    {
        Vector3 direction = info.Direction.normalized;

        TrySplat(info.Point, direction, _exitDistance, _hitSizeRange);

        if (Random.value < _floorChance)
            TrySplat(info.Point, Vector3.down, _floorRayDistance, _floorSizeRange);
    }

    private void HandleDecapitated(DamageInfo info)
    {
        Vector3 direction = info.Direction.normalized;

        for (int i = 0; i < _headshotSplatCount; i++)
        {
            Vector3 spreadDirection = (direction + Random.insideUnitSphere * _headshotSpread).normalized;
            TrySplat(info.Point, spreadDirection, _exitDistance * 1.5f, _headshotSizeRange);
        }

        TrySplat(info.Point, Vector3.down, _floorRayDistance, _headshotSizeRange);
    }

    private void TrySplat(Vector3 origin, Vector3 direction, float distance, Vector2 sizeRange)
    {
        if (_pool == null || _decalPrefab == null)
            return;

        if (!Physics.Raycast(origin, direction, out RaycastHit hit, distance, _surfaceLayers, QueryTriggerInteraction.Ignore))
            return;

        int variant = Random.Range(0, _atlasColumns * _atlasRows);
        float size = Random.Range(sizeRange.x, sizeRange.y);

        BloodDecal decal = _pool.Spawn(_decalPrefab, hit.point, Quaternion.identity);
        decal.Place(hit.point, hit.normal, size, _depth, variant, _atlasColumns, _atlasRows);
    }
}