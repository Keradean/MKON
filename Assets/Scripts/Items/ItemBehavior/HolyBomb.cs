using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;


public class HolyBomb : MonoBehaviour
{
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
        //Play Sound
        yield return new WaitForSeconds(0.1f);
        Collider[] hitRacer = Physics.OverlapSphere(transform.position, radius, layermask);
        foreach (Collider racer in hitRacer)
        {
            //racer.GetComponent<Racer>().GetHit();
        }
    }
}
