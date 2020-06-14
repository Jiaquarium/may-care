using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_SBookCoverButton : MonoBehaviour
{
    public Script_SBookOverviewController SBookController;

    // called on button "click" (enter on button)
    public void OnEnter()
    {
        SBookController.EnterSBook();
    }
}
