using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_LevelBehavior_16_BioArt : Script_LevelBehavior
{
    public bool isComplete;
    public Script_DDRManager DDRManager;
    public AudioSource audioSource;
    public AudioClip SpotlightSFX;
    public GameObject IdsSpotlight;
    public GameObject PlayerSpotlight;
    public GameObject DanceSpotlight;
    public Script_DialogueNode danceIntroNode;
    public Script_DialogueNode badDanceOutroNode;
    public Script_DialogueNode goodDanceOutroNode;
    public Script_DialogueNode playerDanceIntroNode;
    public Script_BgThemePlayer IdsCandyDanceShortThemePlayerPrefab;
    public Script_BgThemePlayer PlayerCandyDanceThemePlayerPrefab;
    public Script_SongMoves IdsSongMoves;
    public Script_SongMoves PlayerSongMoves;

    public Script_DialogueManager dm;
    public Model_Locations triggerLocs;
    public float IdsDanceWaitTime;
    public float postIdsDanceWaitTime;
    public float DDRWaitTime;
    public float spotlightVolumeScale;
    public int mistakesAllowed;


    private float timer;
    private bool isTriggerActivated;
    private bool IdsDanceNodeActivated;
    private bool isIdsDancing;
    private bool DDR;
    private bool isDone;
    
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

        HandleIdsDanceScene();
    }
    
    protected override void HandleTriggerLocations()
    {
        foreach (Vector3 loc in triggerLocs.locations)
        {
            if (
                game.GetPlayerLocation() == loc
                && game.state == "interact"
                && !isTriggerActivated
            )
            {
                isTriggerActivated = true;
                game.StopBgMusic();
                game.ChangeStateCutScene();
                game.PlayerFaceDirection("up");
                audioSource.PlayOneShot(SpotlightSFX, spotlightVolumeScale);
                DanceSpotlight.SetActive(true);
                PlayerSpotlight.SetActive(false);
                IdsSpotlight.SetActive(false);

                dm.StartDialogueNode(danceIntroNode);
            }
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
            Script_Utils.CheckLastNodeActionCutScene(game, dm, "dance-outro")
            && !isDone
        )
        {
            isDone = true;
        }
        if (
            game.state == "cut-scene"
            && !game.GetPlayerIsTalking()
            && isDone
        )
        {
            game.ChangeStateInteract();
        }
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
            PlayerSongMoves.moves,
            PlayerCandyDanceThemePlayerPrefab,
            mistakesAllowed
        );
        
        game.ChangeStateDDR(); // this triggers the HandleDDRFinish
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
                game.PlayerFaceDirection("up");
                dm.StartDialogueNode(badDanceOutroNode);
            }
            else if (!game.GetNPCThemeMusicIsPlaying())
            {
                DDR = false;
                game.ChangeStateCutScene();
                DDRManager.Deactivate();
                game.PlayerFaceDirection("up");
                isComplete = true;
                dm.StartDialogueNode(goodDanceOutroNode);
            }   
        }
    }

    IEnumerator WaitToTalkAfterIdsDance()
    {
        yield return new WaitForSeconds(postIdsDanceWaitTime);
        
        game.GetMovingNPC(0).FaceDirection("down");
        dm.StartDialogueNode(playerDanceIntroNode);
    }

    void HandleLeftMove()
    {
        if (leftMoveCount > IdsSongMoves.moves.leftMoveTimes.Length - 1)    return;

        if (timer >= IdsSongMoves.moves.leftMoveTimes[leftMoveCount] && !leftMoveDone)
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
        if (downMoveCount > IdsSongMoves.moves.downMoveTimes.Length - 1)    return;

        if (timer >= IdsSongMoves.moves.downMoveTimes[downMoveCount] && !downMoveDone)
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
        if (upMoveCount > IdsSongMoves.moves.upMoveTimes.Length - 1)    return;

        if (timer >= IdsSongMoves.moves.upMoveTimes[upMoveCount] && !upMoveDone)
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
        if (rightMoveCount > IdsSongMoves.moves.rightMoveTimes.Length - 1)    return;

        if (timer >= IdsSongMoves.moves.rightMoveTimes[rightMoveCount] && !rightMoveDone)
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
                dm.currentNode.data.action == "ids-dance"
                || dm.currentNode.data.action == "DDR"
            )
            {
                return;
            }
        }
        
        base.HandleDialogueAction();
    }

    public override void Setup()
    {
        game.CreateNPCs();
        
        timer = 0;
        isTriggerActivated = false;
        IdsDanceNodeActivated = false;
        isIdsDancing = false;
        leftMoveCount = 0;
        downMoveCount = 0;
        upMoveCount = 0;
        rightMoveCount = 0;
        leftMoveDone = false;
        downMoveDone = false;
        upMoveDone = false;
        rightMoveDone = false;

        DDR = false;
        isDone = false;

        IdsSpotlight.SetActive(true);
        PlayerSpotlight.SetActive(true);
        DanceSpotlight.SetActive(false);
    }    
}
