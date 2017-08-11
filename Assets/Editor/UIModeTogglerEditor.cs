using UnityEngine;
using UnityEditor;
using GrimoireTD.Technical;

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
