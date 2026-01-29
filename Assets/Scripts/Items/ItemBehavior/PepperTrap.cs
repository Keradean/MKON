using UnityEngine;

public class PepperTrap : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Physics.SphereCast(transform.position, 0.1f, transform.forward, out RaycastHit hit, 0.1f))
        {
            //hit.GetComponent<Racer>()?.GetHit();
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //other.GetComponent<Racer>()?.GetHit();
        Destroy(gameObject);
    }
}
