using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableInMode : MonoBehaviour {

    [SerializeField]
    private GameMode enabledInMode;
    private InterfaceController interfaceManager;

	private void Start () {

        interfaceManager = GameObject.Find("GameManager").GetComponent<InterfaceController>();

        interfaceManager.RegisterAsEnabledInMode(gameObject, enabledInMode);

        if ( enabledInMode == GameMode.DEFEND )
        {
            gameObject.SetActive(false);
        }
	}

}
