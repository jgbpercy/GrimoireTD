using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerUIElement : MonoBehaviour {

    [HideInInspector]
    public int indexInTowerList;

    private InterfaceManager interfaceManager;

    private void Awake()
    {
        interfaceManager = GameObject.Find("GameManager").GetComponent<InterfaceManager>();
    }

    public void SendClickToInterfaceManager()
    {
        interfaceManager.SelectTowerToBuild(indexInTowerList);
    }
}
