using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_LevelBehavior_3 : Script_LevelBehavior
{
        /* =======================================================================
        STATE DATA
    ======================================================================= */
    public bool isDone = false;
    public bool isActivated = false;
    public bool isExitsDisabled = true;
    public int activeTriggerIndex = 0;
    public bool[] demonSpawns;
    /* ======================================================================= */
    
    
    public Model_Locations[] triggerLocations;
    public Model_Dialogue[] dialogues;
    public float NPCMoveSpeed;
    
    
    protected override void HandleTriggerLocations()
    {
        if (activeTriggerIndex == 1 && game.state == "interact")
        {
            game.NPCFaceDirection(0, "down");
        }
        if (isDone)                                                 return;
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
                activeTriggerIndex++;
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
                isExitsDisabled = false;
                game.EnableExits();
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

    public override void EatDemon(int Id) {
        demonSpawns[Id] = false;
    }

    public override void Setup()
    {
        if (isExitsDisabled)    game.DisableExits();
        else                    game.EnableExits();
        
        if (!isDone)
        {
            if (isActivated)
            {
                // activeTriggerIndex - 1 because here we have 2 trigger locations but only 1 moveSet
                string dir = activeTriggerIndex == 1 ? "down" : null;
                game.CreateMovingNPC(0, dir, activeTriggerIndex - 1, true);
                game.CreateDemons(demonSpawns);
            }
            else
            {
                game.CreateMovingNPC(0, null, activeTriggerIndex - 1, false);
                isActivated = true;
                game.CreateDemons(null);
                demonSpawns = new bool[game.GetDemonsCount()];
                
                for (int i = 0; i < demonSpawns.Length; i++)
                {
                    demonSpawns[i] = true;
                }
            }
            
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
}
