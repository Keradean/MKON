using UnityEngine;

public class CollectableItem : MonoBehaviour
{
    [SerializeField] GameObject item;
    [SerializeField] float respawnTime = 5f;
    private bool flag = true;

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<ItemInventory>() == null) return;
        if (!flag) return;
        Debug.Log("Collected Item");
        flag = false;
        item.SetActive(false);
        StartCoroutine(RespawnItem());
        other.GetComponent<ItemInventory>().CollectItem();

    }
    private System.Collections.IEnumerator RespawnItem()
    {
        yield return new WaitForSeconds(respawnTime);
        item.SetActive(true);
        flag = true;
    }
}
