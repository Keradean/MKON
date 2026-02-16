using UnityEngine;
using System.Collections.Generic;

public class ItemManager : MonoBehaviour
{
    public static ItemManager Instance;

    [SerializeField] List<Item> traps = new List<Item>();
    [SerializeField] List<Item> boosts = new List<Item>();
    [SerializeField] List<Item> projectiles = new List<Item>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public Item GetRandomItem(int ranking, float luck) 
    {
        int racerCount = GameManager.Instance.RacerRanking.Count;
        //modifiy chance to get items for your ranking, projectiles in the back, traps in the front
        float modifire = (racerCount/2 - ranking) *2;
        float roll = Random.Range(0f, 100f);
        //return null;
        if (roll > 70f - modifire)
        {
            int index = Random.Range(0, traps.Count);
            return traps[GetBiasedIndex(traps.Count, luck)];
        }
        else if (roll > 40 + modifire)
        {
            int index = Random.Range(0, projectiles.Count);
            return projectiles[GetBiasedIndex(projectiles.Count, luck)];
        }
        else if (roll > 5 + luck)
        {
            int index = Random.Range(0, boosts.Count);
            return boosts[GetBiasedIndex(boosts.Count, luck)];
        }
        else
        {
            return null;
        }
    }

    // Returns a randomly selected index with a controllable bias. 
    // The parameter k determines how strongly the distribution is skewed.
    int GetBiasedIndex(int count, float k = 1f)
    {
        float r = Random.value;   // 0..1

        // Apply a power-based bias: 
        // k > 1 → favors lower indices (values near 0 become more likely) 
        // k < 1 → favors higher indices (values near 1 become more likely) 
        // k = 1 → no bias
        float biased = Mathf.Pow(r, k);
        return Mathf.FloorToInt(biased * count);
    }

}