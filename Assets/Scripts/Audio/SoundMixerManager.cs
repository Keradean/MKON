using UnityEngine;
using UnityEngine.Audio;
//Hauk (Wiederverwertet)
public class SoundMixerManager : MonoBehaviour
{
    [SerializeField] AudioMixer audioMixer;

    public void SetMasterVolume(float volume)
    {
        audioMixer.SetFloat("masterVolume", Mathf.Log10(volume) * 20f);
    }

    public void SetSFXVolume(float volume)
    {
        audioMixer.SetFloat("SFXVolume", Mathf.Log10(volume) * 20f);
    }

    public void SetAmbienteVolume(float volume)
    {
        audioMixer.SetFloat("ambienteVolume", Mathf.Log10(volume) * 20f);
    }
}