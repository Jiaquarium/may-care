using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_Switch : Script_InteractableObject
{
    public Sprite onSprite;
    public Sprite offSprite;
    public bool isOn = true;

    public virtual void TurnOff()
    {
        isOn = false;
        GetComponent<SpriteRenderer>().sprite = offSprite;
    }

    public virtual void TurnOn()
    {
        isOn = true;
        GetComponent<SpriteRenderer>().sprite = onSprite;
    }

    public override void DefaultAction()
    {
        if (isOn)   TurnOff();
        else TurnOn();
    }
}
