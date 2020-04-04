using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_TeachHowToEat : MonoBehaviour
{
    public Script_Game game;
    public Model_Locations[] triggerLocations;
    public Vector3[] triggerLoc;
    public Model_Dialogue[] dialogues;
    
    
    private bool isDone = false;
    private int activeTriggerIndex = 0;
    private bool hasSwitchedMusic = false;

    void Start()
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
        
        game.ChangeMovingNPCSpeed(0, 0.175f);
        
        // TODO: enable exits only when all demons are eaten       
    }

    // Update is called once per frame
    void Update()
    {
        HandleAction();
        
        // play HARD music when Ero finishes first dialogue and moveset
        // if (activeTriggerIndex == 1 && game.state == "interact" && !hasSwitchedMusic)
        // {
            
        //     hasSwitchedMusic = true;
        //     game.SwitchBgMusic(2);
        // }

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

    void HandleAction()
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
}
