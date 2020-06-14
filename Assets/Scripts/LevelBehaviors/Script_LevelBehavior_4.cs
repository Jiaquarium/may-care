using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_LevelBehavior_4 : Script_LevelBehavior
{
    /* =======================================================================
        STATE DATA
    ======================================================================= */
    public bool isInitialized = false;
    public bool[] switchesStates;
    /* ======================================================================= */


    public Transform lightSwitchesParent;


    private Script_LBSwitchHandler switchHandler;
    

    public override void SetSwitchState(int Id, bool isOn)
    {
        switchHandler.SetSwitchState(switchesStates, Id, isOn);
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

        isInitialized = true;
    }
}
