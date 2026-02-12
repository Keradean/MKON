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
        float modifire = (racerCount/2 - ranking) *2;
        float roll = Random.Range(0f, 100f);
        //roll = 40;
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

    int GetBiasedIndex(int count, float k = 1f)
    {
        float r = Random.value;   // 0..1
        float biased = Mathf.Pow(r, k);
        return Mathf.FloorToInt(biased * count);
    }

}