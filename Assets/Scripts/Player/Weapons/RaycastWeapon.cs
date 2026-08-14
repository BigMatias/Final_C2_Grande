using System.Collections;
using UnityEngine;

public class RaycastWeapon : WeaponBase
{
    [SerializeField] private LayerMask hitLayers;
    [SerializeField] private LineRenderer bulletTracer;
    [SerializeField] private float tracerDuration = 0.05f;
    [SerializeField] protected Transform shootPoint;
    
    
    protected override void Shoot(Ray ray)
    {
        Ray shootRay = shootPoint != null ? 
            new Ray(shootPoint.position, shootPoint.forward) : ray;

        Debug.DrawRay(shootRay.origin, shootRay.direction * _weaponDataSo.range, Color.yellow, 0.1f);

        Vector3 endPoint;
        if (Physics.Raycast(shootRay, out RaycastHit hit, _weaponDataSo.range, hitLayers))
        {
            hit.collider.GetComponentInParent<IDamageable>()?.TakeDamage(_weaponDataSo.damage);
            endPoint = hit.point;
        }
        else
        {
            endPoint = shootRay.origin + shootRay.direction * _weaponDataSo.range;
        }
        
        StartCoroutine(ShowTracer(shootRay.origin, endPoint));
    }

    private IEnumerator ShowTracer(Vector3 start, Vector3 end)
    {
        bulletTracer.enabled = true;
        bulletTracer.SetPosition(0, start);
        bulletTracer.SetPosition(1, end);

        yield return new WaitForSeconds(tracerDuration);

        bulletTracer.enabled = false;
    }
}