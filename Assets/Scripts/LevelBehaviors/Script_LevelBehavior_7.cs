using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Script_LevelBehavior_7 : Script_LevelBehavior
{
    public bool isDone = false;
    public bool isActivated = false;
    
    
    public Script_CutSceneNPC cutSceneNPC;
    public float demonRevealWaitTime;
    public float postDemonRevealWaitTime;
    public float cagesRevealWaitTime;
    public float postCagesRevealWaitTime;
    public float postZoomInWaitTime;
    public float zoomInOnMelzSize;
    public float moveToMelzSmoothTime;
    public float zoomSmoothTime;
    public float postFinalWordsWaitTime;
    public float hintTimeBeforeShowing;
    public string hint;
    public Vector3 zoomPosition;
    public float spotlightVolumeScale;
    public float zoomOutDemonsSize;
    public float zoomOutCagesSize;
    
    public Script_BgThemePlayer MelzBgThemePlayerPrefab;
    public AudioSource audioSource;
    public AudioClip SpotlightSFX;
    public GameObject MelzSpotlight;
    public GameObject demonsAndSpotlights;
    public GameObject demonSpotLights;
    public GameObject cages;
    public Script_DialogueManager dm;
    public Script_DialogueNode ateDemonsNode;
    public Script_DialogueNode sparedDemonsNode;
    public Script_DialogueNode showDemonsNode;
    public Script_DialogueNode showCagesNode;
    public Script_DialogueNode noseRingNode;
    public Script_DialogueNode finalWordsNode;
    public Script_LevelBehavior_3 level3;

    [SerializeField]
    private bool didEatDemons = false;
    [SerializeField]
    private bool shownHint = false;
    private Coroutine hintCoroutine;
    
    protected override void OnDisable() {
        if (game.camera != null)
        {
            game.SetOrthographicSizeDefault();
        }
    }

    public override void HandleDialogueNodeAction(string action)
    {
        if (action == "reveal-melz")            MelzReveal();
        else if (action == "reveal-demons")     StartCoroutine(WaitToRevealDemons());
        else if (action == "reveal-cages")      StartCoroutine(WaitToRevealCages());
        else if (action == "nose-ring-zoom")    StartCoroutine(WaitToZoomOnNoseRing());
        else if (action == "final-words")       StartCoroutine(WaitToMelzExit());
    }

    public override void HandleDialogueNodeUpdateAction(string action)
    {
        if (action == "inventory-open")         HandleInventoryOpenAndClose();
    }

    protected override void HandleOnEntrance()
    {
        if (game.state == "interact" && !isActivated)
        {
            isActivated = true;

            foreach(bool demonSpawn in level3.demonSpawns)
            {
                if (!demonSpawn)    didEatDemons = true;
            }
            if (didEatDemons)   dm.StartDialogueNode(ateDemonsNode);
            else                dm.StartDialogueNode(sparedDemonsNode);
            
            game.ChangeStateCutScene();
        }
    }

    void MelzReveal()
    {
        game.SetCameraOffset(Vector3.zero);
        game.CameraMoveToTargetSmooth(
            moveToMelzSmoothTime,
            zoomPosition,
            null
        );
        audioSource.PlayOneShot(SpotlightSFX, spotlightVolumeScale);
        game.GetNPC(0).SetVisibility(true);
        MelzSpotlight.SetActive(true);
        game.PauseBgMusic();
        game.PlayNPCBgTheme(MelzBgThemePlayerPrefab);
    }
    
    IEnumerator WaitToRevealDemons()
    {
        audioSource.PlayOneShot(SpotlightSFX, spotlightVolumeScale);
        game.SetOrthographicSize(zoomOutDemonsSize);
        demonsAndSpotlights.SetActive(true);
        demonSpotLights.SetActive(true);

        yield return new WaitForSeconds(postDemonRevealWaitTime);
        dm.StartDialogueNode(showCagesNode, false);
    }

    IEnumerator WaitToRevealCages()
    {
        audioSource.PlayOneShot(SpotlightSFX, spotlightVolumeScale);
        game.SetOrthographicSize(zoomOutCagesSize);
        cages.SetActive(true);
        demonSpotLights.SetActive(false);

        yield return new WaitForSeconds(postCagesRevealWaitTime);
        dm.StartDialogueNode(noseRingNode, false);
    }

    IEnumerator WaitToZoomOnNoseRing()
    {
        Script_StaticNPC npc = game.GetNPC(0);
        
        game.CameraSetIsTrackingTarget(false);
        game.CameraMoveToTargetSmooth(
            moveToMelzSmoothTime,
            zoomPosition - Camera.main.GetComponent<Script_Camera>().offset,
            () => {
                game.CameraZoomSmooth(
                    zoomInOnMelzSize,
                    zoomSmoothTime,
                    zoomPosition - Camera.main.GetComponent<Script_Camera>().offset,
                    null
                );
            }
        );

        // wait for lerp and zoom smooth times
        yield return new WaitForSeconds(moveToMelzSmoothTime + zoomSmoothTime);
        npc.Freeze(true);        
        npc.Glimmer();

        yield return new WaitForSeconds(postZoomInWaitTime);
        npc.Freeze(false);
        dm.StartDialogueNode(finalWordsNode, false);
    }
    
    IEnumerator WaitToMelzExit()
    {
        yield return new WaitForSeconds(postFinalWordsWaitTime);
        
        MelzExit();
    }

    void ChangeGameStateInteract()
    {
        game.ChangeStateInteract();
    }

    void MelzExitCallback()
    {
        MelzSpotlight.SetActive(false);
        dm.EndDialogue();
        game.ChangeStateInteract();
        game.StopMovingNPCThemes();
        game.UnPauseBgMusic();

        game.CameraInstantMoveSpeed();
        game.CameraSetIsTrackingTarget(true);
        game.CameraTargetToPlayer();
        game.SetOrthographicSizeDefault();
        game.SetCameraOffsetDefault();
        game.CameraDefaultMoveSpeed();
        
        isDone = true;
        this.gameObject.SetActive(false);
        game.ClearNPCs();
    }

    void MelzExit()
    {
        game.GetNPC(0).FadeOut(MelzExitCallback);
    }

    // handle inventory here since player is still in talking mode
    void HandleInventoryOpenAndClose()
    {
        if (!shownHint)
        {
            shownHint = true;
            hintCoroutine = StartCoroutine(WaitToShowHint());
        }
        
        if (Input.GetButtonDown("Inventory") && game.state == Const_States_Game.CutScene)
        {
            game.OpenInventory();
            StopCoroutine(hintCoroutine);
            game.HideHint();
        }
    }

    public override void OnCloseInventory()
    {
        if (!isDone)
        {
            dm.StartDialogueNode(showDemonsNode, false);
        }
    }

    IEnumerator WaitToShowHint()
    {
        yield return new WaitForSeconds(hintTimeBeforeShowing);
        game.ShowHint(hint);
    }

    protected override void HandleAction()
    {
        base.HandleDialogueAction();
    }

    public override void Setup()
    {
        if (!isDone)
        {
            game.SetupCutSceneNPC(cutSceneNPC);
            game.GetNPC(0).SetVisibility(false);
            MelzSpotlight.SetActive(false);
            demonsAndSpotlights.SetActive(false);
            demonSpotLights.SetActive(false);
            cages.SetActive(false);
        }
        else
        {
            demonsAndSpotlights.SetActive(true);
            demonSpotLights.SetActive(false);
            MelzSpotlight.SetActive(false);
            cages.SetActive(true);
        }
    }
}
