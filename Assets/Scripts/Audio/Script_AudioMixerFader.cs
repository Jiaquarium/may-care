using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System;

public class Script_AudioMixerFader : MonoBehaviour
{
    public IEnumerator Fade(
        AudioMixer audioMixer,
        string exposedParam,
        float fadeTime,
        float targetVol,
        Action cb
    )
    {
        float currentTime = 0;
        float currentVol;
        audioMixer.GetFloat(exposedParam, out currentVol);
        currentVol = Mathf.Pow(10, currentVol / 20);
        float clampedVol = Mathf.Clamp(targetVol, 0.0001f, 1);

        while (currentTime < fadeTime)
        {
            currentTime += Time.deltaTime;
            float newVol = Mathf.Lerp(currentVol, clampedVol, currentTime / fadeTime);
            audioMixer.SetFloat(exposedParam, Mathf.Log(newVol) * 20);

            yield return null;
        }

        if (cb != null)     cb();
        yield break;
    }
}
