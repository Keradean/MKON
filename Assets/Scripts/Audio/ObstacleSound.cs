using UnityEngine;
using System.Collections;

public class ObstacleSound : MonoBehaviour
{
    public AudioSource _audioSource;
    private bool _hasHit;
    public AudioClip FenceHit;
    public AudioClip Splat;
    public AudioClip Return;
    private bool _hasSplat;
    
    private Rigidbody _rigidbody;
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void OnTriggerEnter(Collider colision)
    {
        if (colision.gameObject.CompareTag("Obstacle"))
        {
            if (!_hasHit)
            {
                if (!_audioSource.isPlaying)
                {
                    _audioSource.clip = FenceHit;
                    _audioSource.Play();
                    _hasHit = true;
                }
            }
        }

        if (colision.gameObject.CompareTag("Splat"))
        {
            if (!_hasSplat)
            {
                if (!_audioSource.isPlaying && !_hasSplat)
                {
                    _audioSource.clip = Splat;
                    _audioSource.Play();
                    _hasSplat = true;
                    
                    // Kart is crushed!!
                    transform.localScale = new Vector3(2.5f, 0.3f, 1.8f);
                    _rigidbody.isKinematic = true;
                    _hasSplat = true;

                }
            }
        }
    }

    private void OnTriggerExit(Collider colision)
    {
        if (colision.gameObject.CompareTag("Obstacle"))
        {
            if (_hasHit)
            {
                _hasHit = false;
            }
        }  
        if (colision.gameObject.CompareTag("Splat"))
        {
            if (_hasSplat)
            {
                _hasHit= false;
                StartCoroutine(ResetKart());
            }
        }  
    }

    IEnumerator ResetKart()
    {
        yield return new WaitForSeconds(1.5f);
        // Reset Kart body
        transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
        _rigidbody.isKinematic = false;

        if (!_audioSource.isPlaying && _hasSplat)
        {
            _audioSource.clip = Return;
            _audioSource.Play();
            _hasSplat = false;
        }
    }

}
