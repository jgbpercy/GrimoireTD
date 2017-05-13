using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

//TODO: Make this not crap
[CustomEditor(typeof(UIModeToggler))]
public class UIModeTogglerEditor : Editor {

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if ( GUILayout.Button("Switch") )
        {
            UIModeToggler toggler = (UIModeToggler)target;
            toggler.SwitchMode();
        }
    }
}
