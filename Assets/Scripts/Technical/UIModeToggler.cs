using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO: Make this not crap
[ExecuteInEditMode]
public class UIModeToggler : MonoBehaviour {

    private enum UIMode
    {
        BUILD,
        DEFEND,
        RUN
    }

    private List<GameObject> enabledInDefendMode = new List<GameObject>();
    private List<GameObject> enabledInBuildMode = new List<GameObject>();

    [SerializeField]
    private UIMode uiMode;

    public void SwitchMode()
    {
        EnableInMode[] enabledInModeObjects = GameObject.Find("CanvasMainUI").GetComponentsInChildren<EnableInMode>(true);

        foreach( EnableInMode enableInMode in enabledInModeObjects)
        {
            if (enableInMode.EnabledInMode == GameMode.BUILD && !enabledInBuildMode.Contains(enableInMode.gameObject))
            {
                enabledInBuildMode.Add(enableInMode.gameObject);
            }
            else if ( enableInMode.EnabledInMode == GameMode.DEFEND && !enabledInDefendMode.Contains(enableInMode.gameObject))
            {
                enabledInDefendMode.Add(enableInMode.gameObject);
            }
        }

        foreach(GameObject enableInBuildMode in enabledInBuildMode )
        {
            if ( uiMode == UIMode.RUN || uiMode == UIMode.BUILD )
            {
                enableInBuildMode.SetActive(true);
            }
            else
            {
                enableInBuildMode.SetActive(false);
            }
        }

        foreach(GameObject enableInDefendMode in enabledInDefendMode)
        {
            if (uiMode == UIMode.RUN || uiMode == UIMode.DEFEND)
            {
                enableInDefendMode.SetActive(true);
            }
            else
            {
                enableInDefendMode.SetActive(false);
            }
        }
    }
}