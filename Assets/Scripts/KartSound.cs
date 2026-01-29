using Unity.VisualScripting;
using UnityEngine;

public class KartSound : MonoBehaviour
{
    [Header("Effect Sounds")]
    [SerializeField] private AudioSource StartSound;
    [SerializeField] private AudioSource EngineSound;
    [SerializeField] private AudioSource DrivingSound;
    [SerializeField] private AudioSource ReverseSound;
    //[SerializeField]

    public static KartSound Instance;

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
