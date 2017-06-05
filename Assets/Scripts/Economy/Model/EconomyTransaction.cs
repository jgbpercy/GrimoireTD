using UnityEngine;

[System.Serializable]
public struct EconomyTransaction {

    public EconomyTransaction(int foodChange, int woodChange, int stoneChange, int goldChange, int manaChange)
    {
        this.foodChange = foodChange;
        this.woodChange = woodChange;
        this.stoneChange = stoneChange;
        this.goldChange = goldChange;
        this.manaChange = manaChange;
    }

    public enum StringFormats
    {
        FullNameSingleLine,
        FullNameLineBreaks,
        ShortNameSingleLine,
        ShortNameLineBreaks
    }

    [SerializeField]
    int foodChange;
    [SerializeField]
    int woodChange;
    [SerializeField]
    int stoneChange;
    [SerializeField]
    int goldChange;
    [SerializeField]
    int manaChange;
        
    public int FoodChange
    {
        get
        {
            return foodChange;
        }
    }

    public int WoodChange
    {
        get
        {
            return woodChange;
        }
    }

    public int StoneChange
    {
        get
        {
            return stoneChange;
        }
    }

    public int GoldChange
    {
        get
        {
            return goldChange;
        }
    }

    public int ManaChange
    {
        get
        {
            return manaChange;
        }
    }

    public override string ToString()
    {
        return "Food: " + goldChange + ", Wood: " + woodChange + ", Stone: " + ", Gold: " + goldChange + ", Mana: " + manaChange;
    }

    public string ToString(bool absolute)
    {
        return ToString(StringFormats.FullNameSingleLine, absolute);
    }

    public string ToString(StringFormats format, bool absolute)
    {
        EconomyTransaction displayTransaction;

        if ( absolute )
        {
            displayTransaction = this.Abs();
        }
        else
        {
            displayTransaction = this;
        }

        if ( format == StringFormats.FullNameSingleLine )
        {
            return "Food: " + displayTransaction.foodChange + ", Wood: " + displayTransaction.woodChange + ", Stone: " + displayTransaction.stoneChange + ", Gold: " + displayTransaction.goldChange + ", Mana: " + displayTransaction.manaChange;
        }
        else if ( format == StringFormats.FullNameLineBreaks)
        {
            return "Food: " + displayTransaction.foodChange + "\nWood: " + displayTransaction.woodChange + "\nStone: " + displayTransaction.stoneChange + "\nGold: " + displayTransaction.goldChange + "\nMana: " + displayTransaction.manaChange;
        }
        else if ( format == StringFormats.ShortNameSingleLine )
        {
            return "F: " + displayTransaction.foodChange + ", W: " + displayTransaction.woodChange + ", S: " + displayTransaction.stoneChange + ", G: " + displayTransaction.goldChange + ", M: " + displayTransaction.manaChange;
        }
        else if ( format == StringFormats.ShortNameLineBreaks )
        {
            return "F: " + displayTransaction.foodChange + "\nW: " + displayTransaction.woodChange + "\nS: " + displayTransaction.stoneChange + "\nG: " + displayTransaction.goldChange + "\nM: " + displayTransaction.manaChange;
        }
        else
        {
            return ToString(absolute);
        }
    }

    public EconomyTransaction Abs()
    {
        return new EconomyTransaction(Mathf.Abs(foodChange), Mathf.Abs(woodChange), Mathf.Abs(stoneChange), Mathf.Abs(goldChange), Mathf.Abs(manaChange));
    }

}
