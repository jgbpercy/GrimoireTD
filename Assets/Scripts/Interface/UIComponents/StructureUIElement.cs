using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructureUIElement : MonoBehaviour {

    [HideInInspector]
    public int indexInStructureList;

    private InterfaceController interfaceController;

    private void Start()
    {
        interfaceController = InterfaceController.Instance;
    }

    public void SendClickToInterfaceController()
    {
        interfaceController.SelectStructureToBuild(indexInStructureList);
    }
}
