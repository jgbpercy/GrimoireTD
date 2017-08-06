using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EconomyManager : SingletonMonobehaviour<EconomyManager>
{
    [SerializeField]
    private SoResource[] resources;

    public IReadOnlyCollection<IResource> Resources
    {
        get
        {
            return resources;
        }
    }

    public IEconomyTransaction ResourcesAsTransaction
    {
        get
        {
            var resourceTransactions = resources.Select(x => new CResourceTransaction(x, x.AmountOwned));

            return new CEconomyTransaction(resourceTransactions);
        }
    }

    private Action<IResource, int, int> OnAnyResourceChangedCallback;

    private void Start()
    {
        CDebug.Log(CDebug.applicationLoading, "Econonmy Manager Start");

        foreach (IResource resource in Resources)
        {
            resource.RegisterForOnResourceChangedCallback((int byAmount, int newAmount) => OnResourceChanged(resource, byAmount, newAmount));
        }

        MapGenerator.Instance.Level.StartingResources.DoTransaction();
    }

    private void OnResourceChanged(IResource resource, int byAmount, int newAmount)
    {
        OnAnyResourceChangedCallback?.Invoke(resource, byAmount, newAmount);
    }

    public void RegisterForOnAnyResourceChangedCallback(Action<IResource, int, int> callback)
    {
        OnAnyResourceChangedCallback += callback;
    }

    public void DeregisterForOnAnyResourceChangedCallback(Action<IResource, int, int> callback)
    {
        OnAnyResourceChangedCallback -= callback;
    }
}
