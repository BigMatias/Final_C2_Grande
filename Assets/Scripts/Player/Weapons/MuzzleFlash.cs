using UnityEngine;

public class MuzzleFlash : MonoBehaviour
{
    [SerializeField] private Light _light;
    [SerializeField] private ParticleSystem _particles;
    [SerializeField] private float _duration = 0.06f;
    [SerializeField] private Vector2 _intensityVariation = new Vector2(0.8f, 1.2f);

    private float _baseIntensity;
    private float _currentPeak;
    private float _timer;

    private void Awake()
    {
        if (_light == null)
            return;

        _baseIntensity = _light.intensity;
        _light.enabled = false;
    }

    public void Play()
    {
        if (_particles != null)
        {
            _particles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            _particles.Play(true);
        }

        if (_light == null)
            return;

        _currentPeak = _baseIntensity * Random.Range(_intensityVariation.x, _intensityVariation.y);
        _light.intensity = _currentPeak;
        _light.enabled = true;
        _timer = _duration;
    }

    private void Update()
    {
        if (_light == null || _timer <= 0f)
            return;

        _timer -= Time.deltaTime;

        if (_timer <= 0f)
        {
            _light.enabled = false;
            return;
        }

        _light.intensity = _currentPeak * (_timer / _duration);
    }
}