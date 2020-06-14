using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_LBSwitchHandler : MonoBehaviour
{
    Script_Game game;

    public void SetSwitchState(bool[] switchesStates, int Id, bool isOn)
    {
        switchesStates[Id] = isOn;
    }

    public bool[] CreateIObjsWithSwitchesState(
        bool[] switchesStates,
        bool isActivated,
        bool isForceSortingLayer,
        bool isSortingLayerAxisZ = true,
        int offset = 0
    )
    {
        if (isActivated)
        {
            game.CreateInteractableObjects(
                switchesStates,
                isForceSortingLayer,
                isSortingLayerAxisZ,
                offset
            );
        }
        else
        {
            game.CreateInteractableObjects(
                null,
                isForceSortingLayer,
                isSortingLayerAxisZ,
                offset
            );
            
            switchesStates = new bool[game.GetSwitchesCount()];
            SetInitialSwitchesState(switchesStates);
        }

        return switchesStates;
    }

    public bool[] SetupIObjsWithSwitchesState(
        Transform lightSwitchesParent,
        bool[] switchesStates,
        bool isInitialize
    )
    {
        if (isInitialize)
        {
            game.SetupLightSwitches(
                lightSwitchesParent,
                switchesStates,
                isInitialize
            );

            switchesStates = new bool[game.GetSwitchesCount()];
            SetInitialSwitchesState(switchesStates);
        }
        else
        {
            game.SetupLightSwitches(
                lightSwitchesParent,
                switchesStates,
                isInitialize
            );
        }
        
        return switchesStates;   
    }

    private void SetInitialSwitchesState(bool[] switchesStates)
    {
        for (int i = 0; i < switchesStates.Length; i++)
        {
            switchesStates[i] = game.GetSwitch(i).isOn;
        }
    }

    public void Setup(Script_Game _game)
    {
        game = _game;
    }
}
