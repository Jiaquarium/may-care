using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Script_SaveGameControl))]
public class Script_SaveTester : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Script_SaveGameControl control = (Script_SaveGameControl)target;
        if (GUILayout.Button("Save Game"))
        {
            control.Save();
        }

        if (GUILayout.Button("Delete Game Data"))
        {
            control.Delete();
        }
    }
}
