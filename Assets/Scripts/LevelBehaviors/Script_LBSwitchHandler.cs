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
        bool isActivated
    )
    {
        if (isActivated)
        {
            game.CreateInteractableObjects(switchesStates);
        }
        else
        {
            game.CreateInteractableObjects(null);
            
            switchesStates = new bool[game.GetSwitchesCount()];
            SetInitialSwitchesState(switchesStates);
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
