using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Script_ReflectionCheckCollisions : Script_CheckCollisions
{
    protected override bool CheckNotOffTilemap(int desiredX, int desiredZ, Vector3Int tileLocation)
    {
        return false;
    }
}
