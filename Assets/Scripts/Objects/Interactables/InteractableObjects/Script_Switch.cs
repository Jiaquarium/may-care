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
        rendererChild.GetComponent<SpriteRenderer>().sprite = offSprite;
        
        game.SetSwitchState(switchId, false);
    }

    public virtual void TurnOn()
    {
        isOn = true;
        rendererChild.GetComponent<SpriteRenderer>().sprite = onSprite;
        
        game.SetSwitchState(switchId, true);
    }

    public override void ActionDefault()
    {
        if (isOn)   TurnOff();
        else TurnOn();
    }

    public override void SetupSwitch(
        bool _isOn,
        Sprite _onSprite,
        Sprite _offSprite
    )
    {
        isOn = _isOn;
        
        if (_onSprite != null)    onSprite = _onSprite;
        if (_offSprite != null)   offSprite = _offSprite;

        rendererChild.GetComponent<SpriteRenderer>().sprite = isOn ? onSprite : offSprite;
    }
}
