using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_EntriesViewInputManager : Script_SBookViewInputManager
{
    protected override void ExitView()
    {
        sBookController.ExitEntriesView();
    }    
}
