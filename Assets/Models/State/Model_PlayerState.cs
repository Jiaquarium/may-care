using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Model_PlayerState
{
    public string name;
    public int? spawnX;
    public int? spawnY;
    public int? spawnZ;
    public string faceDirection;
    public Model_PlayerStats stats;

    public Model_PlayerState(
        string _name,
        int? _spawnX,
        int? _spawnY,
        int? _spawnZ,
        string _faceDirection
    )
    {
        name = _name;
        spawnX = _spawnX;
        spawnY = _spawnY;
        spawnZ = _spawnZ;
        faceDirection = _faceDirection;
    }
}