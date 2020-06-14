using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_LevelBehavior_11 : Script_LevelBehavior
{
    public Script_SavePoint sp;
    public bool isInitialize = true;

    public override void Setup()
    {
        game.SetupSavePoint(sp, isInitialize);
        isInitialize = false;
    }
}
