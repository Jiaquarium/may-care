using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_Switch : Script_InteractableObject
{
    public int switchId;
    public Sprite onSprite;
    public Sprite offSprite;
    public bool isOn;

    public virtual void TurnOff()
    {
        isOn = false;
        GetComponent<SpriteRenderer>().sprite = offSprite;
        
        print("turnoff() my switchId: " + switchId);

        game.SetSwitchState(switchId, false);
    }

    public virtual void TurnOn()
    {
        isOn = true;
        GetComponent<SpriteRenderer>().sprite = onSprite;
        
        print("turnon() my switchId: " + switchId);

        game.SetSwitchState(switchId, true);
    }

    public override void ActionDefault()
    {
        if (isOn)   TurnOff();
        else TurnOn();
    }

    public override void SetupSwitch(bool _isOn)
    {
        isOn = _isOn;
    }
}
