using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;
    [SerializeField] private AudioSource soundFXObject;//prefab for soundeffects
    [Header("SoundReferences")]
    [SerializeField] private GameObject backgroundMusic;
    [SerializeField] private AudioClip[] raceMusic;


    private void Awake()
    {
        //make sure their is only one instance
        if (instance == null)
        {
            instance = this;
        }
        //play backgroundmusic on start
        RaceMusic();
    }


    //play race music and stop backround music loop
    public void RaceMusic()
    {
        CancelInvoke("RaceMusic");

        int random = Random.Range(0, raceMusic.Length);
        backgroundMusic.GetComponent<AudioSource>().clip = raceMusic[random];
        backgroundMusic.GetComponent<AudioSource>().volume = 0.3f;
        backgroundMusic.GetComponent<AudioSource>().Play();
        float clipLength = raceMusic[random].length +1f;
        Invoke("RaceMusic", clipLength);
    }

    //play a soundeffect on a instantiated prefab
    public void PlaySoundFXClip(AudioClip audioClip, Transform spawnTransform, float volume)
    {
        AudioSource audioSource = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity);
        audioSource.clip = audioClip;
        audioSource.volume = volume;
        audioSource.Play();
        float clipLength = audioSource.clip.length;
        Destroy(audioSource, clipLength);
    }

    public AudioSource LoopSFXClip(AudioClip audioClip, Transform spawnTransform, float volume)
    {
        AudioSource audioSource = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity);
        audioSource.clip = audioClip;
        audioSource.volume = volume;
        audioSource.loop = true;
        audioSource.Play();
        return audioSource;
    }

    //stop a soundeffect 
    public void StopSoundFXClip(AudioSource audioSource)
    {
        if (audioSource != null)
        {
            audioSource.Stop();
            Destroy(audioSource.gameObject);
        }
    }
}