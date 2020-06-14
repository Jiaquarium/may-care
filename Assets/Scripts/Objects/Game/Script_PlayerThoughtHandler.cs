using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_PlayerThoughtHandler : MonoBehaviour
{
    public void AddPlayerThought(
        Model_Thought thought,
        Model_PlayerThoughts thoughts
    )
    {
        // TODO: DON'T HARDCODE THIS
        thoughts.uglyThoughts.Add(thought);
    }

    public int GetThoughtsCount(Model_PlayerThoughts thoughts)
    {
        return thoughts.uglyThoughts.Count;
    }
}
