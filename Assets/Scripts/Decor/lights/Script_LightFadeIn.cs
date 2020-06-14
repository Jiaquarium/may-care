using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Script_LightFadeIn : MonoBehaviour
{
    public Transform t;
    public Vector3 offset;
    
    [SerializeField]
    private float maxIntensity;
    [SerializeField]
    private float tmpIntensity;
    private Light light;

    public IEnumerator FadeInLightOnTarget(
        float maxTime,
        Transform target,
        Action cb
    )
    {
        t.position = target.position + offset;

        while (light.intensity < maxIntensity)
        {
            tmpIntensity += (Time.deltaTime / maxTime) * maxIntensity;

            if (tmpIntensity > maxIntensity)
            {
                tmpIntensity = maxIntensity;
            }

            light.intensity = tmpIntensity;
            yield return null;
        }

        if (cb != null)    cb();
    }

    public IEnumerator FadeOutLight(
        float maxTime,
        Action cb
    )
    {
        while (light.intensity > 0f)
        {
            tmpIntensity -= (Time.deltaTime / maxTime) * maxIntensity;

            if (tmpIntensity < 0f)
            {
                tmpIntensity = 0f;
            }

            light.intensity = tmpIntensity;
            yield return null;
        }

        if (cb != null)    cb();
    }

    public void Setup(float startingIntensity)
    {
        t = GetComponent<Transform>();
        light = GetComponent<Light>();
        light.intensity = startingIntensity;
        tmpIntensity = startingIntensity;
    }
}
