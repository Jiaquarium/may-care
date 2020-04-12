using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_InteractableObject : MonoBehaviour
{
    public int Id;
    public Script_Game game;
    
    // Update is called once per frame
    public virtual void Update() {}

    public virtual void ActionDefault() { print("DefaultAction() called in InteractableObject"); }
    
    public virtual void ActionB() {}
    
    public virtual void ActionC() {}
    
    public virtual void Setup() {
        print("Setting up IObj");
        game = FindObjectOfType<Script_Game>();
    }
    
    public virtual void SetupSwitch(bool isOn) {}
    
    public virtual void SetupLights(
        Light[] lights,
        float onIntensity,
        float offIntensity,
        bool isOn
    ){}
    
    public virtual void SetupText(
        Script_DialogueManager dm,
        Script_Player p,
        Model_Dialogue d
    ){}
}
