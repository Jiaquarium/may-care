using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_PlayerInteractionBox : Script_InteractionBox
{
    public override Script_SavePoint GetSavePoint()
    {
        ExposeBox();
        foreach (Collider col in colliders)
        {
            if (col.tag == Const_Tags.SavePoint)
                return col.transform.parent.GetComponent<Script_SavePoint>();
        }

        return null;
    }

    public override Script_Pushable GetPushable()
    {
        ExposeBox();
        foreach (Collider col in colliders)
        {
            if (col.tag == Const_Tags.Pushable)
                return col.transform.parent.GetComponent<Script_Pushable>();
        }

        return null;
    }
}
