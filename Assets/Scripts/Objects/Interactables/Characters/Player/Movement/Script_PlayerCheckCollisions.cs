using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Script_PlayerCheckCollisions : Script_CheckCollisions
{
    /// <summary>
    /// in addition to checking if on tilemap, player needs to verify
    /// entrance and exits. allow player to go onto exits and entrance tiles
    /// </summary>
    /// <param name="desiredX">int of x player is trying to move to</param>
    /// <param name="desiredZ">int of z player is trying to move to</param>
    /// <param name="tileLocation"></param>
    /// <returns>true if not on tilemap</returns>
    protected override bool CheckNotOffTilemap(
        int desiredX,
        int desiredZ,
        Vector3Int tileLocation
    )
    {
        Tilemap tileMap = Script_Game.Game.GetTileMap();
        Tilemap entrancesTileMap = Script_Game.Game.GetEntrancesTileMap();
        Tilemap[] exitsTileMaps = Script_Game.Game.GetExitsTileMaps();

        // tiles map from (xyz) to (xz)
        if (!tileMap.HasTile(tileLocation))
        {
            // check to see if current tile not in Ground Tilemap
            // is possibly in exit/entrance tilemaps
            if (entrancesTileMap != null && entrancesTileMap.HasTile(tileLocation))
            {
                return false;
            }
            
            foreach(Tilemap tm in exitsTileMaps)
            {
                if (
                    tm != null
                    && tm.HasTile(tileLocation)
                    && tm.GetComponent<Script_TileMapExitEntrance>().isDisabled)
                {
                    return true;
                }

                else if (exitsTileMaps != null && tm.HasTile(tileLocation))
                {
                    return false;
                }
            }

            return true;
        }

        return false;
    }

    /// <summary>
    /// before player moves, check if there is a pushable there you can possibly move
    /// </summary>
    /// <param name="dir"></param>
    /// <returns></returns>
    protected override bool CheckPushableBlocking(
        Vector3 dir
    )
    {
        // use player colliders to detect pushable
        // if pushable

        // pushable.push
        // check if pushable can be pushed

        // if not return true
        return false;
    }
}
