using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Script_Utils
{
    public static T FindComponentInChildWithTag<T>(
        this GameObject parent, string tag
    ) where T:Component
    {
        Transform t = parent.transform;
        foreach(Transform tr in t)
        {
            if(tr.tag == tag)
            {
                return tr.GetComponent<T>();
            }
        }
        return null;
    }

    public static Dictionary<string, Vector3> GetDirectionToVectorDict()
    {
        return new Dictionary<string, Vector3>()
        {
            {"up"       , new Vector3(0f, 0f, 1f)},
            {"down"     , new Vector3(0f, 0f, -1f)},
            {"left"     , new Vector3(-1f, 0f, 0f)},
            {"right"    , new Vector3(1f, 0f, 0f)}
        };
    }

    public static Vector3 MovesToVector(Model_MoveSet moveSet)
    {
        Dictionary<string, Vector3> directions = GetDirectionToVectorDict();
        Vector3 v = Vector3.zero;
        foreach(string m in moveSet.moves)
        {
            v = v + directions[m];
        }
        Debug.Log("v adding to NPCSpawnLocation: " + v);
        return v;
    }
}
