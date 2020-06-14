using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

[RequireComponent(typeof(CanvasGroup))]
public class Script_CanvasGroupFadeInOut : MonoBehaviour
{
    public IEnumerator FadeInCo(float t, Action cb)
    {
        CanvasGroup cg = GetComponent<CanvasGroup>();

        float alpha = cg.alpha;

        while (alpha < 1f)
        {
            alpha += Time.deltaTime / t;
            if (alpha > 1f)   alpha = 1f;

            cg.alpha = alpha;

            yield return null;
        }

        if (cb != null)    cb();
    }
    
    public IEnumerator FadeOutCo(float t, Action cb)
    {
        CanvasGroup cg = GetComponent<CanvasGroup>();

        float alpha = cg.alpha;

        while (alpha < 1f)
        {
            alpha += Time.deltaTime / t;
            if (alpha > 1f)   alpha = 1f;

            cg.alpha = alpha;

            yield return null;
        }

        if (cb != null)    cb();
    }
}
