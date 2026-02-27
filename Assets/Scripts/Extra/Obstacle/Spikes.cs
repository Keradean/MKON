using System.Collections;
using UnityEngine;
//Hauk
public class Spikes : MonoBehaviour
{
    [SerializeField] GameObject spikesMesh;
    [SerializeField] LayerMask layerMask;
    private bool spikesActive = false;

    private void OnTriggerEnter(Collider other)
    {
        if ((layerMask.value & (1 << other.gameObject.layer)) != 0)
        {
            if (spikesActive)
            {
                other.GetComponent<Racer>()?.GetHit();
                return;
            }
            StartCoroutine(AnimateSpikes(other.GetComponent<Racer>()) );
        }
    }

    //shoot spikes out of the ground
    private IEnumerator AnimateSpikes(Racer racer)
    {
        spikesActive = true;
        float timer = 0f;
        while (timer < 0.3f)
        {
            if (timer > 0.15f && racer != null)
            {
                racer.GetHit();
                racer = null; // ensure GetHit is only called once
            }
            timer += Time.deltaTime * 4;
            spikesMesh.transform.localPosition = Vector3.Lerp(new Vector3(0, -1.5f, 0), Vector3.zero, timer / 0.3f);
            yield return null;
        }
        yield return new WaitForSeconds(2f);
        timer = 0f;
        while (timer < 0.3f)
        {
            timer += Time.deltaTime * 2;
            spikesMesh.transform.localPosition = Vector3.Lerp(Vector3.zero, new Vector3(0, -1.5f, 0), timer / 0.3f);
            yield return null;
        }
        spikesActive = false;
    }
}
