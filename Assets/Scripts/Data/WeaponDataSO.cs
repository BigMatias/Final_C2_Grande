using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "Weapons/Weapon Data")]
public class WeaponDataSO : ScriptableObject
{
    [Header("General")]
    public string weaponName;
    public GameObject weaponPrefab;
    public int price;

    [Header("Damage")]
    public float damage = 25f;
    public float range = 100f;
    public float impactForce = 4f;
    public LayerMask hitLayers = ~0;

    [Header("Projectile")]
    public Projectile projectilePrefab;
    public float projectileSpeed = 250f;
    public float projectileGravity = 0f;
    public int projectilePoolSize = 30;

    [Header("Fire")]
    public float roundsPerMinute = 300f;
    public bool isAutomatic;

    [Header("Handling")]
    public float weight = 1.2f;
    public float aimFov = 55f;

    [Header("Camera Recoil")]
    public Vector2 recoilPitchRange = new Vector2(1.5f, 2.5f);
    public Vector2 recoilYawRange = new Vector2(-0.6f, 0.6f);
    public float recoilRecoverySpeed = 6f;
    [Range(0f, 1f)] public float aimRecoilMultiplier = 0.6f;

    [Header("Weapon Kick")]
    public float kickBack = 0.06f;
    public float kickRotation = 6f;
    public float kickReturnSpeed = 10f;

    public float TimeBetweenShots => 60f / Mathf.Max(roundsPerMinute, 1f);
    public float ProjectileLifetime => range / Mathf.Max(projectileSpeed, 0.01f);
}