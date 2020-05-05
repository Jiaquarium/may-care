using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_LevelBehavior_1 : Script_LevelBehavior
{
    /* =======================================================================
        STATE DATA
    ======================================================================= */
    public bool isDone = false;
    public bool isExitsDisabled = true;
    /* ======================================================================= */
    
    
    public Vector3[] triggerLocations;
    public Model_Dialogue dialogue;
    

    public Script_BgThemePlayer EroBgThemePlayerPrefab;
    

    protected override void HandleTriggerLocations() {
        if (isDone)   return;

        for (int i = 0; i < triggerLocations.Length; i++)
        {
            if (
                game.GetPlayerLocation() == triggerLocations[i]
                && game.state == "interact"
            )
            {
                game.PauseBgMusic();
                game.PlayNPCBgTheme(EroBgThemePlayerPrefab);
                game.ChangeStateCutScene();
                
                game.PlayerFaceDirection("down");
                game.ChangeCameraTargetToNPC(0);
                game.StartDialogue(dialogue);
            }
        }
    }

    protected override void HandleAction()
    {
        if (game.state == "cut-scene" && !game.GetPlayerIsTalking())
        {
            print("trigger move");
            isDone = true;

            isExitsDisabled = false;
            game.EnableExits();
            game.ChangeStateCutSceneNPCMoving();
            game.TriggerMovingNPCMove(0);

            // ero then leaves through door
        }

        if (Input.GetButtonDown(Script_KeyCodes.Action1) && game.state == "cut-scene" && !isDone)
        {
            game.HandleContinuingDialogueActions(Script_KeyCodes.Action1);
        }

        if (Input.GetButtonDown(Script_KeyCodes.Skip) && game.state == "cut-scene" && !isDone)
            game.HandleContinuingDialogueActions(Script_KeyCodes.Skip);
    }

    public override void Setup()
    {
        if (isExitsDisabled)    game.DisableExits();
        else game.EnableExits();

        game.CreateInteractableObjects(null);

        // base.Setup();
        if (!isDone)
        {
            game.CreateNPCs();
        }
    }
}
