using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_PlayerReflectionInteractionBox : Script_InteractionBox
{
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
