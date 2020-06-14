using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Script_StickersViewController : Script_SBookViewController
{
    protected override void SetLast()
    {
        base.SetLast();
        lastSlotIndex = lastSelected.GetComponent<Script_StickersViewSlot>().Id;
    }

    public override void Setup()
    {
        base.Setup();
        for (int i = 0; i < slots.Length; i++)
            slots[i].GetComponent<Script_StickersViewSlot>().Id = i;
    }
}
