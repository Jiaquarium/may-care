using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(SpriteRenderer))]
public class Script_SpriteFadeOut : MonoBehaviour
{
    public float fadeOutTime;

    public IEnumerator FadeOutCo(Action cb)
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();

        Color tmpColor = sr.color;

        while (tmpColor.a > 0f)
        {
            tmpColor.a -= Time.deltaTime / fadeOutTime;
            if (tmpColor.a <= 0f)   tmpColor.a = 0f;

            sr.color = tmpColor;

            yield return null;
        }

        sr.color = tmpColor;
        if (cb != null)    cb();
    }
}
