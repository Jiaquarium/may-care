using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Script_CharacterStats : MonoBehaviour
{
    public Model_CharacterStats stats;
    public int currentHp;
    
    public void Hurt(int dmg)
    {
        // reduce dmg by defense
        dmg -= stats.defense.GetVal();
        dmg = Mathf.Clamp(dmg, 0, int.MaxValue);

        // reduce health
        currentHp -= dmg;
        currentHp = Mathf.Clamp(currentHp, 0, int.MaxValue);
        Debug.Log($"{transform.name} took damage {dmg}. currentHp: {currentHp}");

        if (currentHp == 0)
        {
            Die();
        }
    }

    /// <summary>
    /// override this for custom way to die e.g. cutscene, drop item, etc.
    /// </summary>
    protected virtual void Die() { }

    public void Setup()
    {
        currentHp = stats.maxHp.GetVal();
    }
}

[CustomEditor(typeof(Script_CharacterStats))]
public class Script_StatsTester : Editor
{
    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        Script_CharacterStats stats = (Script_CharacterStats)target;
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
