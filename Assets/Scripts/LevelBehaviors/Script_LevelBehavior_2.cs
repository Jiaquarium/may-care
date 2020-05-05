using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_LevelBehavior_2 : Script_LevelBehavior
{
    /* =======================================================================
        STATE DATA
    ======================================================================= */
    public bool isDone = false;
    public bool isActivated = false;
    public int activeTriggerIndex = 0;
    public string EroFaceDirection;
    public bool[] switchesStates;
    /* ======================================================================= */
    
    public Model_Locations[] triggerLocations;
    public Model_Dialogue[] dialogues;
    public Script_Exits exitsHandler;
    public Script_BgThemePlayer EroBgThemePlayerPrefab;

    private Script_LBSwitchHandler switchHandler;
    
    protected override void HandleOnEntrance() {
        if (!exitsHandler.isFadeIn && !isActivated)
        {
            isActivated = true;
            game.ChangeStateCutSceneNPCMoving();
            game.TriggerMovingNPCMove(0);            
        }
    }

    protected override void HandleTriggerLocations()
    {
        if (isDone || game.GetPlayerIsTalking()) return;
        
        foreach (Vector3 loc in triggerLocations[activeTriggerIndex].locations)
        {
            if (
                game.GetPlayerLocation() == loc
                && game.state == "interact"
            )
            {
                game.PauseBgMusic();
                
                if (game.GetNPCBgThemeActive())
                {
                    game.UnPauseNPCBgTheme();
                }
                else
                {
                    game.PlayNPCBgTheme(EroBgThemePlayerPrefab);
                }
                game.ChangeStateCutScene();
                
                // game.PlayerFaceDirection("down");
                game.StartDialogue(dialogues[activeTriggerIndex]);
                
                /*
                    trigger locations must be 1 < dialogue
                */
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
            game.ChangeStateCutSceneNPCMoving();
            // need this bc once leave room, no longer inProgress
            game.TriggerMovingNPCMove(0);
        }

        base.HandleDialogueAction();
    }

    public override void SetSwitchState(int Id, bool isOn)
    {
        switchHandler.SetSwitchState(switchesStates, Id, isOn);
    }

    // called from Script_Exits()
    public override void InitGameState() {
        // to happen after fadein 
        if (isActivated)
        {
            game.ChangeStateInteract();
            
        }
        isActivated = true;
    }
    
    public override void Setup()
    {
        switchHandler = GetComponent<Script_LBSwitchHandler>();
        switchHandler.Setup(game);
        switchesStates = switchHandler
            .CreateIObjsWithSwitchesState(switchesStates, isActivated);

        game.EnableExits();

        if (!isDone)
        {
            if (isActivated)
            {
                game.CreateMovingNPC(0, "right", activeTriggerIndex, true);
            }
            else
            {
                game.CreateMovingNPC(
                    0,
                    null,
                    activeTriggerIndex
                );
            }
        }
    }
}
