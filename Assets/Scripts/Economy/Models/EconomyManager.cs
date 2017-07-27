using UnityEngine;
using System;

public class EconomyManager : SingletonMonobehaviour<EconomyManager>
{
    [SerializeField]
    private int maxFood;
    [SerializeField]
    private int maxWood;
    [SerializeField]
    private int maxStone;
    [SerializeField]
    private int maxGold;
    [SerializeField]
    private int maxMana;

    private Resource food;
    private Resource wood;
    private Resource stone;
    private Resource gold;
    private Resource mana;

    private Action OnResourceValueChangeCallback;

    public Resource Food
    {
        get
        {
            return food;
        }
    }

    public Resource Wood
    {
        get
        {
            return wood;
        }
    }

    public Resource Stone
    {
        get
        {
            return stone;
        }
    }

    public Resource Gold
    {
        get
        {
            return gold;
        }
    }

    public Resource Mana
    {
        get
        {
            return mana;
        }
    }

    private void Start ()
    {
        CDebug.Log(CDebug.applicationLoading, "Econonmy Manager Start");

        food = new Resource("Food", maxFood, 0);
        wood = new Resource("Wood", maxWood, 0);
        stone = new Resource("Stone", maxStone, 0);
        gold = new Resource("Gold", maxGold, 0);
        mana = new Resource("Mana", maxMana, 0);

        DoTransaction(MapGenerator.Instance.Level.StartingResources);

        OnResourceValueChangeCallback?.Invoke();
    }

    public bool CanDoTransaction(EconomyTransaction transaction)
    {
        if (!food.CanDoTransaction(transaction.FoodChange)) return false;
        if (!wood.CanDoTransaction(transaction.WoodChange)) return false;
        if (!stone.CanDoTransaction(transaction.StoneChange)) return false;
        if (!gold.CanDoTransaction(transaction.GoldChange)) return false;
        if (!mana.CanDoTransaction(transaction.ManaChange)) return false;

        return true;
    }
    
    public void DoTransaction(EconomyTransaction transaction)
    {
        food.DoTransaction(transaction.FoodChange);
        wood.DoTransaction(transaction.WoodChange);
        stone.DoTransaction(transaction.StoneChange);
        gold.DoTransaction(transaction.GoldChange);
        mana.DoTransaction(transaction.ManaChange);

        OnResourceValueChangeCallback?.Invoke();
    }

    public void RegisterForOnResourceValueChangeCallback(Action callback)
    {
        OnResourceValueChangeCallback += callback;
    }

    public void DeregisterForOnResourceValueChangeCallback(Action callback)
    {
        OnResourceValueChangeCallback -= callback;
    }

    public void OnCreepDied(EconomyTransaction bounty)
    {
        DoTransaction(bounty);
    }
}
