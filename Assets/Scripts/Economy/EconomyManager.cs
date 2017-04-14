using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ResNames
{
    GOLD,
    STONE
}

public class EconomyManager : MonoBehaviour {

    [SerializeField]
    private Resource[] resources;

    // until I bother serialising dictionaries properly
    private Dictionary<GameObject, int[]> towerCostsDictionary;

    private Dictionary<string, int[]> creepBountiesDictionary;

    [System.Serializable]
    private class TowerCost
    {
        public GameObject tower;
        public int[] costs;
    }
    [SerializeField]
    private TowerCost[] towerCostsArray;

    [System.Serializable]
    private class CreepBounty
    {
        public string creep;
        public int[] bounty;
    }
    [SerializeField]
    private CreepBounty[] creepBountiesArray;

    void Awake () {

        resources = new Resource[2];
        resources[0] = new Resource("Gold", 20, 5);
        resources[1] = new Resource("Stone", 20, 10);

        towerCostsDictionary = towerCostsObjectsToDictionary();

        creepBountiesDictionary = creepBountiesObjectsToDictionary();
	}

    private Dictionary<GameObject, int[]> towerCostsObjectsToDictionary()
    {
        towerCostsDictionary = new Dictionary<GameObject, int[]>();

        foreach (TowerCost towerCost in towerCostsArray)
        {
            towerCostsDictionary.Add(towerCost.tower, towerCost.costs);
        }

        return towerCostsDictionary;
    }

    private Dictionary<string, int[]> creepBountiesObjectsToDictionary()
    {
        creepBountiesDictionary = new Dictionary<string, int[]>();

        foreach (CreepBounty creepBounty in creepBountiesArray)
        {
            creepBountiesDictionary.Add(creepBounty.creep, creepBounty.bounty);
        }

        return creepBountiesDictionary;
    }

    public string ResourceUIText()
    {
        string resourceUIText = "";

        foreach(Resource resource in resources)
        {
            resourceUIText += resource.Name + ": <i>" + resource.AmountOwned + "</i>\n";
        }

        return resourceUIText;
    }

    public string TowerCostUIText(GameObject towerToCost)
    {
        string towerCostUIText = "";

        for (int i = 0; i < resources.Length; i++)
        {
            towerCostUIText += resources[i].Name + ": <i>" + towerCostsDictionary[towerToCost][i] + "</i>, ";
        }

        return towerCostUIText;
    }
    /*
    public int ResourceOwnedAmount(int resourceIndex)
    {
        return resources[resourceIndex].AmountOwned;
    }

    public bool IncreaseResouce(int resourceIndex, int byAmount)
    {
        return resources[resourceIndex].Increase(byAmount);
    }

    public bool TryPayResouce(int resourceIndex, int amount)
    {
        return resources[resourceIndex].TryPay(amount);
    }

    public bool CanPayResource(int resourceIndex, int amount)
    {
        return resources[resourceIndex].CanPay(amount);
    }

    public bool DecreaseResource(int resourceIndex, int byAmount)
    {
        return resources[resourceIndex].Decrease(byAmount);
    }
    */
    private bool CanPayCost(int[] cost)
    {
        for (int i = 0; i < cost.Length; i++)
        {
            if ( !resources[i].CanPay(cost[i]) )
            {
                return false;
            }
        }

        return true;
    }
    
    private bool TryPayCost(int[] cost)
    {
        if ( CanPayCost(cost) )
        {
            for (int i = 0; i < cost.Length; i++)
            {
                resources[i].TryPay(cost[i]);
            }
            return true;
        }

        return false;
    }

    public bool TryPayTowerCost(GameObject tower)
    {
        int[] cost;

        towerCostsDictionary.TryGetValue(tower, out cost);

        if ( cost != null )
        {
            return TryPayCost(cost);
        } else
        {
            Debug.Log("Attempted to find cost of tower that was not in costs dictionary");
            throw new System.Exception("Attempted to find cost of tower that was not in costs dictionary");
        }
        
    }

    public void GainCreepBounty(string creepObjectName)
    {
        int[] bounty;

        creepBountiesDictionary.TryGetValue(creepObjectName, out bounty);

        if (bounty != null)
        {
            for (int i = 0; i < bounty.Length; i++)
            {
                resources[i].Increase(bounty[i]);
            }
        }
        else
        {
            Debug.Log("Attempted to find bounty of creep that was not in bounties dictionary");
            throw new System.Exception("Attempted to find bounty of creep that was not in bounties dictionary");
        }

    }

    public int numberOfResources()
    {
        return resources.Length;
    }

    public string[] ResourceNames()
    {
        string[] resourceNames = new string[resources.Length];

        for (int i = 0; i < resources.Length; i++)
        {
            resourceNames[i] = resources[i].Name;
        }

        return resourceNames;
    }

}
