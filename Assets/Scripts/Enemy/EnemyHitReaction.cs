using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHitReaction : MonoBehaviour
{
    [Serializable]
    private struct ReactionBone
    {
        public Transform bone;
        public float weight;
    }

    [SerializeField] private ReactionBone[] _spineBones;
    [SerializeField] private float _hitBoneWeight = 1.5f;
    [SerializeField] private float _degreesPerDamage = 0.6f;
    [SerializeField] private float _maxDegrees = 35f;
    [SerializeField] private float _recoverySpeed = 8f;

    private readonly Dictionary<Transform, Vector3> _offsets = new Dictionary<Transform, Vector3>();
    private readonly List<Transform> _activeBones = new List<Transform>();
    private EnemyHitReceiver _receiver;

    private void Awake()
    {
        _receiver = GetComponent<EnemyHitReceiver>();
    }

    private void OnEnable()
    {
        _offsets.Clear();
        _receiver.OnHit += HandleHit;
    }

    private void OnDisable()
    {
        _receiver.OnHit -= HandleHit;
    }

    private void HandleHit(DamageInfo info)
    {
        Vector3 direction = info.Direction;
        direction.y = 0f;

        if (direction.sqrMagnitude < 0.0001f)
            return;

        Vector3 axis = Vector3.Cross(Vector3.up, direction.normalized);
        float angle = Mathf.Min(info.Amount * _degreesPerDamage, _maxDegrees);

        foreach (ReactionBone reactionBone in _spineBones)
        {
            if (reactionBone.bone != null)
                AddOffset(reactionBone.bone, axis * angle * reactionBone.weight);
        }

        if (info.HitBone != null)
            AddOffset(info.HitBone, axis * angle * _hitBoneWeight);
    }

    private void AddOffset(Transform bone, Vector3 rotation)
    {
        _offsets.TryGetValue(bone, out Vector3 current);
        _offsets[bone] = Vector3.ClampMagnitude(current + rotation, _maxDegrees);
    }

    private void LateUpdate()
    {
        if (_offsets.Count == 0)
            return;

        if (_receiver.IsDead)
        {
            _offsets.Clear();
            return;
        }

        float decay = Mathf.Exp(-_recoverySpeed * Time.deltaTime);

        _activeBones.Clear();
        _activeBones.AddRange(_offsets.Keys);

        foreach (Transform bone in _activeBones)
        {
            Vector3 offset = _offsets[bone];
            float angle = offset.magnitude;

            if (angle < 0.05f)
            {
                _offsets.Remove(bone);
                continue;
            }

            bone.rotation = Quaternion.AngleAxis(angle, offset / angle) * bone.rotation;
            _offsets[bone] = offset * decay;
        }
    }
}