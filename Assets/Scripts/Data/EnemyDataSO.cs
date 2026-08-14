using UnityEngine;

[CreateAssetMenu(fileName = "EnemySettings", menuName = "Enemy/Settings")]

public class EnemyDataSO : ScriptableObject
{
    [Header("Normal Zombie: ")]
    public float speed;
    public float meleeDamage;
    public float meleeRange;
    public float meleeCd;
    public float meleeActivateHitboxTime;
    public int pointsGivenMelee;
    [Header("Gun Zombie: ")] 
    public float gunRange;
    public float gunBulletLifetime;
    public float gunBulletSpeed;
    public float gunShootCooldown;
    public float gunDamage;
    public int pointsGivenRanged;
    [Header("Granadier Zombie: ")]
    public float granadeRange;
    public float granadeLifeTime;
    public float granadeCooldown;
    public float granadeDamage;
    public float granadeRadius;
    public float granadeFlightTime;
    public int pointsGivenGranadier;
}  