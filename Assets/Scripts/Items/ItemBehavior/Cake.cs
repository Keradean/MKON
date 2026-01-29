using UnityEngine;

public class Cake : MonoBehaviour
{
    [SerializeField] float speed = 4f;//maybe get from gamemanager
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //reset x and z rotation
        transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);
    }

    // Update is called once per frame
    void Update()
    {
        //move forward
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
        if(Physics.SphereCast(transform.position, 1f, transform.forward, out RaycastHit hit, 1f))
        {
            Debug.Log(hit.collider.name);
            //hit.GetComponent<Racer>()?.GetHit();
            Destroy(gameObject);
        }
    }

}
