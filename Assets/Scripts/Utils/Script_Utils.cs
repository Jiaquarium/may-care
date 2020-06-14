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

    public static Transform GetChildren(this GameObject parent)
    {
        Transform t = parent.transform;
        return t;
    }

    public static GameObject FindChildWithTag(
        this GameObject parent, string tag
    )
    {
        Transform t = parent.transform;
        foreach(Transform tr in t)
        {
            if(tr.tag == tag)
            {
                return tr.gameObject;
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
        return v;
    }

    public static void AnimatorSetDirection(Animator animator, string dir)
    {
        if (dir == "up")
        {
            animator.SetFloat("LastMoveX", 0f);
            animator.SetFloat("LastMoveZ", 1f);
            animator.SetFloat("MoveX", 0f);
            animator.SetFloat("MoveZ", 1f);
        }
        else if (dir == "down")
        {
            animator.SetFloat("LastMoveX", 0f);
            animator.SetFloat("LastMoveZ", -1f);
            animator.SetFloat("MoveX", 0f);
            animator.SetFloat("MoveZ", -1f);
        }
        else if (dir == "left")
        {
            animator.SetFloat("LastMoveX", -1f);
            animator.SetFloat("LastMoveZ", 0f);
            animator.SetFloat("MoveX", -1f);
            animator.SetFloat("MoveZ", 0f);
        }
        else if (dir == "right")
        {
            animator.SetFloat("LastMoveX", 1f);
            animator.SetFloat("LastMoveZ", 0f);
            animator.SetFloat("MoveX", 1f);
            animator.SetFloat("MoveZ", 0f);
        }
    }

    // TODO REMOVE THESE, CALL FROM DM NODE
    public static bool CheckLastNodeActionCutScene(
        Script_Game g,
        Script_DialogueManager dm,
        string s
    )
    {
        return g.state == "cut-scene"
            && g.GetPlayerIsTalking()
            && dm.dialogueSections.Count == 0
            && dm.lines.Count == 0
            && !dm.isRenderingDialogueSection
            && dm.currentNode.data.action == s;
    }

    public static string FormatString(string unformattedString)
    {
        return string.Format(
            unformattedString,
            Script_Names.Player,
            Script_Names.Melz,
            Script_Names.MelzTheGreat,
            Script_Names.Ids,
            Script_Names.Ero,
            Script_Names.SBook
        );
    }

    /// <summary>
    /// copies elements from array 1 into array 2; depends on array2 size
    /// </summary>
    /// <param name="array1">array to copy from</param>
    /// <param name="array2">array to copy into</param>
    /// <typeparam name="T">type</typeparam>
    /// <returns>array2 with copied elements</returns>
    public static T[] CopyArrayElements<T>(
        T[] array1, T[] array2
    ) where T:Component
    {
        for (int i = 0; i < Mathf.Min(array2.Length, array1.Length); i++)
        {
            array2[i] = array1[i];
        }

        return array2;
    }

    public static void MakeFontsCrispy(Font[] fonts)
    {
        foreach (Font f in fonts)
        {
            if (f)
            {
                Debug.Log("making font crispy: " + f);
                f.material.mainTexture.filterMode = FilterMode.Point;
                f.material.mainTexture.anisoLevel = 0;
            }
        }
    }
}
