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

    [SerializeField] private Transform _pivot;
    [SerializeField] private ReactionBone[] _spineBones;
    [SerializeField] private float _hitBoneWeight = 1.5f;
    [SerializeField] private float _degreesPerDamage = 0.6f;
    [SerializeField] private float _maxDegrees = 35f;
    [SerializeField] private float _referenceLever = 0.5f;
    [SerializeField] private float _baseBend = 0.3f;
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
        if (info.Direction.sqrMagnitude < 0.0001f)
            return;

        Vector3 direction = info.Direction.normalized;
        float angle = Mathf.Min(info.Amount * _degreesPerDamage, _maxDegrees);
        Vector3 rotation = GetHitRotation(info.Point, direction) * angle;

        foreach (ReactionBone reactionBone in _spineBones)
        {
            if (reactionBone.bone != null)
                AddOffset(reactionBone.bone, rotation * reactionBone.weight);
        }

        if (info.HitBone != null)
            AddOffset(info.HitBone, rotation * _hitBoneWeight);
    }

    private Vector3 GetHitRotation(Vector3 hitPoint, Vector3 direction)
    {
        Vector3 bend = GetBendAxis(direction) * _baseBend;

        if (_pivot == null)
            return bend;

        Vector3 lever = hitPoint - _pivot.position;
        Vector3 torque = Vector3.Cross(lever, direction) / Mathf.Max(_referenceLever, 0.01f);

        return Vector3.ClampMagnitude(torque + bend, 1.5f);
    }

    private static Vector3 GetBendAxis(Vector3 direction)
    {
        Vector3 flat = direction;
        flat.y = 0f;

        if (flat.sqrMagnitude < 0.0001f)
            return Vector3.zero;

        return Vector3.Cross(Vector3.up, flat.normalized);
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