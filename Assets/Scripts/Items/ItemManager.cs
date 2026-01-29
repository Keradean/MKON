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

    public Item GetRandomItem(int ranking) 
    {
        int racerCount = 8;//RaceManager.Instance.racers.Count;
        float modifire = (racerCount/2 - ranking) *2;
        float roll = Random.Range(0f, 100f);
        roll = 40;
        if (roll > 60f - modifire)
        {
            int index = Random.Range(0, traps.Count);
            return traps[index];
        }
        else if (roll > 30 + modifire)
        {
            int index = Random.Range(0, projectiles.Count);
            return projectiles[index];
        }
        else
        {
            int index = Random.Range(0, boosts.Count);
            return boosts[index];
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
