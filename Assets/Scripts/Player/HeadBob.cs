using UnityEngine;

public class HeadBob : MonoBehaviour
{
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private Animator _animator;
    [SerializeField] private int _locomotionLayer = 0;
    [SerializeField, Range(0f, 1f)] private float _phaseOffset = 0f;
    [SerializeField] private float _walkSpeedReference = 2.4f;
    [SerializeField] private float _maxIntensity = 1.5f;
    [SerializeField] private float _verticalAmplitude = 0.045f;
    [SerializeField] private float _horizontalAmplitude = 0.025f;
    [SerializeField] private float _rollAmplitude = 0.8f;
    [SerializeField] private float _smoothing = 6f;
    [SerializeField] private float _airborneThreshold = 1f;

    private Vector3 _baseLocalPosition;
    private Quaternion _baseLocalRotation;
    private float _intensity;

    private void Awake()
    {
        _baseLocalPosition = transform.localPosition;
        _baseLocalRotation = transform.localRotation;
    }

    private void LateUpdate()
    {
        Vector3 velocity = _rigidbody.linearVelocity;
        float planarSpeed = new Vector2(velocity.x, velocity.z).magnitude;
        bool airborne = Mathf.Abs(velocity.y) > _airborneThreshold;

        float targetIntensity = airborne ? 0f : Mathf.Clamp(planarSpeed / _walkSpeedReference, 0f, _maxIntensity);
        _intensity = Mathf.Lerp(_intensity, targetIntensity, 1f - Mathf.Exp(-_smoothing * Time.deltaTime));

        float cycle = GetAnimationCycle();
        float sway = Mathf.Sin(cycle);
        float step = Mathf.Abs(sway) - 1f;

        Vector3 offset = new Vector3(sway * _horizontalAmplitude, step * _verticalAmplitude, 0f) * _intensity;

        transform.localPosition = _baseLocalPosition + offset;
        transform.localRotation = _baseLocalRotation * Quaternion.Euler(0f, 0f, sway * _rollAmplitude * _intensity);
    }

    private float GetAnimationCycle()
    {
        AnimatorStateInfo state = _animator.GetCurrentAnimatorStateInfo(_locomotionLayer);
        float normalized = Mathf.Repeat(state.normalizedTime + _phaseOffset, 1f);
        return normalized * Mathf.PI * 2f;
    }
}