using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "Weapons/WeaponData")]
public class WeaponDataSO : ScriptableObject
{
    [Header("Info")]
    public string weaponName;
    public GameObject weaponPrefab;

    [Header("Stats")]
    public float damage;
    public float range;
    public float fireRate;
}