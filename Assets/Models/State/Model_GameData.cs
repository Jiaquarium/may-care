using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Model_GameData
{
    public int level { get; set; }

    public Model_GameData(
        int _level
    )
    {
        level = _level;
    }
}