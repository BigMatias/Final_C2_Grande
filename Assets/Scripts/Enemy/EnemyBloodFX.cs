using UnityEngine;

public class EnemyBloodFX : MonoBehaviour
{
    [SerializeField] private PoolServiceSO _pool;
    [SerializeField] private ParticleSystem _hitBloodPrefab;

    private EnemyHitReceiver _receiver;

    private void Awake()
    {
        _receiver = GetComponent<EnemyHitReceiver>();
    }

    private void OnEnable()
    {
        _receiver.OnHit += HandleHit;
    }

    private void OnDisable()
    {
        _receiver.OnHit -= HandleHit;
    }

    private void HandleHit(DamageInfo info)
    {
        if (_pool == null || _hitBloodPrefab == null)
            return;

        Quaternion rotation = info.Direction.sqrMagnitude > 0.0001f ? Quaternion.LookRotation(-info.Direction) : Quaternion.identity;
        ParticleSystem blood = _pool.Spawn(_hitBloodPrefab, info.Point, rotation);
        blood.Play(true);

        ParticleSystem.MainModule main = blood.main;
        PooledObject.Despawn(blood.gameObject, main.duration + main.startLifetime.constantMax);
    }
}