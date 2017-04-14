using UnityEngine;

[System.Serializable]
public class Resource {

    [SerializeField]
    private string name;

    public string Name
    {
        get
        {
            return name;
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
        this.name = name;
        this.maxAmount = maxAmount;
        amountOwned = baseAmount <= maxAmount ? baseAmount : maxAmount;
    }

    public bool Increase(int byAmount)
    {
        if ( amountOwned + byAmount > maxAmount )
        {
            amountOwned = maxAmount;
            return false;
        }
        else
        {
            amountOwned += byAmount;
            return true;
        }
    }

    public bool TryPay(int amount)
    {
        if ( amountOwned - amount < 0 )
        {
            return false;
        }
        else
        {
            amountOwned -= amount;
            return true;
        }
    }

    public bool CanPay(int amount)
    {
        if (amountOwned - amount < 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public bool Decrease(int byAmount)
    {
        if ( amountOwned - byAmount < 0 )
        {
            amountOwned = 0;
            return false;
        }
        else
        {
            amountOwned -= byAmount;
            return true;
        }
    }

}
