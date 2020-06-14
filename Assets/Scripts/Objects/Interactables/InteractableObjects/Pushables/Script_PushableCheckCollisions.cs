using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Script_PushableCheckCollisions : Script_CheckCollisions
{
    protected override bool CheckNotOffTilemap(
        int desiredX,
        int desiredZ,
        Vector3Int tileLocation
    )
    {
        Tilemap tileMap = Script_Game.Game.GetTileMap();
        Tilemap pushablesTileMap = Script_Game.Game.GetPushablesTileMap();;

        if (!tileMap.HasTile(tileLocation) && !pushablesTileMap.HasTile(tileLocation))
                        return true;
        else            return false;
    }
}
