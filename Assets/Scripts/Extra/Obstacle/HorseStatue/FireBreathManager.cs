using UnityEngine;
using System.Collections; 

public class FireBreathManager : MonoBehaviour
{
    [Header("Fire Breath")]
    public GameObject fiBre;
    public float _fireBreathDuration = 3f;
    public float _pausedDuration = 5f;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(FireBreathLoop());
    }
    
    IEnumerator FireBreathLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(_fireBreathDuration);
            fiBre.SetActive(false);
            yield return new WaitForSeconds(_pausedDuration);
            fiBre.SetActive(true);
        }
    }
}