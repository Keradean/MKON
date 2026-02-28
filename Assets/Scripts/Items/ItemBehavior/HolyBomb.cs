using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

//Hauk
public class HolyBomb : MonoBehaviour
{
    [SerializeField] GameObject grenade;
    [SerializeField] GameObject shockwave;
    [SerializeField] GameObject shockwave2;
    [SerializeField] GameObject shockwave3;
    [SerializeField] GameObject shockwave4;
    [SerializeField] AudioClip explosionSound;

    [SerializeField] float delay = 1.0f;
    [SerializeField] float radius = 5f;
    [SerializeField] LayerMask layermask;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(Explode());
    }

    private IEnumerator Explode()
    {
        yield return new WaitForSeconds(delay);
        //StartShockwave
        grenade.SetActive(false);
        //Play Sound
        SoundManager.instance.PlaySoundFXClip(explosionSound, transform, 1f);
        float targetScale = radius * 2 * 44;
        while (shockwave.transform.localScale.x < targetScale)
        {
            float speed = Time.deltaTime * 3;
            shockwave.transform.localScale += new Vector3(1, 1, 0) * speed * targetScale;

            if (shockwave.transform.localScale.x > targetScale * 0.3f)
            {
                shockwave2.transform.localScale += new Vector3(1, 1, 0) * speed * targetScale;
                if (shockwave.transform.localScale.x > targetScale * 0.6)
                {
                    shockwave3.transform.localScale += new Vector3(1, 1, 0) * speed * targetScale;
                    if (shockwave.transform.localScale.x > targetScale * 0.9f)
                    {
                        shockwave4.transform.localScale += new Vector3(1, 1, 0) * speed * targetScale;
                    }
                }
            }
            
            yield return null;
        }
        yield return new WaitForSeconds(0.1f);
        Collider[] hitRacer = Physics.OverlapSphere(transform.position, radius, layermask);
        foreach (Collider racer in hitRacer)
        {
            racer.GetComponent<Racer>()?.GetHit();
        }

        Destroy(gameObject);
    }
}
