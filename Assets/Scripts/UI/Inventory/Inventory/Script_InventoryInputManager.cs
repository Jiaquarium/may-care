using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_InventoryInputManager : MonoBehaviour
{
    public Script_Game game;
    public Script_EventSystemLastSelected es;
    
    public void HandleExitInput()
    {
        if (Input.GetButtonDown("Inventory") || Input.GetButtonDown("Cancel"))
        {
            game.CloseInventory();
            es.lastSelected = null;
        }
    }

    public void Setup()
    {
        game = FindObjectOfType<Script_Game>();
    }
}
