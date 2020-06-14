using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Script_PlayerReflectionInteractiveMovement))]
[RequireComponent(typeof(Script_InteractionBoxController))]
public class Script_PlayerReflectionInteractive : Script_PlayerReflection
{
    public bool CanMove()
    {
        return GetComponent<Script_PlayerReflectionInteractiveMovement>().CanMove();
    }

    public void TryPushPushable(string dir)
    {
        string myFaceDirection = ToOppositeDirectionZ(dir);
        
        Script_Pushable pushable = GetComponent<Script_InteractionBoxController>()
            .GetPushable(myFaceDirection);
        if (pushable != null) pushable.Push(myFaceDirection);
    }
}
