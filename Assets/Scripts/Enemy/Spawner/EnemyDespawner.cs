using UnityEngine;

public class EnemyDespawner : MonoBehaviour
{
    [SerializeField] private float _corpseLifetime = 10f;

    private EnemyHitReceiver _receiver;

    private void Awake()
    {
        _receiver = GetComponent<EnemyHitReceiver>();
    }

    private void OnEnable()
    {
        _receiver.OnDied += HandleDied;
    }

    private void OnDisable()
    {
        _receiver.OnDied -= HandleDied;
    }

    private void HandleDied(DamageInfo lastHit)
    {
        PooledObject.Despawn(gameObject, _corpseLifetime);
    }
}