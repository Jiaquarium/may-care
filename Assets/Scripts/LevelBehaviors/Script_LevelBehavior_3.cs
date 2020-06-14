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
    public int activeTriggerIndex = 0;
    public bool[] demonSpawns;
    /* ======================================================================= */
    
    
    public Transform demonsParent;
    public Script_MovingNPC Ero;
    public Script_DialogueNode[] triggerNodes;
    public Script_DialogueManager dm;

    public Script_BgThemePlayer EroBgThemePlayerPrefab;
    
    public override void Cleanup() {
        if (isDone)     game.DestroyNPCs();
    }
    
    public override void ActivateTrigger(string Id)
    {
        if (
            (
                (Id == "room_1" && activeTriggerIndex == 0 )
                || (Id == "room_2" && activeTriggerIndex == 1)
            ) && !isDone
        )
        {
            game.ChangeStateCutScene();
            
            if (activeTriggerIndex == 0)
            {
                game.PauseBgMusic();
                game.PlayNPCBgTheme(EroBgThemePlayerPrefab);
                game.PlayerFaceDirection("down");
                game.NPCFaceDirection(0, "up");
            }
            else if (activeTriggerIndex == 1)
            {
                game.PlayerFaceDirection("right");
                game.NPCFaceDirection(0, "left");
            }
            
            game.ChangeCameraTargetToNPC(0);
            dm.StartDialogueNode(triggerNodes[activeTriggerIndex]);
            
            activeTriggerIndex++;
            if (activeTriggerIndex > 1)     isDone = true;
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
                game.ChangeStateInteract();
            }
        }

        base.HandleDialogueAction();
    }

    public override void HandleMovingNPCAllMovesDone()
    {
        game.NPCFaceDirection(0, "down");
    }

    public override void EatDemon(int Id) {
        demonSpawns[Id] = false;
    }

    public override void Setup()
    {
        // on initialization
        if (!isActivated)
        {
            demonSpawns = new bool[demonsParent.childCount];
            for (int i = 0; i < demonSpawns.Length; i++)
                demonSpawns[i] = true;
        }
        
        game.SetupMovingNPC(Ero, !isActivated);
        game.SetupDemons(demonsParent, demonSpawns);
        
        isActivated = true;
    }
}
