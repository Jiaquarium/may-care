using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Script_DemonStats : Script_CharacterStats
{
    protected override void Die()
    {
        Debug.Log($"{transform.name} DEMON OVERRIDE Die() called");
        GetComponent<Script_Demon>().Die();
    }
}

[CustomEditor(typeof(Script_DemonStats)), CanEditMultipleObjects]
public class Script_DemonStatsTester : Editor
{
    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        Script_DemonStats stats = (Script_DemonStats)target;
        if (GUILayout.Button("Setup()"))
        {
            stats.Setup();
        }
        if (GUILayout.Button("Hurt(1)"))
        {
            stats.Hurt(1);
        }
    }
}