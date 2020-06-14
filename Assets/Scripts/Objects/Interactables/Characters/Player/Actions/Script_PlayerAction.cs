using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Script_InteractionBoxController))]
[RequireComponent(typeof(Script_PlayerAttacks))]
[RequireComponent(typeof(Script_Player))]
public class Script_PlayerAction : MonoBehaviour
{
    private Script_PlayerAttacks attacks;
    private Script_Game game;
    private Script_Player player;
    private Dictionary<string, Vector3> Directions;
    private Script_InteractionBoxController interactionBoxController;
    
    public void HandleActionInput(string facingDirection, Vector3 location)
    {   
        /// <summary>
        /// interact state available actions
        /// </summary>
        if (player.GetIsInteract())
        {
            if (Input.GetButtonDown(Const_KeyCodes.Action1))
                HandleStartDialogue(facingDirection, location);
            else if (Input.GetButtonDown(Const_KeyCodes.Skip))
                HandleSkipDialogue(facingDirection, location);
            else if (Input.GetButtonDown(Const_KeyCodes.Action2))
                
                /// attack handler here, to choose which attack will be done
                attacks.Eat(facingDirection);

            else if (Input.GetButtonDown(Const_KeyCodes.Inventory))
                OpenInventory();
        }
        /// <summary>
        /// dialogue state ""
        /// </summary>
        else if (player.GetIsTalking())
        {
            if (Input.GetButtonDown(Const_KeyCodes.Action1))
                HandleStartDialogue(facingDirection, location);
            else if (Input.GetButtonDown(Const_KeyCodes.Skip))
                HandleSkipDialogue(facingDirection, location);
        }
    }

    void HandleStartDialogue(string facingDirection, Vector3 location)
    {
        // TODO REFACTOR USE COLLISIONS FOR EVERYTHING
        if (!DetectNPCProximity(facingDirection, Const_KeyCodes.Action1, location))
        {
            if (!DetectInteractableObjectProximity(facingDirection, Const_KeyCodes.Action1, location))
                DetectSavePoint(Const_KeyCodes.Action1, facingDirection);
        }
    }

    void HandleSkipDialogue(string facingDirection, Vector3 location)
    {
        if (!DetectNPCProximity(facingDirection, Const_KeyCodes.Skip, location))
        {
            if (!DetectInteractableObjectProximity(facingDirection, Const_KeyCodes.Skip, location))
                DetectSavePoint(Const_KeyCodes.Skip, facingDirection);
        }
    }

    bool DetectNPCProximity(
        string direction,
        string action,
        Vector3 location
    )
    {
        // using facing direction, get an activation location
        // use this location to check if there's an NPC that's active there
        Vector3 desiredLocation = location + Directions[direction];

        return game.HandleActionToNPC(desiredLocation, action);
    }

    bool DetectInteractableObjectProximity(
        string direction,
        string action,
        Vector3 location
    )
    {
        Vector3 desiredLocation = location + Directions[direction];
        
        return game.HandleActionToInteractableObject(desiredLocation, action);
    }

    void OpenInventory()
    {
        game.OpenInventory();
        player.SetState(Const_States_Player.Inventory);
    }

    public void DetectSavePoint(string action, string dir)
    {
        Script_SavePoint savePoint = interactionBoxController.GetSavePoint(dir);
        if (savePoint != null)  savePoint.HandleAction(action);
    }

    public void TryPushPushable(string dir)
    {
        Script_Pushable pushable = interactionBoxController.GetPushable(dir);
        if (pushable != null) pushable.Push(dir);
    }

    public void Setup(Script_Game _game)
    {
        attacks = GetComponent<Script_PlayerAttacks>();
        player = GetComponent<Script_Player>(); 
        interactionBoxController = GetComponent<Script_InteractionBoxController>();
        game = _game;
        Directions = Script_Utils.GetDirectionToVectorDict();
    }
}
