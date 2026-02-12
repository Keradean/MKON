using UnityEngine;
using UnityEngine.InputSystem;

public class KartSound : MonoBehaviour
{
    [Header("Effect Sounds")]
    public AudioSource StartSound;
    public AudioSource IdleSound;
    public AudioSource DrivingSound;
    public AudioSource DriftSound;
    public AudioSource ReverseSound;
    [SerializeField] AudioClip StartSoundClip;
    [SerializeField] AudioClip IdleSoundClip;
    [SerializeField] AudioClip DrivingSoundClip;
    [SerializeField] AudioClip DriftSoundClip;
    [SerializeField] AudioClip ReverseSoundClip;


    [Header("Sound Settings")]
    [Range(0f, 1f)] public float StartSoundVolume = 1f;
    [Range(0f, 1f)] public float IdleSoundVolume = 1.0f;
    [Range(0f, 1f)] public float DrivingSoundVolume = 1.0f;
    [Range(0f, 1f)] public float ReverseSoundVolume = 0.5f;
    [Range(0f, 1f)] public float DriftSoundVolume = 1.0f;
    [Range(0f, 2f)] public float DrivingSoundMaxPitch = 1.0f;
    [Range(0f, 2f)] public float ReverseSoundMaxPitch = 1.5f;
    
    private Rigidbody rb;
    private float ddc_KartSpeed;
    [HideInInspector] public bool isReversing = false;
    [HideInInspector] public bool BrakeToAStop = false;
    public ParticleSystem[] TireSmoke;
    
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        for (int i = 0; i < TireSmoke.Length; i++)
        {
            TireSmoke[i].gameObject.SetActive(true);
            TireSmoke[i].Stop();
        }

        if (IdleSound != null)
        {
            IdleSound.volume = 0f; 
            IdleSound.Play();
        }

        if (DrivingSound != null)
        {
            DrivingSound.volume = 0f;
            DrivingSound.Play();
        }

        if (ReverseSound != null)
        {
            ReverseSound.volume = 0f;
            ReverseSound.Play();
        }
        
        if (DriftSound != null) 
        {
            DriftSound.volume = 0f;
            DriftSound.Play();
        }
    }

    void Update()
    {
        ddc_KartSpeed = rb.linearVelocity.magnitude * 3.6f / 100f; 
        
        // Prüfen ob rückwärts
        CheckIfReversing();
        
        // Sounds abspielen
        PlayIdleSound();
        PlayDrivingSound();
        PlayReverseSound();
        PlayDriftSound();
        PlayBrakeToAStop();
    }

    private void CheckIfReversing()
    {
        // Geschwindigkeit Fahrtrichtung prüfen
        float forwardSpeed = Vector3.Dot(rb.linearVelocity, transform.forward);
        isReversing = forwardSpeed < -0.5f;
    }

    private void PlayIdleSound()
    {
        if (IdleSound != null)
        {
            IdleSound.volume = Mathf.Lerp(IdleSoundVolume, 0.0f, ddc_KartSpeed * 4);
        }
    }

    private void PlayDrivingSound()
    {
        if (DrivingSound != null)
        {
            if (!isReversing && ddc_KartSpeed > 0.01f)
            {
                // Vorwärts fahren
                DrivingSound.volume = Mathf.Lerp(0.1f, DrivingSoundVolume, ddc_KartSpeed * 1.2f);
                DrivingSound.pitch = Mathf.Lerp(0.3f, DrivingSoundMaxPitch, ddc_KartSpeed + (Mathf.Sin(Time.time * 10f) * 0.05f));
            }
            else
            {
                DrivingSound.volume = 0f;
            }
        }
    }

    private void PlayReverseSound()
    {
        if (ReverseSound != null)
        {
            if (isReversing && ddc_KartSpeed > 0.01f) 
            {
                // Rückwärts fahren
                ReverseSound.volume = Mathf.Lerp(0.1f, ReverseSoundVolume, ddc_KartSpeed * 1.2f);
                ReverseSound.pitch = Mathf.Lerp(0.5f, ReverseSoundMaxPitch, ddc_KartSpeed + (Mathf.Sin(Time.time * 10f) * 0.05f));
            }
            else
            {
                ReverseSound.volume = 0f;
            }
        }
    }
    private void PlayDriftSound()
    {
        PlayerKartControl kartControl = GetComponent<PlayerKartControl>();
    
        if (DriftSound != null && kartControl != null)
        {
            float sidewaysSpeed = Vector3.Dot(rb.linearVelocity, transform.right);
            bool isDrifting = Mathf.Abs(sidewaysSpeed) > 2f && ddc_KartSpeed > 0.1f;
        
            if (isDrifting)
            {
                DriftSound.volume = Mathf.Lerp(DriftSound.volume, DriftSoundVolume, Time.deltaTime * 5f);
            }
            else
            {
                DriftSound.volume = Mathf.Lerp(DriftSound.volume, 0f, Time.deltaTime * 10f);
            }
        }
    }

    private void PlayBrakeToAStop()
    {
        // Harsh brake sound
        if (BrakeToAStop && ddc_KartSpeed < 0.60f && !isReversing)
        {
            ReverseSound.volume = 0f;
            DrivingSound.volume = 0f; 
            DriftSound.volume = Mathf.Lerp(0.1f, DriftSoundVolume, ddc_KartSpeed * 1.2f);

            for (int i = 0; i < TireSmoke.Length; i++)
            {
                if (TireSmoke[i].isStopped)
                {
                    TireSmoke[i].Play();
                }
            }
        }
        // When kart stops 
        if (ddc_KartSpeed < 0.03f || !BrakeToAStop)
        {
            DriftSound.volume = 0f;
            for (int i = 0; i < TireSmoke.Length; i++)
            {
                if (TireSmoke[i].isPlaying)
                {
                    TireSmoke[i].Stop();
                }
            }

        }
        
    }

    public void OnBrake(InputValue button)
    {
        if(button.isPressed)
        {
            BrakeToAStop = true;
            
        }
        if(!button.isPressed)
            {
            BrakeToAStop = false;
            }
    }
}