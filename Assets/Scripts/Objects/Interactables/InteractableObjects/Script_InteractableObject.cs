using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_InteractableObject : Script_Interactable
{
    public int Id;
    public string nameId;
    public Script_Game game;
    public Vector3 worldOffset;
    public bool isActive = true;
    [SerializeField] protected Transform rendererChild;
    
    // Update is called once per frame
    public virtual void Update() {}

    public virtual void ActionDefault() {}
    
    public virtual void ActionB() {}
    
    public virtual void ActionC() {}
    
    public virtual void Setup(
        bool isForceSortingLayer,
        bool isAxisZ,
        int offset
    )
    {
        game = FindObjectOfType<Script_Game>();
        if (isForceSortingLayer)    EnableSortingOrder(isAxisZ, offset); 
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

    public virtual void SwitchDialogueNodes(Script_DialogueNode[] nodes){}

    public virtual void EnableSortingOrder(bool isAxisZ, int offset)
    {
        rendererChild.GetComponent<Script_SortingOrder>().EnableWithOffset(offset, isAxisZ);
    }

    public Transform GetRendererChild()
    {
        return rendererChild;
    }
}
