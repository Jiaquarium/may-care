using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_SBookViewInputManager : MonoBehaviour
{
    protected Script_SBookOverviewController sBookController;

    public void HandleExitInput()
    {
        if (Input.GetButtonDown("Inventory") || Input.GetButtonDown("Cancel"))
        {
            ExitView();
        }
    }

    protected virtual void ExitView() { }

    public void Setup(
        Script_SBookOverviewController _SBookController
    )
    {
        sBookController = _SBookController;
    }
}
