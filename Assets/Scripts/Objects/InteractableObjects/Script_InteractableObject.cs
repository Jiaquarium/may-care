using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_InteractableObject : MonoBehaviour
{
    public int Id;
    
    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual void DefaultAction()
    {
        print("DefaultAction() called in InteractableObject");
    }

    public virtual void Setup()
    {
        print("Setting up IObj");
    }

    public virtual void SetupLights(Light[] lights, float onIntensity, float offIntensity){}
}
