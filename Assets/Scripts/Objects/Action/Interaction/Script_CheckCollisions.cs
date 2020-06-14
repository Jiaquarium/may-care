using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Script_CheckCollisions : MonoBehaviour
{
    [SerializeField] protected Script_InteractionBoxController interactionBoxController;

    /// <summary>
    /// 
    /// check if allowed to move to desired grid tile
    /// </summary>
    /// <param name="desiredDirection"></param>
    /// <returns>true if not allowed to move to that space</returns>
    public bool CheckCollisions(Vector3 currentLocation, string dir)
    {
        Vector3 desiredDirection = Script_Utils.GetDirectionToVectorDict()[dir];

        int desiredX = (int)Mathf.Round((currentLocation + desiredDirection).x);
        int desiredZ = (int)Mathf.Round((currentLocation + desiredDirection).z);

        // offset for grid world adjustments
        int adjustedDesiredX = desiredX - (int)Script_Game.Game.grid.transform.position.x;
        int adjustedDesiredZ = desiredZ - (int)Script_Game.Game.grid.transform.position.z;
        
        Vector3Int tileLocation = new Vector3Int(adjustedDesiredX, adjustedDesiredZ, 0);
        
        Tilemap tileMap = Script_Game.Game.GetTileMap();
        
        if (CheckNotOffTilemap(desiredX, desiredZ, tileLocation))     return true;
        if (CheckInteractableBlocking(dir))                           return true;

        return false;
    }

    protected virtual bool CheckNotOffTilemap(int desiredX, int desiredZ, Vector3Int tileLocation)
    {
        Tilemap tileMap = Script_Game.Game.GetTileMap();

        if (!tileMap.HasTile(tileLocation))     return true;
        else                                    return false;
    }

    protected virtual bool CheckInteractableBlocking(string dir)
    {
        List<Script_Interactable> interactables = interactionBoxController.GetInteractables(dir);
        return interactables.Count > 0;
    }

    protected virtual bool CheckPushableBlocking(Vector3 desiredDirection) {
        // check trigger that is in that direction

        return false;
    }
}
