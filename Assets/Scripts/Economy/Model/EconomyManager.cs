using UnityEngine;
using System;

public class EconomyManager : SingletonMonobehaviour<EconomyManager> {

    [SerializeField]
    private int maxFood;
    [SerializeField]
    private int startingFood;
    [SerializeField]
    private int maxWood;
    [SerializeField]
    private int startingWood;
    [SerializeField]
    private int maxStone;
    [SerializeField]
    private int startingStone;
    [SerializeField]
    private int maxGold;
    [SerializeField]
    private int startingGold;
    [SerializeField]
    private int maxMana;
    [SerializeField]
    private int startingMana;

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

    void Start ()
    {
        CDebug.Log(CDebug.applicationLoading, "Econonmy Manager Start");

        food = new Resource("Food", maxFood, startingFood);
        wood = new Resource("Wood", maxWood, startingWood);
        stone = new Resource("Stone", maxStone, startingStone);
        gold = new Resource("Gold", maxGold, startingGold);
        mana = new Resource("Mana", maxMana, startingMana);

        if ( OnResourceValueChangeCallback != null )
        {
            OnResourceValueChangeCallback();
        }
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

        if (OnResourceValueChangeCallback != null)
        {
            OnResourceValueChangeCallback();
        }
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
