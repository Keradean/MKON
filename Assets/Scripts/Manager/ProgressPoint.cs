using UnityEngine;

public class ProgressPoint : MonoBehaviour
{
    public int waypointIndex;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        Racer racer = other.GetComponent<Racer>();
        if (racer != null)
        {
            if (racer.waypointIndex == waypointIndex -1)
            {
                racer.waypointIndex = waypointIndex;
                if (!racer.isAI) other.GetComponentInParent<PlayerKartControl>().kartReset = transform;
            }
        }
    }
}
