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
    public bool isActivated;
    public bool isPuzzleCompleted;
    /* ======================================================================= */

    public AudioClip puzzleCompleteSFX;
    public AudioSource audioSource;
    public GameObject cameraFocalPoint;
    public float mirrorRevealWaitTime;
    public float postMirrorRevealWaitTime;


    public bool[] puzzleCompleteSwitchesStates;
    public Tilemap Ground;
    public GameObject completionGrid;
    public float volumeScale;

    private Script_LBSwitchHandler switchHandler;


    private List<Script_InteractableObject> interactableObjects;
    

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
        print("PUZZLE COMPLETE!!!");

        game.ChangeStateCutScene();
        
        // move camera
        game.ChangeCameraTargetToGameObject(cameraFocalPoint);
        
        // wait a bit before mirror reveal
        StartCoroutine(WaitToForMirrorReveal());
    }

    IEnumerator WaitToForMirrorReveal()
    {
        yield return new WaitForSeconds(mirrorRevealWaitTime);

        // play sound effect and activate 6:1 grid
        audioSource.PlayOneShot(puzzleCompleteSFX, volumeScale);

        completionGrid.SetActive(true);
        game.SetNewTileMapGround(Ground);
        game.RemovePlayerReflection();
        
        StartCoroutine(WaitToMoveAfterReveal());
    }

    IEnumerator WaitToMoveAfterReveal()
    {
        yield return new WaitForSeconds(postMirrorRevealWaitTime);

        // return camera to player and change game state back
        // to interact so player can move again
        game.CameraTargetToPlayer();
        game.CameraMoveToTarget();
        
        game.ChangeStateInteract();
    }
    
    public override void Setup()
    {
        switchHandler = GetComponent<Script_LBSwitchHandler>();
        switchHandler.Setup(game);
        switchesStates = switchHandler
            .CreateIObjsWithSwitchesState(switchesStates, isActivated);

        if (!isActivated)   isActivated = true;

        completionGrid.SetActive(isPuzzleCompleted);
        if (isPuzzleCompleted)
        {
            // remove "mirror"
            game.RemovePlayerReflection();
            game.SetNewTileMapGround(Ground);
        }

        interactableObjects = game.GetInteractableObjects();
    }
}
