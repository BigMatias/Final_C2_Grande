using UnityEngine;

[CreateAssetMenu(fileName = "PlayerSettings", menuName = "Player/Data")]

public class PlayerSettingsSO : ScriptableObject
{
    [Header("Controls")]
    public KeyCode jumpKey;
    public KeyCode runkey;
    public KeyCode crouchKey;
    [Header("General Configs: ")]
    public float acceleration = 8f;
    public float deceleration = 10f;
    public float mouseSens;
    public float speed;
    public float runSpeed;
    public float crouchSpeed;
    public float jumpForce;
}