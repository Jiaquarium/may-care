using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_LevelBehavior_10 : Script_LevelBehavior
{
    public bool isDone;
    public bool isActivated;
    public int activeTriggerIndex;
    public float zoomInWaitTime;
    public float postZoomInWaitTime;
    public float IdsDanceWaitTime;
    public float DDRWaitTime;
    public float postIdsDanceWaitTime;
    public float zoomInOnPlayerSize;
    

    public Script_DialogueManager dm;
    public Script_DDRManager DDRManager;
    public Script_Exits exitsHandler;
    public Script_BgThemePlayer IdsBgThemePlayerPrefab;
    public Script_BgThemePlayer IdsCandyDanceShortThemePlayerPrefab;
    public Script_BgThemePlayer PlayerCandyDanceThemePlayerPrefab;
    public Script_DialogueNode introNode;
    public Script_DialogueNode introNode3;
    public Script_DialogueNode danceIntroNode;
    public Script_DialogueNode playerDanceIntroNode;
    public Script_DialogueNode badDanceOutroNode;
    public Script_DialogueNode goodDanceOutroNode;
    public Script_DialogueNode specterStoryNode;
    
    public Model_SongMoves playerSongMoves;
    public Model_SongMoves IdsSongMoves;
    public Model_Locations[] triggerLocations;
    public Vector3 newOffset;
    public int mistakesAllowed;

    private bool shouldMove = true;
    private bool shouldRunToDanceFloor = false;
    private bool shouldRunToLastRoom = false;
    private bool shouldExitRoom = false;
    private bool zoomInOnPlayerNodeActivated = false;
    private bool IdsDanceNodeActivated = false;
    private bool DDR = false;
    private bool isIdsDancing = false;
    private float timer;
    private int leftMoveCount;
    private int downMoveCount;
    private int upMoveCount;
    private int rightMoveCount;
    private bool leftMoveDone;
    private bool downMoveDone;
    private bool upMoveDone;
    private bool rightMoveDone;

    protected override void Update()
    {
        HandleTriggerLocations();
        HandleAction();
        HandleOnEntrance();

        HandleIdsDanceScene();
    }

    protected override void HandleOnEntrance() {
        if (!exitsHandler.isFadeIn && !isActivated)
        {
            isActivated = true;
            game.TriggerMovingNPCMove(0);
        }
    }

    protected override void HandleTriggerLocations()
    {
        if (
            Script_Utils.CheckLastNodeActionCutScene(game, dm, "zoom")
            && !zoomInOnPlayerNodeActivated
        )
        {
            zoomInOnPlayerNodeActivated = true;
            StartCoroutine(WaitToZoomInOnPlayer());
        }

        if (
            Script_Utils.CheckLastNodeActionCutScene(game, dm, "ids-dance")
            && !IdsDanceNodeActivated
        )
        {
            IdsDanceNodeActivated = true;
            // do ids dance sequence
            StartCoroutine(WaitToIdsDance());
        }

        if (
            Script_Utils.CheckLastNodeActionCutScene(game, dm, "DDR")
            && !DDR
        )
        {
            DDR = true;
            StartCoroutine(WaitToDDR());
        }

        if (
            game.state == "cut-scene"
            && !game.GetPlayerIsTalking()
            && shouldRunToDanceFloor
        )
        {
            shouldRunToDanceFloor = false;
            game.ChangeStateCutSceneNPCMoving();
            game.TriggerMovingNPCMove(0);
            shouldMove = true;
        }

        // will trigger NPC move after finishing talk
        if (
            Script_Utils.CheckLastNodeActionCutScene(game, dm, "dance-outro")
            && !shouldRunToLastRoom
        )
        {
            shouldRunToLastRoom = true;
        }
        if (
            game.state == "cut-scene"
            && !game.GetPlayerIsTalking()
            && shouldRunToLastRoom
        )
        {
            shouldRunToLastRoom = false;
            game.ChangeStateCutSceneNPCMoving();
            game.TriggerMovingNPCMove(0);
            shouldMove = true;
        }

        if (
            Script_Utils.CheckLastNodeActionCutScene(game, dm, "exit-room")
            && !shouldExitRoom
        )
        {
            shouldExitRoom = true;
        }
        if (
            game.state == "cut-scene"
            && !game.GetPlayerIsTalking()
            && shouldExitRoom
        )
        {
            shouldExitRoom = false;
            print("QUITTING APP");
            Application.Quit();
        }

        HandleNPCActuallyMove();

        if (isDone || game.GetPlayerIsTalking()) return;

        foreach (Vector3 loc in triggerLocations[activeTriggerIndex].locations)
        {
            if (
                game.GetPlayerLocation() == loc
                && game.state == "interact"
                && activeTriggerIndex == 0
            )
            {
                game.PauseBgMusic();
                game.PlayNPCBgTheme(IdsBgThemePlayerPrefab);
                game.ChangeStateCutScene();
                
                game.PlayerFaceDirection("up");
                dm.StartDialogueNode(introNode);                
                
                if (activeTriggerIndex == triggerLocations.Length - 1) isDone = true;
                activeTriggerIndex++;
            }

            if (
                game.GetPlayerLocation() == loc
                && game.state == "interact"
                && activeTriggerIndex == 1
            )
            {
                game.PauseNPCBgTheme();
                game.ChangeStateCutScene();
                
                game.PlayerFaceDirection("right");
                game.GetMovingNPC(0).FaceDirection("left");
                dm.StartDialogueNode(danceIntroNode);
                
                if (activeTriggerIndex == triggerLocations.Length - 1) isDone = true;
                activeTriggerIndex++;
            } 

            if (
                game.GetPlayerLocation() == loc
                && game.state == "interact"
                && activeTriggerIndex == 2
            )
            {
                game.ChangeStateCutScene();
                
                game.PlayerFaceDirection("left");
                dm.StartDialogueNode(specterStoryNode);
                
                if (activeTriggerIndex == triggerLocations.Length - 1) isDone = true;
                activeTriggerIndex++;
            }
        }
    }

    IEnumerator WaitToZoomInOnPlayer()
    {
        yield return new WaitForSeconds(zoomInWaitTime);

        game.CameraInstantMoveSpeed();
        game.SetCameraOffset(newOffset);
        game.SetOrthographicSize(zoomInOnPlayerSize);

        yield return new WaitForSeconds(postZoomInWaitTime);
        
        game.SetOrthographicSizeDefault();
        game.SetCameraOffsetDefault();
        game.CameraDefaultMoveSpeed();
        dm.StartDialogueNode(introNode3, false);

        shouldRunToDanceFloor = true;
    }

    IEnumerator WaitToIdsDance()
    {
        yield return new WaitForSeconds(IdsDanceWaitTime);

        // Ids dance sequence
        dm.EndDialogue();
        game.ChangeStateCutScene();
        game.PlayNPCBgTheme(IdsCandyDanceShortThemePlayerPrefab);
        isIdsDancing = true;
    }

    IEnumerator WaitToDDR()
    {
        yield return new WaitForSeconds(DDRWaitTime);

        dm.EndDialogue();
        DDRManager.Activate();
        DDRManager.StartMusic(
            playerSongMoves,
            PlayerCandyDanceThemePlayerPrefab,
            mistakesAllowed
        );
        
        game.ChangeStateDDR(); // this triggers the HandleDDRFinish
    }

    IEnumerator WaitToTalkAfterIdsDance()
    {
        yield return new WaitForSeconds(postIdsDanceWaitTime);
        
        game.GetMovingNPC(0).FaceDirection("left");
        dm.StartDialogueNode(playerDanceIntroNode);
    }

    void HandleIdsDanceScene()
    {
        if (isIdsDancing && game.GetNPCBgThemeActive())
        {
            timer += Time.deltaTime;
            
            HandleLeftMove();
            HandleDownMove();
            HandleUpMove();
            HandleRightMove();

            if (!game.GetNPCThemeMusicIsPlaying())
            {
                print("done daincing... start next dialogue node here");
                isIdsDancing = false;
                StartCoroutine(WaitToTalkAfterIdsDance());
            }
        }

        HandleDDRFinish();
    }

    void HandleDDRFinish()
    {
        if (game.state == "ddr" && DDR)
        {
            // handle fail case
            if (DDRManager.didFail)
            {
                DDR = false;
                game.ChangeStateCutScene();
                dm.StartDialogueNode(badDanceOutroNode);
            }
            else if (!game.GetNPCThemeMusicIsPlaying())
            {
                DDR = false;
                game.ChangeStateCutScene();
                DDRManager.Deactivate();
                dm.StartDialogueNode(goodDanceOutroNode);
            }   
        }
    }

    void HandleLeftMove()
    {
        if (leftMoveCount > IdsSongMoves.leftMoveTimes.Length - 1)    return;

        if (timer >= IdsSongMoves.leftMoveTimes[leftMoveCount] && !leftMoveDone)
        {
            game.GetMovingNPC(0).FaceDirection("left");
            leftMoveCount++;
            leftMoveDone = true;
        }
        else
        {
            leftMoveDone = false;
        }
    }

    void HandleDownMove()
    {
        if (downMoveCount > IdsSongMoves.downMoveTimes.Length - 1)    return;

        if (timer >= IdsSongMoves.downMoveTimes[downMoveCount] && !downMoveDone)
        {
            game.GetMovingNPC(0).FaceDirection("down");
            downMoveCount++;
            downMoveDone = true;
        }
        else
        {
            downMoveDone = false;
        }
    }

    void HandleUpMove()
    {
        if (upMoveCount > IdsSongMoves.upMoveTimes.Length - 1)    return;

        if (timer >= IdsSongMoves.upMoveTimes[upMoveCount] && !upMoveDone)
        {
            game.GetMovingNPC(0).FaceDirection("up");
            upMoveCount++;
            upMoveDone = true;
        }
        else
        {
            upMoveDone = false;
        }
    }

    void HandleRightMove()
    {
        if (rightMoveCount > IdsSongMoves.rightMoveTimes.Length - 1)    return;

        if (timer >= IdsSongMoves.rightMoveTimes[rightMoveCount] && !rightMoveDone)
        {
            game.GetMovingNPC(0).FaceDirection("right");
            rightMoveCount++;
            rightMoveDone = true;
        }
        else
        {
            rightMoveDone = false;
        }
    }

    protected override void HandleAction()
    {   
        if (
            game.GetPlayerIsTalking()
            && dm.dialogueSections.Count == 0
            && dm.lines.Count == 0
        )
        {
            if (
                dm.currentNode.data.action == "zoom"
                || dm.currentNode.data.action == "ids-dance"
            )
            {
                return;
            }
        }
        
        base.HandleDialogueAction();
    }

    void HandleNPCActuallyMove()
    {
        if (!shouldMove)    return;

        Script_MovingNPC NPC = game.GetMovingNPC(0);

        if (NPC.currentMoves.Count != 0 || NPC.progress < 1f)
        {
            game.MovingNPCActuallyMove(0);
        }
    }

    public override void HandleMovingNPCCurrentMovesDone()
    {
        game.NPCFaceDirection(0, "down");
        shouldMove = false;
    }

    public override void HandleDDRArrowClick(int tier) {

    }

    public override void HandleMovingNPCAllMovesDone()
    {
        game.GetMovingNPC(0).FaceDirection("right");
    }

    public override void Setup()
    {
        if (!isDone)
        {
            game.CreateMovingNPC(
                0,
                null,
                activeTriggerIndex
            );
            game.SetMovingNPCExit(0, false);

            // TODO REMOVE and figure out state
            game.DisableExits();
        }   
    }
}
