using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_StickersInputManager : Script_SBookViewInputManager
{
    protected override void ExitView()
    {
        sBookController.ExitStickerView();
    }
}
