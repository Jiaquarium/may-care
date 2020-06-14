using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Model_Stat
{
    [SerializeField] private int baseVal;

    public int GetVal()
    {
        return baseVal;
    }
}
