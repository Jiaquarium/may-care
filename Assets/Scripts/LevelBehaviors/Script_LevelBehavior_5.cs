using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_LevelBehavior_5 : Script_LevelBehavior
{
    /* =======================================================================
        STATE DATA
    ======================================================================= */
    public bool isInitialized;

    /* ======================================================================= */

    public Transform[] paintings;
    
    public override void Setup()
    {
        foreach(Transform t in paintings)   game.SetupInteractableObjectsText(t, !isInitialized);
    }
}
