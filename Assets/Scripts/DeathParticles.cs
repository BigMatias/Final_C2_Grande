using System.Collections;
using UnityEngine;

public class DeathParticles : MonoBehaviour, IPooleable
{
    private ParticleSystem _particles;

    public bool IsActive => gameObject.activeSelf;

    private void Awake() => _particles = GetComponent<ParticleSystem>();

    public void Activate()
    {
        gameObject.SetActive(true);
        _particles.Play();
    }


    public void DeActivate()
    {
        _particles.Stop();
        gameObject.SetActive(false);
    }
    
    private IEnumerator ReturnWhenDone()
    {
        yield return new WaitForSeconds(_particles.main.duration);
    }
}