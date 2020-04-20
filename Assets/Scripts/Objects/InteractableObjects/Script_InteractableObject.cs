using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_InteractableObject : MonoBehaviour
{
    public int Id;
    public Script_Game game;
    public bool isActive = true;
    
    // Update is called once per frame
    public virtual void Update() {}

    public virtual void ActionDefault() { print("DefaultAction() called in InteractableObject"); }
    
    public virtual void ActionB() {}
    
    public virtual void ActionC() {}
    
    public virtual void Setup() {
        game = FindObjectOfType<Script_Game>();
    }
    
    public virtual void SetupSwitch(
        bool isOn,
        Sprite onSprite,
        Sprite offSprite
    ) {}
    
    public virtual void SetupLights(
        Light[] lights,
        float onIntensity,
        float offIntensity,
        bool isOn,
        Sprite onSprite,
        Sprite offSprite
    ){}
    
    public virtual void SetupText(
        Script_DialogueManager dm,
        Script_Player p,
        Model_Dialogue d
    ){}
}
