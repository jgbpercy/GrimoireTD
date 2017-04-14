using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableInMode : MonoBehaviour {

    [SerializeField]
    private GameMode enabledInMode;
    private InterfaceManager interfaceManager;

	private void Start () {

        interfaceManager = GameObject.Find("GameManager").GetComponent<InterfaceManager>();

        interfaceManager.registerAsEnabledInMode(gameObject, enabledInMode);

        if ( enabledInMode == GameMode.DEFEND )
        {
            gameObject.SetActive(false);
        }
	}

}
