using System;
using UnityEngine;

public class EnemyHitReceiver : MonoBehaviour
{
    [SerializeField] private float _headHealth = 40f;
    [SerializeField] private float _killDamage = 99999f;

    public event Action<DamageInfo> OnHit;
    public event Action<DamageInfo> OnDecapitated;
    public event Action<DamageInfo> OnDied;

    public DamageInfo LastHit { get; private set; }
    public bool IsDecapitated { get; private set; }
    public bool IsDead { get; private set; }

    private HealthSystem _healthSystem;
    private IDamageable _damageable;
    private float _currentHeadHealth;

    private void Awake()
    {
        _healthSystem = GetComponent<HealthSystem>();
        _damageable = GetComponent<IDamageable>();
    }

    private void OnEnable()
    {
        IsDead = false;
        IsDecapitated = false;
        _currentHeadHealth = _headHealth;
        _healthSystem.onDie += HandleDie;
    }

    private void OnDisable()
    {
        _healthSystem.onDie -= HandleDie;
    }

    public void ReceiveHit(DamageInfo info)
    {
        if (IsDead)
        {
            PushCorpse(info);
            return;
        }

        LastHit = info;
        OnHit?.Invoke(info);

        if (info.BodyPart == BodyPart.Head)
        {
            _currentHeadHealth -= info.Amount;

            if (_currentHeadHealth <= 0f)
            {
                IsDecapitated = true;
                OnDecapitated?.Invoke(info);
                ApplyDamage(_killDamage);
                return;
            }
        }

        ApplyDamage(info.Amount);
    }

    private void ApplyDamage(float amount)
    {
        if (_damageable != null)
            _damageable.TakeDamage(amount);
        else
            _healthSystem.TakeDamage(amount);
    }

    private void HandleDie()
    {
        if (IsDead)
            return;

        IsDead = true;
        OnDied?.Invoke(LastHit);
    }

    private void PushCorpse(DamageInfo info)
    {
        if (info.HitBody == null || info.HitBody.isKinematic)
            return;

        info.HitBody.AddForceAtPosition(info.Direction * info.Force, info.Point, ForceMode.Impulse);
    }
}