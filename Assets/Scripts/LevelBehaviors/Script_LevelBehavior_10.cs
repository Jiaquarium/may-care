using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class Script_LevelBehavior_10 : Script_LevelBehavior
{
    public bool isDone;
    public bool isActivated;
    public int activeTriggerIndex;
    public bool isDeskSwitchedIn;
    public bool isUnlocked;
    public bool isSpecterStoryStarted;
    public bool isInitialized;


    public float zoomInWaitTime;
    public float postZoomInWaitTime;
    public float IdsDanceWaitTime;
    public float dropDiscoBallTime;
    public float DDRWaitTime;
    public float postIdsDanceWaitTime;
    public float IdsWaitTimeBeforeUnlocking;
    public float IdsExitWaitTimeAfterUnlock;
    public float zoomInOnPlayerSize;
    public float moveToPlayerSmoothTime;
    public float zoomSmoothTime;
    public float waitForQuestionMarkTime;
    public Vector3 IdsExitLocation;
    

    public Script_DialogueManager dm;
    public Script_DDRManager DDRManager;
    public Script_Exits exitsHandler;
    public Script_LevelBehavior_9 lb9;
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
    public Script_DialogueNode finalComplimentNode;
    public Script_DialogueNode deskIONode;
    public Script_DialogueNode chaiseLoungeIONode;
    public GameObject lights;
    public GameObject crystalChandelier;
    public Script_LightFadeIn playerSpotLight;
    public Script_LightFadeIn IdsSpotLight;
    public Vector3 lightsUpOffset;
    public Vector3 lightsDownOffset;
    public Vector3 crystalChandelierDownOffset;
    public Vector3 crystalChandelierUpOffset;
    public Script_DoorLock doorLock;
    public GameObject chaiseLoungeObject;
    public GameObject deskObject;
    public AudioSource audioSource;
    public AudioMixer audioMixer;
    public AudioMixerGroup audioMixerGroup;
    public AudioClip exitSFX;
    public float exitVol;
    public AudioClip onExitFadeOutDoneSFX;
    public float onExitFadeOutDoneVol;
    public float fadeOutTransitionTime;
    public float transitionMusicPauseTime;
    public float melanholicTitleFadeInTime;
    
    public Model_SongMoves playerSongMoves;
    public Model_SongMoves IdsSongMoves;
    public int mistakesAllowed;
    public Transform[] IOTexts;
    public Script_MovingNPC_Ids Ids;

    private bool shouldMove = true;
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
    private bool isFinalComplimentDone;

    protected override void Update()
    {
        HandleNPCActuallyMove();
        HandleAction();
        HandleOnEntrance();

        HandleIdsDanceScene();
        if (game.state == "ddr" && DDR)    HandlePlayerDDRFinish();

    }

    protected override void HandleOnEntrance() {
        if (!exitsHandler.isFadeIn && !isActivated)
        {
            isActivated = true;
            game.TriggerMovingNPCMove(0);
        }
    }

    public override void HandleDialogueNodeAction(string a)
    {
        print("HandleDialogueNodeAction() with: " + a);
        if (a == "zoom")                StartCoroutine(WaitToZoomInOnPlayer());
        else if (a == "NPCmove")
        {
            game.GetMovingNPC(0).SetMoveSpeedWalk();
            game.ChangeStateCutSceneNPCMoving();
            game.TriggerMovingNPCMove(0);
            shouldMove = true;
        }    
        else if (a == "ids-dance")      WaitToIdsDance();
        else if (a == "DDR")
        {
            DDR = true;
            WaitToDDR();
        }
        else if (a == "dance-outro")
        {
            game.ChangeStateCutSceneNPCMoving();
            game.TriggerMovingNPCMove(0);
            shouldMove = true;
        }
        else if (a == "no-to-go-deeper")
        {
            game.ChangeStateInteract();
        }
        else if (a == "approach-exit")
        {
            game.ChangeStateCutSceneNPCMoving();
            game.GetMovingNPC(0).ApproachTarget(
                IdsExitLocation,
                Vector3.zero,
                "left"
            );
        }
        else if (a == "exit")            StartCoroutine(WaitToIdsExit());
    }

    public override void ActivateTrigger(string Id)
    {
        if (Id == "room_N" && activeTriggerIndex == 0)
        {
            if (lb9.speaker != null)
                {
                    lb9.speaker.audioSource.Pause();
                    Destroy(lb9.speaker.gameObject);
                }
                else
                {
                    game.PauseBgMusic();
                }
                game.PlayNPCBgTheme(IdsBgThemePlayerPrefab);
                game.ChangeStateCutScene();
                game.PlayerFaceDirection("up");
                game.CameraMoveToTargetSmooth(
                    moveToPlayerSmoothTime,
                    game.GetMovingNPC(0).transform.position,
                    () => {
                        dm.StartDialogueNode(introNode);                
                    }
                );
                
                // if (activeTriggerIndex == triggerLocations.Length - 1) isDone = true;
                activeTriggerIndex++;
        }
        else if (Id == "room_E" && activeTriggerIndex == 1)
        {
            game.PauseNPCBgTheme();

                if (lb9.speaker != null)
                {
                    lb9.speaker.audioSource.Pause();
                    Destroy(lb9.speaker.gameObject);
                }

                game.ChangeStateCutScene();
                
                game.PlayerFaceDirection("right");
                game.GetMovingNPC(0).FaceDirection("left");
                dm.StartDialogueNode(danceIntroNode);
                
                activeTriggerIndex++;
        }
        else if (Id == "room_W" && activeTriggerIndex == 2)
        {
            if (lb9.speaker != null)
            {
                lb9.speaker.audioSource.Pause();
                Destroy(lb9.speaker.gameObject);
            }
            
            game.ChangeStateCutSceneNPCMoving();
            game.PlayerFaceDirection("left");
            game.GetMovingNPC(0).ApproachTarget(
                game.GetPlayerLocation(),
                new Vector3(-2f, 0, 0),
                "right"
            );
            
            activeTriggerIndex++;
        }
    }    

    IEnumerator WaitToZoomInOnPlayer()
    {
        yield return new WaitForSeconds(zoomInWaitTime);

        game.CameraSetIsTrackingTarget(false);
        
        game.CameraMoveToTargetSmooth(
            moveToPlayerSmoothTime,
            game.GetPlayerLocation() + Camera.main.GetComponent<Script_Camera>().offset,
            () => {
                game.CameraZoomSmooth(
                    zoomInOnPlayerSize,
                    zoomSmoothTime,
                    game.GetPlayerLocation() + Camera.main.GetComponent<Script_Camera>().offset,
                    null
                );
            }
        );

        // wait for smooth move and zoom to finish
        yield return new WaitForSeconds(moveToPlayerSmoothTime + zoomSmoothTime + waitForQuestionMarkTime);
        game.PlayerEffectQuestion(true);
        yield return new WaitForSeconds(postZoomInWaitTime);
        game.PlayerEffectQuestion(false);
        
        game.CameraSetIsTrackingTarget(true);
        game.CameraTargetToPlayer();
        game.SetOrthographicSizeDefault();
        game.SetCameraOffsetDefault();
        game.CameraDefaultMoveSpeed();
        dm.StartDialogueNode(introNode3, false);
        game.EnableSBook(true);
    }

    void WaitToIdsDance()
    {
        game.ChangeStateCutScene();
        SwitchLightsOutAnimation();
    }

    void WaitToDDR()
    {
        DDRManager.Activate();
        DDRManager.StartMusic(
            playerSongMoves,
            PlayerCandyDanceThemePlayerPrefab,
            mistakesAllowed
        );
        crystalChandelier.GetComponent<Script_CrystalChandelier>()
            .StartSpinning();
        
        game.ChangeStateDDR(); // this triggers the HandleDDRFinish
    }

    IEnumerator WaitToTalkAfterIdsDance()
    {
        yield return new WaitForSeconds(postIdsDanceWaitTime);
        
        game.GetMovingNPC(0).FaceDirection("left");
        dm.StartDialogueNode(playerDanceIntroNode);
    }

    IEnumerator WaitToIdsExit()
    {
        yield return new WaitForSeconds(IdsWaitTimeBeforeUnlocking);
        doorLock.Unlock();
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
                crystalChandelier.GetComponent<Script_CrystalChandelier>()
                    .StopSpinning();
                isIdsDancing = false;
                StartCoroutine(WaitToTalkAfterIdsDance());
            }
        }
    }

    void HandlePlayerDDRFinish()
    {
        // handle fail case
        if (DDRManager.didFail)
        {
            DDRFinish(badDanceOutroNode);
        }
        else if (!game.GetNPCThemeMusicIsPlaying())
        {
            DDRManager.Deactivate();
            DDRFinish(goodDanceOutroNode);
        }   
    }

    void DDRFinish(Script_DialogueNode node)
    {
        DDR = false;
        game.ManagePlayerViews(Const_States_PlayerViews.Health);
        game.ChangeStateCutScene();
        game.PlayerFaceDirection("right");
        
        crystalChandelier.GetComponent<Script_CrystalChandelier>()
            .StopSpinning();
        SwitchLightsInAnimation();

        dm.StartDialogueNode(node);
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
        base.HandleDialogueAction();
    }

    void SwitchLightsOutAnimation()
    {
        // todo: setinactive after
        crystalChandelier.SetActive(true);
        StartCoroutine(
            lights.GetComponent<Script_MoveDirection>().MoveSmooth(
                dropDiscoBallTime,
                lightsUpOffset,
                () => {
                    game.PlayNPCBgTheme(IdsCandyDanceShortThemePlayerPrefab);
                    isIdsDancing = true;
                    crystalChandelier.GetComponent<Script_CrystalChandelier>()
                        .StartSpinning();

                    DeskSwitchIn();
                    isDeskSwitchedIn = true;
                }
            )
        );
        StartCoroutine(
            crystalChandelier.GetComponent<Script_MoveDirection>().MoveSmooth(
                dropDiscoBallTime,
                crystalChandelierDownOffset,
                null
            )
        );
        StartCoroutine(playerSpotLight.FadeInLightOnTarget(
            dropDiscoBallTime,
            game.GetPlayerTransform(),
            null
        ));
        StartCoroutine(IdsSpotLight.FadeInLightOnTarget(
            dropDiscoBallTime,
            game.GetNPC(0).GetComponent<Transform>(),
            null
        ));
    }

    void SwitchLightsInAnimation()
    {
        StartCoroutine(
            lights.GetComponent<Script_MoveDirection>().MoveSmooth(
                dropDiscoBallTime,
                lightsDownOffset,
                null
            )
        );
        StartCoroutine(
            crystalChandelier.GetComponent<Script_MoveDirection>().MoveSmooth(
                dropDiscoBallTime,
                crystalChandelierUpOffset,
                () => {
                    crystalChandelier.SetActive(false);
                }
            )
        );
        StartCoroutine(playerSpotLight.FadeOutLight(
            dropDiscoBallTime,
            null
        ));
        StartCoroutine(IdsSpotLight.FadeOutLight(
            dropDiscoBallTime,
            null
        ));
    }

    void HandleNPCActuallyMove()
    {
        if (!shouldMove)    return;

        Script_MovingNPC NPC = game.GetMovingNPC(0);

        if (NPC.currentMoves.Count != 0 && NPC.progress < 1f)
        {
            game.MovingNPCActuallyMove(0);
        }
        else if (NPC.currentMoves.Count == 0 && NPC.progress < 1f)
        {
            game.MovingNPCActuallyMove(0);
        }
    }

    public override void HandleMovingNPCCurrentMovesDone()
    {
        shouldMove = false;
    }

    public override void HandleDDRArrowClick(int tier) {

    }

    public override void HandleMovingNPCAllMovesDone()
    {
        if (activeTriggerIndex == 3 && !isSpecterStoryStarted)
        {
            isSpecterStoryStarted = true;
            game.ChangeStateCutScene();
            dm.StartDialogueNode(specterStoryNode);
            game.GetMovingNPC(0).SetMute(false);
        }
        else if (activeTriggerIndex == 3 && !isFinalComplimentDone)
        {
            isFinalComplimentDone = true;
            game.ChangeStateCutScene();
            dm.StartDialogueNode(finalComplimentNode);
        }
    }

    // animation done callback
    public override void OnDoorLockUnlock(int id)
    {
        StartCoroutine(DoorLockUnlockAction(id));
    }

    void ChaseLoungeSwitchIn()
    {
        SetupIOsDialogue(chaiseLoungeIONode);
        chaiseLoungeObject.SetActive(true);
        deskObject.SetActive(false);
    }

    void DeskSwitchIn()
    {
        SetupIOsDialogue(deskIONode);
        chaiseLoungeObject.SetActive(false);
        deskObject.SetActive(true);
    }

    IEnumerator DoorLockUnlockAction(int id)
    {
        yield return new WaitForSeconds(IdsExitWaitTimeAfterUnlock);

        Destroy(game.GetMovingNPC(0).gameObject);
        game.ChangeStateInteract();
        game.DisableExits(false, 0);
        isUnlocked = true;
        isDone = true;
    }

    void SetupIOsDialogue(Script_DialogueNode node)
    {
        List<Script_InteractableObject> IOs = game.GetInteractableObjects();
        foreach (Script_InteractableObject IO in IOs)
        {
            if (IO.nameId == "chaise-lounge")
            {
                IO.SwitchDialogueNodes(new Script_DialogueNode[]{node});
            }
        }
    }

    public override void HandleExitCutScene()
    {
        game.ChangeStateCutScene();
        game.DestroyPlayer();
        audioSource.PlayOneShot(exitSFX, exitVol);
        
        // handle if player went back a room and activated persisting proximity speaker
        if (lb9.speaker != null)
        {
            lb9.speaker.audioSource.outputAudioMixerGroup = audioMixerGroup;
        }

        StartCoroutine(
            GetComponent<Script_AudioMixerFader>().Fade(
                audioMixer,
                "ExposedMasterVolume",
                fadeOutTransitionTime,
                0f,
                // continue to handle if player activated persisting proximity speaker from previous room
                () => {
                    if (lb9.speaker != null)
                    {
                        lb9.speaker.audioSource.Stop();
                        Destroy(lb9.speaker.gameObject);
                    }           
                }
            )
        );
        StartCoroutine(
            game.TransitionFadeIn(fadeOutTransitionTime, () => {
                audioSource.PlayOneShot(onExitFadeOutDoneSFX, onExitFadeOutDoneVol);
                game.MelanholicTitleCutScene();
                StartCoroutine(WaitToPlayTheme());
            })
        );
    }

    IEnumerator WaitToPlayTheme()
    {
        yield return new WaitForSeconds(transitionMusicPauseTime);
        
        StartCoroutine(
            GetComponent<Script_AudioMixerFader>().Fade(
                audioMixer,
                "ExposedMasterVolume",
                melanholicTitleFadeInTime,
                1f,
                null
            )
        );
    }

    public override void Setup()
    {
        if (!isInitialized)
        {
            crystalChandelier.SetActive(false);
            playerSpotLight.Setup(0f);
            IdsSpotLight.Setup(0f);
        }
        foreach (Transform t in IOTexts )   game.SetupInteractableObjectsText(t, !isInitialized);
        game.SetupMovingNPC(Ids, !isInitialized);
        isInitialized = true;
        
        if (!isDeskSwitchedIn)
        {
            ChaseLoungeSwitchIn();
        }
        else
        {
            DeskSwitchIn();
        }
        
        if (lb9.speaker == null)
        {
            game.SwitchBgMusic(4);
        }

        // TODO: COMBINE THIS STATE WITH DOOR LOCK?
        if (!isDone)
        {
            // we're in mid convo in last room
            if (activeTriggerIndex > 2)
            {
                // reset state to allow for Ids approach
                activeTriggerIndex = 2;
                isSpecterStoryStarted = false;
            }
            
            Ids.SetMute(true);
            if (activeTriggerIndex > 1)     Ids.SetMoveSpeedWalk();
            else                            Ids.SetMoveSpeedRun();
        }
        else
        {
            if (game.GetNPC(0) != null)     game.DestroyNPCs();
        }

        if (!isUnlocked)
        {
            game.DisableExits(true, 0);
        }
        else
        {
            game.DisableExits(false, 0);
            doorLock.gameObject.SetActive(false);
        }
    }
}
