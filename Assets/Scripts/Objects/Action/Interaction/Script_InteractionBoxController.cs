using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_InteractionBoxController : MonoBehaviour
{
    [SerializeField] private Script_InteractionBox InteractionBoxN;
    [SerializeField] private Script_InteractionBox InteractionBoxE;
    [SerializeField] private Script_InteractionBox InteractionBoxS;
    [SerializeField] private Script_InteractionBox InteractionBoxW;
    public Script_InteractionBox activeBox;

    public void HandleActiveInteractionBox(string dir)
    {
        if (dir == Const_Directions.Up)         SetActiveInteractionBox(InteractionBoxN);
        else if (dir == Const_Directions.Right) SetActiveInteractionBox(InteractionBoxE);
        else if (dir == Const_Directions.Down)  SetActiveInteractionBox(InteractionBoxS);
        else if (dir == Const_Directions.Left)  SetActiveInteractionBox(InteractionBoxW);
    }

    void SetActiveInteractionBox(Script_InteractionBox box)
    {
        InteractionBoxN.isExposed = false;
        InteractionBoxE.isExposed = false;
        InteractionBoxS.isExposed = false;
        InteractionBoxW.isExposed = false;
        
        activeBox = box;
        box.isExposed = true;
    }

    public Script_SavePoint GetSavePoint(string dir)
    {
        HandleActiveInteractionBox(dir);
        return activeBox.GetSavePoint();
    }

    public List<Script_Interactable> GetInteractables(string dir)
    {
        HandleActiveInteractionBox(dir);
        return activeBox.GetInteractables();
    }

    public Script_Pushable GetPushable(string dir)
    {
        HandleActiveInteractionBox(dir);
        return activeBox.GetPushable();
    }
}
