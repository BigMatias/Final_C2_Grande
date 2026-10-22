using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Enemies/Enemy Data")]
public class EnemyDataSO : ScriptableObject
{
    [Header("Avoidance")]
    public Vector2Int chaseAvoidancePriority = new Vector2Int(40, 60);
    public int attackAvoidancePriority = 5;
    
    [Header("Movement")]
    public float moveSpeed = 1.8f;
    public Vector2 speedVariation = new Vector2(0.9f, 1.1f);
    public float acceleration = 6f;
    public float angularSpeed = 360f;
    public float repathInterval = 0.2f;

    [Header("Attack")]
    public float attackRange = 1.4f;
    public float attackDamage = 15f;
    public float attackCooldown = 1.2f;
    public float attackDuration = 1f;
    public float hitDelay = 0.45f;
    public float hitRangeTolerance = 0.4f;
}