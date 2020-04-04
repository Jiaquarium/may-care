using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Model_PlayerState
{
    public string name;
    public int? spawnX;
    public int? spawnZ;
    public string faceDirection;

    public Model_PlayerState(
        string _name,
        int? _spawnX,
        int? _spawnZ,
        string _faceDirection
    )
    {
        name = _name;
        spawnX = _spawnX;
        spawnZ = _spawnZ;
        faceDirection = _faceDirection;    
    }
}