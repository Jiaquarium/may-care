using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_LevelBehavior_1 : Script_LevelBehavior
{
    /* =======================================================================
        STATE DATA
    ======================================================================= */
    public bool isInitialized = false;
    public bool isDone = false;
    public bool isExitsDisabled = true;
    /* ======================================================================= */
    
    
    public Vector3[] triggerLocations;
    public Transform pianoTextParent;
    public Transform paintingTextParent;
    public Script_MovingNPC Ero;
    public Script_DialogueNode EroNode;
    public Script_DialogueManager dialogueManager;
    

    public Script_BgThemePlayer EroBgThemePlayerPrefab;
    
    public override void ActivateTrigger(string Id){
        if (Id == "piano" && !isDone)
        {
            game.PauseBgMusic();
            game.PlayNPCBgTheme(EroBgThemePlayerPrefab);
            game.ChangeStateCutScene();
            StartCoroutine(TriggerAction());
        }        
    }

    public override void HandleDialogueNodeAction(string a)
    {
        if (a == "exit")
        {
            isDone = true;

            isExitsDisabled = false;
            game.DisableExits(false, 0);
            game.ChangeStateCutSceneNPCMoving();
            game.TriggerMovingNPCMove(0);
            // ero then leaves through door
        }
    }

    IEnumerator TriggerAction()
    {
        yield return new WaitForSeconds(Const_WaitTimes.OnTrigger);

        game.PlayerFaceDirection("down");
        game.NPCFaceDirection(0, "up");
        game.ChangeCameraTargetToNPC(0);
        dialogueManager.StartDialogueNode(EroNode);
    }

    protected override void HandleAction()
    {
        // if (game.state == "cut-scene" && !game.GetPlayerIsTalking())
        // {
        //     isDone = true;

        //     isExitsDisabled = false;
        //     game.DisableExits(false, 0);
        //     game.ChangeStateCutSceneNPCMoving();
        //     game.TriggerMovingNPCMove(0);

        //     // ero then leaves through door
        // }

        // if (Input.GetButtonDown(Script_KeyCodes.Action1) && game.state == "cut-scene" && !isDone)
        // {
        //     game.HandleContinuingDialogueActions(Script_KeyCodes.Action1);
        // }

        // if (Input.GetButtonDown(Script_KeyCodes.Skip) && game.state == "cut-scene" && !isDone)
        //     game.HandleContinuingDialogueActions(Script_KeyCodes.Skip);
        base.HandleDialogueAction();
    }

    public override void Setup()
    {
        // game.CreateInteractableObjects(null, false);
        game.SetupInteractableObjectsText(pianoTextParent, !isInitialized);
        game.SetupInteractableObjectsText(paintingTextParent, !isInitialized);
        game.SetupMovingNPC(Ero, !isInitialized);

        isInitialized = true;
        
        if (isExitsDisabled)    game.DisableExits(true, 0);
        else game.DisableExits(false, 0);
    }
}
