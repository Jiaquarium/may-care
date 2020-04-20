using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_CutSceneNPC : Script_StaticNPC
{
    public int CutSceneNPCId;

    public override void Setup
    (
        Sprite sprite,
        Model_Dialogue dialogue,
        Model_MoveSet[] movesets
    )
    {
        Debug.Log("setup in CutSceneNPC");

        base.Setup(sprite, dialogue, movesets);
    }
}
