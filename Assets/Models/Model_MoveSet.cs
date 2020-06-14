using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Model_MoveSet
{
    public string[] moves;
    public string endFaceDirection;

    public Model_MoveSet(string[] _moves, string _endFaceDirection)
    {
        moves = _moves;
        endFaceDirection = _endFaceDirection;
    }
}
