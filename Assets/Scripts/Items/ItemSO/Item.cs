using UnityEngine;
//Hauk
[CreateAssetMenu(fileName = "Item", menuName = "Scriptable Objects/Item")]
public class Item : ScriptableObject
{
    public Itemtype itemType;
    [SerializeField] GameObject ItemPrefab;
    public GameObject itemIconPrefab;

    [SerializeField] boostType boostEffect;
    [SerializeField] float boostDuration;
    [SerializeField] float boostAmount; 

    private ItemInventory user;
    private GameObject InstantiatedPrefab;

    public void Activate(ItemInventory user)
    {
        this.user = user;
        switch (itemType)
        {
            case Itemtype.Trap:
                ActivateTrap();
                break;
            case Itemtype.Boost:
                ActivateBoost();
                break;
            case Itemtype.Projectile:
                ActivateProjectile();
                break;
        }
    }
    private void ActivateTrap()
    {
        Vector3 spawn = user.transform.position - user.transform.forward * 3f;
        Instantiate(ItemPrefab, spawn, Quaternion.identity);
    }
    private void ActivateBoost()
    {
        user.ApplyBoost(boostEffect, boostAmount, boostDuration);
        if (boostEffect == boostType.Shield)
        {
            InstantiatedPrefab = Instantiate(ItemPrefab, user.transform);
            InstantiatedPrefab.GetComponent<ShieldBuff>().duration = boostDuration;
        }
    }
    private void ActivateProjectile()
    {
        Vector3 spawn = user.transform.position + new Vector3(0, 0.2f, 0) + user.transform.forward * 4f;
        GameObject proj = Instantiate(ItemPrefab, spawn, user.transform.rotation);
        if (proj.TryGetComponent(out Coconut coconut))
        {
            int targetIndex = user.racer.rankingPos - 2;
            if (targetIndex < 0) targetIndex = GameManager.Instance.RacerRanking.Count - 1;
            coconut.target = GameManager.Instance.RacerRanking[targetIndex];
        }
    }
}

public enum Itemtype
{
    Trap,
    Boost,
    Projectile
}

public enum boostType
{
    Speed,
    Shield
}