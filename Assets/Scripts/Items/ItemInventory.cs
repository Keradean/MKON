using UnityEngine;
using System.Collections.Generic;
using System.Collections;


public class ItemInventory : MonoBehaviour
{
    private List<Item> collectedItems = new List<Item>();
    private Racer racer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        racer = GetComponent<Racer>();
    }

    public void CollectItem()
    {
        if (collectedItems.Count < 4)
        {
            collectedItems.Add(ItemManager.Instance.GetRandomItem(8/*racer.raking*/));
            Debug.Log(collectedItems[collectedItems.Count-1].itemType);
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
                racer.Speedboost(amount, duration);
                break;
            case boostType.Shield:
                racer.GetShieldBoost(duration);
                break;
        }
    }
}
