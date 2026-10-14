using UnityEngine;

public class CameraHeadFollow : MonoBehaviour
{
    [SerializeField] private Transform _eyesPoint;
    [SerializeField, Range(0f, 1f)] private float _followWeight = 0.3f;
    [SerializeField] private float _smoothSpeed = 15f;

    private Vector3 _baseLocalPosition;

    private void Awake()
    {
        _baseLocalPosition = transform.localPosition;
    }

    private void LateUpdate()
    {
        if (_eyesPoint == null)
            return;

        Vector3 basePosition = transform.parent.TransformPoint(_baseLocalPosition);
        Vector3 targetPosition = Vector3.Lerp(basePosition, _eyesPoint.position, _followWeight);

        if (_smoothSpeed <= 0f)
        {
            transform.position = targetPosition;
            return;
        }

        float t = 1f - Mathf.Exp(-_smoothSpeed * Time.deltaTime);
        transform.position = Vector3.Lerp(transform.position, targetPosition, t);
    }
}