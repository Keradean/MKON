using UnityEngine;
using System.Collections;
using TMPro;
//Hauk
public class MySoul : MonoBehaviour
{
    [SerializeField] GameObject LavaPoolPrefab;
    [SerializeField] GameObject Lights;
    private Racer targetRacer;
    private Coroutine trapHim;
    [SerializeField] float cd = 3f;
    [SerializeField] [Range(0, 100)] float activationChance = 100f;
    [SerializeField] float distanceAhead = 20f;




    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Racer>(out Racer racer))
        {
            targetRacer = racer;
            if (trapHim == null)
            {
                trapHim = StartCoroutine(TrapHim());
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<Racer>(out Racer racer))
        {
            if (targetRacer == racer)
            {
                targetRacer = null;
            }
        }
    }

    private IEnumerator TrapHim()
    {
        int flickerCount = Random.Range(3, 6);
        for (int i = 0; i < flickerCount; i++)
        {
            Lights.SetActive(true);
            yield return new WaitForSeconds(Random.Range(0.05f, 0.1f));
            Lights.SetActive(false);
            yield return new WaitForSeconds(Random.Range(0.1f, 0.2f));
        }

        if (Random.Range(0,100) < activationChance)
        {
            Debug.Log("My Soul Trap Activated!");
            Lights.SetActive(true);
            yield return new WaitForSeconds(0.4f);
            if (targetRacer != null)
            {
                Vector3 spawnPos = targetRacer.transform.position + targetRacer.transform.forward * distanceAhead;
                spawnPos.y = 0.5f;
                GameObject lavapool = Instantiate(LavaPoolPrefab, spawnPos, transform.rotation);
                yield return new WaitForSeconds(2f);
                Destroy(lavapool);
            }
            Lights.SetActive(false);
        }
        targetRacer = null;
        yield return new WaitForSeconds(cd);
        trapHim = null;
    }
}
