using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Hitbox : MonoBehaviour, IHittable
{
    [SerializeField] private BodyPart _bodyPart = BodyPart.Torso;
    [SerializeField] private float _damageMultiplier = 1f;

    private EnemyHitReceiver _receiver;
    private Rigidbody _rigidbody;

    public BodyPart BodyPart => _bodyPart;

    private void Awake()
    {
        _receiver = GetComponentInParent<EnemyHitReceiver>();
        _rigidbody = GetComponent<Rigidbody>();
    }

    public void ReceiveHit(DamageInfo info)
    {
        if (_receiver == null)
            return;

        info.Amount *= _damageMultiplier;
        info.BodyPart = _bodyPart;
        info.HitBone = transform;
        info.HitBody = _rigidbody;

        _receiver.ReceiveHit(info);
    }
}