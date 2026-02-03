using Unity.VisualScripting;
using UnityEngine;

public class PepperTrap : MonoBehaviour
{
    [SerializeField] LayerMask layerMask;
    private void OnTriggerEnter(Collider other)
    {
        if ((layerMask.value & (1 << other.gameObject.layer)) != 0)
        {
            Debug.Log("Pepper Trap Triggered");
            other.GetComponent<Racer>()?.GetHit();
            Destroy(gameObject);
        }
    }

    public void DisableTrap()
    {
        Destroy(gameObject);
    }
}
