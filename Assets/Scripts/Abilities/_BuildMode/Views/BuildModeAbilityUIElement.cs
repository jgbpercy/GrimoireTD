using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildModeAbilityUIElement : MonoBehaviour {

    [HideInInspector]
    public int indexInAbilityList;

    private InterfaceController interfaceController;

    private void Start()
    {
        interfaceController = InterfaceController.Instance;
    }

    public void SendClickToInterfaceController()
    {
        interfaceController.ActivateBuildModeAbility(indexInAbilityList);
    }

}
