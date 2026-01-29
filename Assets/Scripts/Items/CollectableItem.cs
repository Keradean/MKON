using UnityEngine;

public class CollectableItem : MonoBehaviour
{
    [SerializeField] GameObject item;
    [SerializeField] float respawnTime = 5f;
    private bool flag = true;
    [SerializeField]
    LayerMask racerLayer;

    void Update()
    {
        if (flag) 
        {
            if (Physics.SphereCast(transform.position, 1f, -transform.up, out RaycastHit hit, 1f, racerLayer))
            {
                flag = false;
                Debug.Log("Collected Item from: " + hit.collider.name);
                item.SetActive(false);
                StartCoroutine(RespawnItem());
                if (hit.collider.GetComponent<ItemInventory>() == null) return;
                hit.collider.GetComponent<ItemInventory>().CollectItem();

            }
        }
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    Debug.Log("Collected Item");
    //    item.SetActive(false);
    //    StartCoroutine(RespawnItem());
    //    if(other.GetComponent<ItemInventory>() == null) return;
    //    other.GetComponent<ItemInventory>().CollectItem();

    //}
    private System.Collections.IEnumerator RespawnItem()
    {
        yield return new WaitForSeconds(respawnTime);
        item.SetActive(true);
        flag = true;
    }
}
