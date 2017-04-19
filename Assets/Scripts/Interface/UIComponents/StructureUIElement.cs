using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructureUIElement : MonoBehaviour {

    [HideInInspector]
    public int indexInStructureList;

    private InterfaceController interfaceManager;

    private void Awake()
    {
        interfaceManager = GameObject.Find("GameManager").GetComponent<InterfaceController>();
    }

    public void SendClickToInterfaceManager()
    {
        interfaceManager.SelectStructureToBuild(indexInStructureList);
    }
}
