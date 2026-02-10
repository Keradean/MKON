using UnityEngine;
using UnityEngine.AI;
public class KartSoundRival : MonoBehaviour
{
    public AudioSource IdleSound;

    public AudioSource DrivingSound;
    // Volume and pitch Settings
    [Range(0.1f, 1.0f)] public float DrivingSoundVolume = 1.0f;
    [Range(0.1f, 2.0f)] public float DrivingSoundMaxPitch = 1.0f;
    private NavMeshAgent _agent;

    private float _kartSpeed;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        _kartSpeed = _agent.speed / 100;

        PlayIdleSound();
        PlayDrivingSound();

    }

    void PlayIdleSound()
    {
        IdleSound.volume = Mathf.Lerp(0.4f, 0f, _kartSpeed * 4);
    }

    void PlayDrivingSound()
    {
        if (_kartSpeed > 0.0f)
        {
            DrivingSound.volume = Mathf.Lerp(0.1f, DrivingSoundVolume, _kartSpeed * 1.2f);
            DrivingSound.pitch = Mathf.Lerp(0.3f, DrivingSoundMaxPitch,_kartSpeed + (Mathf.Sin(Time.time) * .1f));
        }
    }
}
