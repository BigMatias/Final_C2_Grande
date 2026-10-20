using UnityEngine;

public struct DamageInfo
{
    public float Amount;
    public Vector3 Point;
    public Vector3 Direction;
    public float Force;
    public BodyPart BodyPart;
    public Transform HitBone;
    public Rigidbody HitBody;

    public DamageInfo(float amount, Vector3 point, Vector3 direction, float force)
    {
        Amount = amount;
        Point = point;
        Direction = direction;
        Force = force;
        BodyPart = BodyPart.Torso;
        HitBone = null;
        HitBody = null;
    }
}