using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Script_LevelBehavior_6 : Script_LevelBehavior
{
    /* =======================================================================
        STATE DATA
    ======================================================================= */
    public bool[] switchesStates;
    public bool isPuzzleCompleted;
    /* ======================================================================= */

    public bool isInitialized;
    public AudioClip puzzleCompleteSFX;
    public AudioSource audioSource;
    public GameObject cameraFocalPoint;
    public GameObject mirrorReflection;
    public float mirrorRevealWaitTime;
    public float postMirrorRevealWaitTime;


    public bool[] puzzleCompleteSwitchesStates;
    public Tilemap completionGround;
    public GameObject completionGrid;
    public float volumeScale;
    public int switchesSortingLayerOffset;
    public Transform lightSwitchesParent;
    public Transform[] ptgTextParents;

    private Script_LBSwitchHandler switchHandler;


    private List<Script_InteractableObject> interactableObjects;
    

    protected override void OnDisable()
    {
        game.RemovePlayerReflection();
    }
    
    public override void SetSwitchState(int Id, bool isOn)
    {
        switchHandler.SetSwitchState(switchesStates, Id, isOn);
    }

    // only allow reading when light is turned on
    void HandleWallTextActive()
    {
        if (switchesStates.Length != 6)    return;
        
        interactableObjects[6].isActive = switchesStates[0];
        interactableObjects[7].isActive = switchesStates[0];
        
        interactableObjects[8].isActive = switchesStates[1];
        interactableObjects[9].isActive = switchesStates[1];

        interactableObjects[10].isActive = switchesStates[2];
        interactableObjects[11].isActive = switchesStates[2];

        interactableObjects[12].isActive = switchesStates[3];
        interactableObjects[13].isActive = switchesStates[3];

        interactableObjects[14].isActive = switchesStates[4];
        interactableObjects[15].isActive = switchesStates[4];

        interactableObjects[16].isActive = switchesStates[5];
        interactableObjects[17].isActive = switchesStates[5];
    }

    protected override void HandlePuzzle()
    {
        HandleWallTextActive();
        
        // check switchesStates with winState
        if (switchesStates == null)    return;

        for (int i = 0; i < puzzleCompleteSwitchesStates.Length; i++)
        {
            if (switchesStates[i] != puzzleCompleteSwitchesStates[i])
            {
                return;
            }
        }

        if (!isPuzzleCompleted)    OnPuzzleCompletion();
    }

    private void OnPuzzleCompletion()
    {
        isPuzzleCompleted = true;
        game.StopBgMusic();

        game.ChangeStateCutScene();
        
        // move camera
        game.ChangeCameraTargetToGameObject(cameraFocalPoint);
        
        // wait a bit before mirror reveal
        StartCoroutine(WaitToForMirrorReveal());
    }

    IEnumerator WaitToForMirrorReveal()
    {
        yield return new WaitForSeconds(mirrorRevealWaitTime);
        
        // play puzzle complete SFX and fade in grid
        audioSource.PlayOneShot(puzzleCompleteSFX, volumeScale);
        StartCoroutine(
            completionGround.GetComponent<Script_TileMapFadeIn>().FadeInCo(
                () => {
                    StartCoroutine(WaitToMoveAfterReveal());
                })
        );
        StartCoroutine(
            mirrorReflection.GetComponent<Script_SpriteFadeOut>().FadeOutCo(null)
        );
        completionGrid.SetActive(true);
        game.SetNewTileMapGround(completionGround);
        game.RemovePlayerReflection();
    }

    IEnumerator WaitToMoveAfterReveal()
    {
        yield return new WaitForSeconds(postMirrorRevealWaitTime);

        // return camera to player and change game state back
        // to interact so player can move again
        game.CameraTargetToPlayer();
        
        game.ChangeStateInteract();
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
        foreach(Transform t in ptgTextParents)  game.SetupInteractableObjectsText(t, !isInitialized);
        
        // for HandleWallTextActive polling
        interactableObjects = game.GetInteractableObjects();
        isInitialized = true;

        completionGrid.SetActive(isPuzzleCompleted);
        if (isPuzzleCompleted)
        {
            mirrorReflection.SetActive(false);
            game.RemovePlayerReflection();
            game.SetNewTileMapGround(completionGround);
        }
        else
        {
            // create "mirror"
            game.CreatePlayerReflection(game.Levels.levelsData[6].playerData.reflectionVector);
            mirrorReflection.SetActive(true);
        }

    }
}
