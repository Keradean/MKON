using UnityEngine;

public class Cake : MonoBehaviour
{
    [SerializeField] float speed = 4f;//maybe get from gamemanager
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //move forward
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
        if(Physics.SphereCast(transform.position, 0.1f, transform.forward, out RaycastHit hit, 0.1f))
        {
            //hit.GetComponent<Racer>()?.GetHit();
            Destroy(gameObject);
        }
    }

}
