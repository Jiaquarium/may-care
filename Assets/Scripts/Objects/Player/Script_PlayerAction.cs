﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_PlayerAction : MonoBehaviour
{
    private Script_Game game;
    private Dictionary<string, Vector3> Directions = new Dictionary<string, Vector3>();
    
    public void HandleActionInput(string facingDirection, Vector3 location)
    {   
        // talk and continue
        // Action1: x, space (interact)
        if (Input.GetButtonDown("Action1"))
        {
            // TODO REFACTOR: refactor split out checking locs?
            bool isNPC = DetectNPCProximity(facingDirection, "Action1", location);
            if (!isNPC) DetectInteractableObjectProximity(facingDirection, "Action1", location);
        }
        else if (Input.GetButtonDown("Submit"))
        {
            DetectNPCProximity(facingDirection, "Submit", location);
        }
        else if (Input.GetButtonDown("Action2"))
        {
            DetectDemonProximity(facingDirection, "Action2", location);
        }
        else if (Input.GetButtonDown("Inventory"))
        {
            OpenInventory();        
        }
    }

    public void HandleInventoryActionsInput()
    {
        if (Input.GetButtonDown("Inventory") || Input.GetButtonDown("Cancel"))
        {
            CloseInventory();
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

    bool DetectDemonProximity(
        string direction,
        string action,
        Vector3 location
    )
    {
        Vector3 desiredLocation = location + Directions[direction];

        return game.HandleActionToDemon(desiredLocation, action);
    }

    void OpenInventory()
    {
        game.OpenPlayerThoughtsInventory();
    }

    void CloseInventory()
    {
        game.ClosePlayerThoughtsInventory();
    }

    public void Setup(
        Script_Game _game,
        Dictionary<string, Vector3> _Directions
    )
    {
        print("setting up Script_PlayerAction");
        game = _game;
        Directions = _Directions;
    }
}