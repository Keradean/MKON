using UnityEngine;
using System.Collections.Generic;
using System.Collections;


public class ItemInventory : MonoBehaviour
{
    private List<Item> collectedItems = new List<Item>();
    public Racer racer;
    private bool isAI = false;
    private bool isUsingItem = false;
    [SerializeField] private Item specialItem;
    [SerializeField] [Range(0.4f, 1.3f)] private float itemLuck = 0.7f; // 1 no bias, <1 more likely to get worse items, >1 more likely to get better items

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        racer = GetComponent<Racer>();
        isAI = racer.isAI;
        if(specialItem == null)
        {
            specialItem = ItemManager.Instance.GetRandomItem(1, itemLuck);
        }
    }

    public void CollectItem()
    {
        if (collectedItems.Count < 4)
        {
            Item item = ItemManager.Instance.GetRandomItem(racer.rankingPos, itemLuck);
            if (item == null) item = specialItem;
            collectedItems.Add(item);
            Debug.Log(collectedItems[collectedItems.Count-1].itemType);
        }
        if (isAI && !isUsingItem)
        {
            StartCoroutine(AIUseItem());
        }
    }

    public void OnItemuse()
    {
        Debug.Log("Use Item");
        if (collectedItems.Count > 0)
        {
            Item itemToUse = collectedItems[0];
            collectedItems.RemoveAt(0);
            itemToUse.Activate(this);
        }
    }

    public void ApplyBoost(boostType type, float amount, float duration)
    {
        switch (type) {
            case boostType.Speed:
                racer.Speedboost(amount, duration, Vector3.zero);
                break;
            case boostType.Shield:
                racer.GetShieldBoost(duration);
                break;
        }
    }

    private IEnumerator AIUseItem()
    {
        isUsingItem = true;
        while (collectedItems.Count > 0)
        {
            yield return new WaitForSeconds(Random.Range(1f, 5f));
            OnItemuse();
        }
        isUsingItem = false;
    }
}
