using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Script_ReflectionCheckCollisions))]
[RequireComponent(typeof(Script_InteractionBoxController))]
public class Script_PlayerReflectionInteractiveMovement : Script_PlayerReflectionMovement
{
    [SerializeField] private Script_InteractionBoxController interactionBoxController;
    
    /// <summary>
    /// Player will dictate the moves by checking CanMove()
    /// ActuallyMove() here will set the proper active interaction box
    /// and match/"reflect" the player's position
    /// </summary>
    protected override void ActuallyMove()
    {
        string myFacingDir = ToOppositeDirectionZ(player.facingDirection);
        HandleActiveInteractionBox(myFacingDir);
        base.ActuallyMove();
    }

    public bool CanMove()
    {
        string myFacingDir = ToOppositeDirectionZ(player.facingDirection);
        HandleActiveInteractionBox(myFacingDir);

        if (
            GetComponent<Script_CheckCollisions>().CheckCollisions(transform.position, myFacingDir)
        )
        {
            return false;
        }

        return true;
    }

    void HandleActiveInteractionBox(string dir)
    {
        interactionBoxController.HandleActiveInteractionBox(dir);
    }
}
