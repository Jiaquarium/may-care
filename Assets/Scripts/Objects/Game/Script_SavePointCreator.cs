using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_SavePointCreator : MonoBehaviour
{
    public void SetupSavePoint(Script_SavePoint sp, bool isInitialize)
    {
        if (isInitialize)
        {
            sp.Setup();
        }
    }
}
