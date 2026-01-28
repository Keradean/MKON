using UnityEngine;

public class CollectableItem : MonoBehaviour
{
    [SerializeField] GameObject item;
    [SerializeField] float respawnTime = 5f;


    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<ItemInventory>() == null) return;
        other.GetComponent<ItemInventory>().CollectItem();

        item.SetActive(false);
        StartCoroutine(RespawnItem());
    }
    private System.Collections.IEnumerator RespawnItem()
    {
        yield return new WaitForSeconds(respawnTime);
        item.SetActive(true);
    }
}
