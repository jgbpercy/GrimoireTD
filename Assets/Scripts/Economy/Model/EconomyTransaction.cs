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
}
