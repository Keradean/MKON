using UnityEngine;

[CreateAssetMenu(fileName = "CharacterSO", menuName = "Scriptable Objects/CharacterSO")]
public class CharacterSO : ScriptableObject
{
    [Range(0f, 1f)] public float acceleration;
    public float brakePower;
    public float enginePower;
    public float maxSpeed;
    [Range(0.4f, 1.3f)]public float luck;
    [Range(0f, 1f)] public float gripFactor;
    public float maxSteerAngle;
    [Range(0.5f, 2f)] public float steerResponse = 1f;

    [Header("Drift Settings")]
    [Range(400f, 700f)]public float driftSideForce = 600f;        // seitliche Driftkraft
    [Range(0.6f, 0.9f)]public float frontDriftGripLoss = 0.8f;         // weniger Grip
    [Range(0.1f, 0.4f)] public float backDriftGripLoss = 0.2f;          // starker Gripverlust
    [Range(1.5f, 2f)] public float driftSteerMultiplier = 1.5f;  // stärkeres Einlenken

}
