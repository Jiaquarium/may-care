using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_PlayerThoughtManager : MonoBehaviour
{
    public void AddThought(Model_PlayerThoughts thoughts, Model_Thought thought)
    {
        // TODO: don't hardcode this
        thoughts.uglyThoughts.Add(thought);
    }

    public int GetThoughtCount(Model_PlayerThoughts thoughts)
    {
        return thoughts.uglyThoughts.Count;
    }

    public void Setup()
    {

    }
}
