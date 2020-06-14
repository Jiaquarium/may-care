using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_LevelBehavior_2 : Script_LevelBehavior
{
    /* =======================================================================
        STATE DATA
    ======================================================================= */
    public bool isInitialized = false;
    public bool isDone = false;
    public bool isActivated = false;
    public int activeTriggerIndex = 0;
    
    public string EroFaceDirection;
    public bool[] switchesStates;
    /* ======================================================================= */
    
    public Script_DialogueManager dm;
    public Script_MovingNPC Ero;
    public Script_DialogueNode[] TriggerNodes;
    public Script_Exits exitsHandler;
    public Script_BgThemePlayer EroBgThemePlayerPrefab;
    public Transform lightSwitchesParent;
    public Transform painting1Parent;
    public Transform painting2Parent;
    public Transform painting3Parent;

    private Script_LBSwitchHandler switchHandler;
    [SerializeField]
    private Model_MoveSet[] truncatedMoveSet = new Model_MoveSet[0];
    private Model_MoveSet[] moveSets = new Model_MoveSet[0];
    private Queue<string> cachedCurrentMoves = new Queue<string>();
    private Queue<string[]> cachedAllMoves = new Queue<string[]>();
    
    public override void ActivateTrigger(string Id)
    {
        if (
            (
                (Id == "hallway_1" && activeTriggerIndex == 0)
                || (Id == "hallway_2" && activeTriggerIndex == 1)
                || (Id == "hallway_3" && activeTriggerIndex == 2)
            )
            && !isDone
        )
        {
            OnTrigger();         
        }
    }

    void OnTrigger()
    {
        game.PauseBgMusic();
        if (game.GetNPCBgThemeActive())     game.UnPauseNPCBgTheme();
        else                                game.PlayNPCBgTheme(EroBgThemePlayerPrefab);
        
        CacheMovingNPCMoves(0);
        game.ChangeStateCutSceneNPCMoving();
        activeTriggerIndex++;
        if (activeTriggerIndex > 2) isDone = true;
        game.GetMovingNPC(0).ApproachTarget(
            game.GetPlayerLocation(),
            new Vector3(1f, 0, 0),
            "right"
        );
    }
    
    protected override void HandleOnEntrance() {
        if (!exitsHandler.isFadeIn && !isActivated)
        {
            isActivated = true;
            game.ChangeStateCutSceneNPCMoving();
            game.TriggerMovingNPCMove(0);            
        }
    }
    public override void HandleMovingNPCOnApproachedTarget(int i)
    {
        dm.StartDialogueNode(TriggerNodes[activeTriggerIndex - 1]);
        RehydrateMovingNPCMoves(0);
        game.ChangeStateCutScene();
    }

    /*
        when player is done with current convo this turns state to
        npc-cut-scene-moving which triggers Script_MovingNPC Update()
    */
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

    // need this to save NPC moves since ForceMove will erase its moveSets
    void CacheMovingNPCMoves(int Id)
    {
        Script_MovingNPC npc = game.GetMovingNPC(Id);
        
        Model_NPC NPCData = game.Levels.levelsData[game.level].NPCsData[Id];
        Model_MoveSet[] allMoveSets = NPCData.moveSets;
        truncatedMoveSet = new Model_MoveSet[
            Mathf.Max(allMoveSets.Length - activeTriggerIndex - 1, 0)
        ];
        
        for (int j = 0, k = activeTriggerIndex + 1; j < truncatedMoveSet.Length; j++, k++)
        {
            truncatedMoveSet[j] = allMoveSets[k];
        }
    }

    void RehydrateMovingNPCMoves(int Id)
    {
        Script_MovingNPC npc = game.GetMovingNPC(Id);
        
        npc.moveSets = truncatedMoveSet;
        npc.QueueMoves();
    }
    
    public override void Setup()
    {
        switchHandler = GetComponent<Script_LBSwitchHandler>();
        switchHandler.Setup(game);
        switchesStates = switchHandler.SetupIObjsWithSwitchesState(
            lightSwitchesParent,
            switchesStates,
            isInitialize: !isInitialized
        );
        game.SetupInteractableObjectsText(painting1Parent, !isInitialized);
        game.SetupInteractableObjectsText(painting2Parent, !isInitialized);
        game.SetupInteractableObjectsText(painting3Parent, !isInitialized);
        game.SetupMovingNPC(Ero, isInitialize: !isInitialized);
        
        isInitialized = true;
    }
}
