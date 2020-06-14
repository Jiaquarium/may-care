using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Script_TransitionManager : MonoBehaviour
{
    public Script_CanvasGroupFadeInOut fader;
    
    public IEnumerator FadeIn(float t, Action action)
    {
        return fader.FadeInCo(t, action);
    }
}
