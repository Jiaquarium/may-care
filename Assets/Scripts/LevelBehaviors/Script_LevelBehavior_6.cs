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


    public bool[] puzzleCompleteSwitchesStates;
    public Tilemap Ground;
    public GameObject completionGrid;

    private Script_LBSwitchHandler switchHandler;
    

    public override void SetSwitchState(int Id, bool isOn)
    {
        switchHandler.SetSwitchState(switchesStates, Id, isOn);
    }

    protected override void HandlePuzzle()
    {
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
        // set active 6:1 grid
        completionGrid.SetActive(true);
        game.SetNewTileMapGround(Ground);
    }
    
    public override void Setup()
    {
        switchHandler = GetComponent<Script_LBSwitchHandler>();
        switchHandler.Setup(game);
        switchesStates = switchHandler
            .CreateIObjsWithSwitchesState(switchesStates, isActivated);

        if (!isActivated)   isActivated = true;

        completionGrid.SetActive(isPuzzleCompleted);
    }
}
