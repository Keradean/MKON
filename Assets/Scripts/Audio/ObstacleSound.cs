using UnityEngine;
using System.Collections;
using UnityEngine.AI;
//De Col
public class ObstacleSound : MonoBehaviour
{
    [Header("Blockers Hit Config")]
    public AudioSource _audioSource;
    private bool _hasHit;
    public AudioClip FenceHit;
    
    [Header("Splat Config")]
    public AudioClip Splat;
    public AudioClip Return;
    private bool _hasSplat;
    
    [Header("Fire Confiq")]
    private bool _hasBurned;
    public GameObject BurnEffect;
    public GameObject Explosion;
    public GameObject EngineRunning;
    public AudioClip ExplodeSound;

    [Header("Rival Config")] 
    public bool isPlayer = false;
    private NavMeshAgent _agent;
    private bool RivalRotating;
    public AudioClip SpinSound;
    private bool _spinPlayed;

    public bool RivalIsHit;
    
    private Rigidbody _rigidbody;
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        if (!isPlayer)
        {
            _agent = GetComponent<NavMeshAgent>();
        }
    }

    void Update()
    {
        if (RivalRotating)
        {
            transform.Rotate(0, 25, 0);
            StartCoroutine(StopRotating());
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
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
            if (GetComponentInChildren<Racer>().isShielded) return;

            if (!_hasSplat)
            {
                if (!_audioSource.isPlaying && !_hasSplat)
                {
                    _audioSource.clip = Splat;
                    _audioSource.Play();
                    _hasSplat = true;
                    
                    // Kart is crushed!!
                    transform.localScale = new Vector3(2.5f, 0.3f, 1.8f);
                    if (isPlayer)
                    {
                        _rigidbody.isKinematic = true;
                    }

                    if (!isPlayer)
                    {
                        RivalIsHit = true;
                        _agent.speed = 0;
                    }
                    _hasSplat = true;

                }
            }
        }

        if (colision.gameObject.CompareTag("Burn"))
        {
            if (!GetComponentInChildren<Racer>().isShielded)
            {
                BurnTheKart(true);                
            }
        }

        if (colision.gameObject.CompareTag("Player_1"))
        {
            RivalRotating = true;
            _agent.speed = 5.5f;
        }

        if (colision.gameObject.CompareTag("Rival_1") || colision.gameObject.CompareTag("Rival_2") ||
            colision.gameObject.CompareTag("Rival_3") || colision.gameObject.CompareTag("Rival_4"))
        {
            StartCoroutine(PlayerReact());
        }
    }

    public void BurnTheKart(bool respawn)
    {
        if (!_hasBurned)
        {
            _hasBurned = true;
            StartCoroutine(StopTheKart(respawn));
            BurnEffect.SetActive(true);
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

    IEnumerator StopTheKart(bool respawn)
    {
        yield return new WaitForSeconds(0.2f);
        _hasBurned = true;
        yield return new WaitForSeconds(3);
        // turn off the engine sound
        EngineRunning.SetActive(false);
        // turn off the smoke
        BurnEffect.SetActive(false);
        _hasBurned = false;
        // Stop the Kart
        if (isPlayer)
        {
            _rigidbody.isKinematic = true;
        }
        if (!isPlayer)
        {
            RivalIsHit = true;
            _agent.speed = 0;
        }
        // Determine the current position of the kart
        Vector3 kartPosition = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);
        // Create the explosion at the current position of the kart
        Instantiate(Explosion, kartPosition, Quaternion.identity);
        if (!_audioSource.isPlaying)
        {
            _audioSource.clip = ExplodeSound;
            _audioSource.volume = 0.5f;
            _audioSource.Play();
        }
        // Kart disappears temporarily
        transform.localScale = new Vector3(0, 0, 0);
        if (respawn)
        {
            yield return new WaitForSeconds(2);
            // Kart reappears and engine noises are working again
            transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
            EngineRunning.SetActive(true);
            // Kart darf sich wieder bewegen 
            if (isPlayer)
            {
                _rigidbody.isKinematic = false;
            }
            if (!isPlayer)
            {
                RivalIsHit = false;
                _agent.speed = 25;
			    _agent.isStopped = false; 
            }
            if (!_audioSource.isPlaying)
            {
                _audioSource.clip = Return;
                _audioSource.volume = 0.5f;
                _audioSource.Play();
            }
        }
    }

    IEnumerator ResetKart()
    {
        yield return new WaitForSeconds(1.5f);
        // Reset Kart body
        transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
        if (isPlayer)
        {
            _rigidbody.isKinematic = false;
        }
        if (!isPlayer)
        {
            RivalIsHit = false; 
            _agent.speed = 25;
        }
        if (!_audioSource.isPlaying && _hasSplat)
        {
            _audioSource.clip = Return;
            _audioSource.volume = 0.5f;
            _audioSource.Play();
            _hasSplat = false;
        }
    }

    IEnumerator StopRotating()
    {
        if(!_audioSource.isPlaying && !_spinPlayed)
        {
            _audioSource.clip = SpinSound;
            _audioSource.volume = 0.5f;
            _audioSource.Play();
            _spinPlayed = true;
        }

        yield return new WaitForSeconds(3);
        RivalRotating = false;
        _agent.speed = 20;
        RivalIsHit = false;
        _spinPlayed = false;
    }

    IEnumerator PlayerReact()
    {
        _rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        yield return new WaitForSeconds(1);
        _rigidbody.constraints = RigidbodyConstraints.None; 
    }
}

