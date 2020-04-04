using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_LevelBehavior_2 : Script_LevelBehavior
{
    public Model_Locations[] triggerLocations;
    public Model_Dialogue[] dialogues;
    private int activeTriggerIndex = 0;
    private bool isDone = false;
    
    protected override void HandleTriggerLocations()
    {
        if (isDone) return;

        foreach (Vector3 loc in triggerLocations[activeTriggerIndex].locations)
        {
            if (
                game.GetPlayerLocation() == loc
                && game.state == "interact"
            )
            {
                game.PauseBgMusic();
                
                if (game.GetEroThemeActive())
                {
                    game.UnPauseEroTheme();
                }
                else
                {
                    game.PlayEroTheme();
                }
                game.ChangeStateCutScene();
                
                // game.PlayerFaceDirection("down");
                game.StartDialogue(dialogues[activeTriggerIndex]);
                
                /*
                    trigger locations must be 1 < dialogue
                */
                if (activeTriggerIndex == triggerLocations.Length - 1) isDone = true;
                else activeTriggerIndex++;
            }
        }
    }

    protected override void HandleAction()
    {
        if (
            game.state == "cut-scene"
            && !game.GetPlayerIsTalking()
        )
        {
            // trigger ActuallyMove() in MovingNPC to exit
            if (isDone)
            {
                game.ChangeStateCutSceneNPCMoving();
            }
            else
            {
                game.ChangeStateCutSceneNPCMoving();
            }
        }

        if (Input.GetButtonDown("Action1") && game.state == "cut-scene")
        {
            game.HandleContinuingDialogueActions("Action1");
        }

        if (Input.GetButtonDown("Submit") && game.state == "cut-scene")
        {
            game.HandleContinuingDialogueActions("Submit");
        }
    }

    public override void Setup()
    {
        game.TriggerMovingNPCMove(0);
    }
}
