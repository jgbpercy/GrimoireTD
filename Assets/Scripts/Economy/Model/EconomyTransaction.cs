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
        return "(F: " + goldChange + ", W: " + woodChange + ", S: " + stoneChange + ", G: " + goldChange + ", M: " + manaChange + ")";
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
            displayTransaction = Abs();
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

    public static EconomyTransaction operator +(EconomyTransaction transaction1, EconomyTransaction transaction2)
    {
        return new EconomyTransaction(
            transaction1.foodChange + transaction2.foodChange,
            transaction1.woodChange + transaction2.woodChange,
            transaction1.stoneChange + transaction2.stoneChange,
            transaction1.goldChange + transaction2.goldChange,
            transaction1.manaChange + transaction2.manaChange
            );
    }

    public static EconomyTransaction operator -(EconomyTransaction transaction1, EconomyTransaction transaction2)
    {
        return new EconomyTransaction(
            transaction1.foodChange - transaction2.foodChange,
            transaction1.woodChange - transaction2.woodChange,
            transaction1.stoneChange - transaction2.stoneChange,
            transaction1.goldChange - transaction2.goldChange,
            transaction1.manaChange - transaction2.manaChange
            );
    }

    public EconomyTransaction Multiply(int factor)
    {
        return new EconomyTransaction(
            foodChange * factor,
            woodChange * factor,
            stoneChange * factor,
            goldChange * factor,
            manaChange * factor
            );
    }

    public enum RoundingModes
    {
        NEAREST,
        UP,
        DOWN
    }

    public EconomyTransaction Multiply(float factor, RoundingModes roundingMode)
    {
        if (roundingMode == RoundingModes.NEAREST)
        {
            return new EconomyTransaction(
                Mathf.RoundToInt(foodChange * factor),
                Mathf.RoundToInt(woodChange * factor),
                Mathf.RoundToInt(stoneChange * factor),
                Mathf.RoundToInt(goldChange * factor),
                Mathf.RoundToInt(manaChange * factor)
            );
        }
        else if (roundingMode == RoundingModes.UP)
        {
            return new EconomyTransaction(
                Mathf.CeilToInt(foodChange * factor),
                Mathf.CeilToInt(woodChange * factor),
                Mathf.CeilToInt(stoneChange * factor),
                Mathf.CeilToInt(goldChange * factor),
                Mathf.CeilToInt(manaChange * factor)
            );
        }
        else if (roundingMode == RoundingModes.DOWN)
        {
            return new EconomyTransaction(
                Mathf.FloorToInt(foodChange * factor),
                Mathf.FloorToInt(woodChange * factor),
                Mathf.FloorToInt(stoneChange * factor),
                Mathf.FloorToInt(goldChange * factor),
                Mathf.FloorToInt(manaChange * factor)
            );
        }
        else
        {
            return Multiply(factor); //Do default
        }
    }

    public EconomyTransaction Multiply(float factor)
    {
        return Multiply(factor, RoundingModes.NEAREST);
    }
}
