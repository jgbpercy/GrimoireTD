using System;
using UnityEngine;

[Serializable]
public class SResourceTransaction : IResourceTransaction {

    [SerializeField]
    private SoResource resource;

    [SerializeField]
    private int amount;

    public IResource Resource
    {
        get
        {
            return resource;
        }
    }

    public int Amount
    {
        get
        {
            return amount;
        }
    }

    public bool CanDoTransaction()
    {
        return CResourceTransaction.CanDoTransaction(this);
    }

    public void DoTransaction()
    {
        CResourceTransaction.DoTransaction(this);
    }
}
