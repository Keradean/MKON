using Unity.VisualScripting;
using UnityEngine;

public class KartSound : MonoBehaviour
{
    [Header("Effect Sounds")]
    public AudioSource StartSound;
    public AudioSource EngineSound;
    public AudioSource DrivingSound;
    public AudioSource ReverseSound;
    //[SerializeField]

    public static KartSound Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void PlaySound(AudioSource sound)
    {
        sound.Stop();
        sound.Play();
    }

    public void PlayTunedSound(AudioSource sound)
    {   sound.pitch = Random.Range(0.8f, 1.2f);
        sound.Stop();
        sound.Play();
    }
}
