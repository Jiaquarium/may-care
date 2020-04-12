using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_LightSwitch : Script_Switch
{
    public Light[] lights;
    
    public float onIntensity;
    public float offIntensity;
    public float volumeScale;
    public AudioSource audioSource;
    public AudioClip onOffSFX;

    public override void TurnOn()
    {
        print("turning on light from Script_LightSwitch");
        base.TurnOn();
        
        audioSource.PlayOneShot(onOffSFX, volumeScale);

        foreach (Light l in lights)
        {
            l.intensity = onIntensity;
        }
    }

    public override void TurnOff()
    {
        print("turning off light from Script_LightSwitch");
        base.TurnOff();

        audioSource.PlayOneShot(onOffSFX, volumeScale);
        
        foreach (Light l in lights)
        {
            l.intensity = offIntensity;
        }
    }

    public override void SetupSwitch(bool _isOn)
    {
        base.SetupSwitch(_isOn);
    }

    public override void SetupLights(
        Light[] _lights,
        float _onIntensity,
        float _offIntensity,
        bool isOn
    )
    {
        lights = _lights;
        onIntensity = _onIntensity;
        offIntensity = _offIntensity;
        SetupSwitch(isOn);

        GetComponent<SpriteRenderer>().sprite = isOn ? onSprite : offSprite;
        
        foreach (Light l in lights)
        {
            l.intensity = isOn ? onIntensity : offIntensity;
        }
    }
}
