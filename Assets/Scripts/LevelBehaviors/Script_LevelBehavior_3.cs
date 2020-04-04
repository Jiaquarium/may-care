using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_LevelBehavior_3 : Script_LevelBehavior
{
    public Model_Locations[] triggerLocations;
    public Model_Dialogue[] dialogues;
    public float NPCMoveSpeed;
    
    
    private bool isDone = false;
    private int activeTriggerIndex = 0;
    private bool hasSwitchedMusic = false;
    
    
    protected override void HandleTriggerLocations()
    {
        // need to EAT all demons before can activiate 2nd trigger location
        if (activeTriggerIndex == 1 && game.GetDemonsCount() > 0)   return;

        foreach(Vector3 loc in triggerLocations[activeTriggerIndex].locations)
        {
            if (
                game.GetPlayerLocation() == loc
                && game.state == "interact"
                && !isDone
            )
            {
                game.PauseBgMusic();
                game.UnPauseEroTheme();
                
                // don't pause, stop the music bc we're switching to the HARD song
                // game.StopBgMusic();
                
                game.ChangeStateCutScene();
                
                if (activeTriggerIndex == 0)
                {
                    game.PlayerFaceDirection("down");
                }
                if (activeTriggerIndex == 1)
                {
                    game.PlayerFaceDirection("right");
                    game.NPCFaceDirection(0, "left");
                }
                game.ChangeCameraTargetToNPC(0);
                game.StartDialogue(dialogues[activeTriggerIndex]);

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
            if (!isDone)
            {
                game.ChangeStateCutSceneNPCMoving();
                game.TriggerMovingNPCMove(0);
            } else
            {
                game.exitsHandler.EnableExits();
                game.ChangeStateInteract();
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
        game.exitsHandler.DisableExits();
        
        /*
            Ero will wait at the door as you leave this map
            this triggers game.AllMovesDoneAction() early so we must
            set shouldPersistBgThemes = true
            playingEroTheme next time wili instantiate a new gameObject replacing
            the undeleted gameObject
        */
        game.SetMovingNPCExit(0, false);
        
        game.ChangeMovingNPCSpeed(0, NPCMoveSpeed);
    }
}
