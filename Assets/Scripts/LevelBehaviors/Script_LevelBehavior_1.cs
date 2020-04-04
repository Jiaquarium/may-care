using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_LevelBehavior_1 : Script_LevelBehavior
{
    public Vector3[] triggerLocations;
    public Model_Dialogue dialogue;
    
    
    private bool isDone = false;
    
    protected override void OnDisable() {
        game.exitsHandler.EnableExits();
    }

    protected override void HandleTriggerLocations() {
        for (int i = 0; i < triggerLocations.Length; i++)
        {
            if (
                game.GetPlayerLocation() == triggerLocations[i]
                && game.state == "interact"
                && !isDone
            )
            {
                game.PauseBgMusic();
                game.PlayEroTheme();
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
            isDone = true;

            game.exitsHandler.EnableExits();
            game.ChangeStateCutSceneNPCMoving();
            game.TriggerMovingNPCMove(0);

            // ero then leaves through door
        }

        if (Input.GetButtonDown("Action1") && game.state == "cut-scene" && !isDone)
        {
            game.HandleContinuingDialogueActions("Action1");
        }

        if (Input.GetButtonDown("Submit") && game.state == "cut-scene" && !isDone)
            game.HandleContinuingDialogueActions("Submit");
    }

    public override void Setup()
    {
        base.Setup();
        print("setting up levelBehavior1");

        game.exitsHandler.DisableExits();
    }
}
