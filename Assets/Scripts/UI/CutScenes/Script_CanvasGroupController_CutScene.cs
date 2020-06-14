using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_CanvasGroupController_CutScene : Script_CanvasGroupController
{
    // don't set alpha
    public void SetActiveForFade()
    {
        CanvasGroup c = GetComponent<CanvasGroup>();
        c.gameObject.SetActive(true);
    }

    public void InitializeState()
    {
        Close();
    }

    public override void Setup()
    {
        Close();
    }
}