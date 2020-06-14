using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Script_PlayerStats : Script_CharacterStats
{
    protected override void Die()
    { 
        Debug.Log($"{transform.name} PLAYER OVERRIDE Die() called");
    }
}

[CustomEditor(typeof(Script_PlayerStats)), CanEditMultipleObjects]
public class Script_PlayerStatsTester : Editor
{
    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        Script_PlayerStats stats = (Script_PlayerStats)target;
        if (GUILayout.Button("Hurt(1)"))
        {
            stats.Hurt(1);
        }
    }
}
