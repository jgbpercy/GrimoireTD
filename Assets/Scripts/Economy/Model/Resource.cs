using UnityEngine;

public class Resource {

    [SerializeField]
    private string nameInGame;

    public string NameInGame
    {
        get
        {
            return nameInGame;
        }
    }

    [SerializeField]
    private int amountOwned;

    public int AmountOwned
    {
        get
        {
            return amountOwned;
        }
    }

    [SerializeField]
    private int maxAmount;

    public Resource(string name, int maxAmount, int baseAmount)
    {
        nameInGame = name;
        this.maxAmount = maxAmount;
        amountOwned = baseAmount <= maxAmount ? baseAmount : maxAmount;
    }

    public bool CanDoTransaction(int amount)
    {
        int resultingAmount = amountOwned + amount;
        if ( resultingAmount > maxAmount || resultingAmount < 0 )
        {
            return false;
        }

        return true;
    }

    public void DoTransaction(int amount)
    {
        amountOwned += amount;
    }
}
