using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_LightSwitch : Script_Switch
{
    public Light[] lights;
    
    public float onIntensity;
    public float offIntensity;

    public override void TurnOn()
    {
        print("turning on light from Script_LightSwitch");
        base.TurnOn();
        
        foreach (Light l in lights)
        {
            l.intensity = onIntensity;
        }
    }

    public override void TurnOff()
    {
        print("turning off light from Script_LightSwitch");
        base.TurnOff();
        
        foreach (Light l in lights)
        {
            l.intensity = offIntensity;
        }
    }

    public override void SetupLights(
        Light[] _lights,
        float _onIntensity,
        float _offIntensity
    )
    {
        lights = _lights;
        onIntensity = _onIntensity;
        offIntensity = _offIntensity;
    }
}
