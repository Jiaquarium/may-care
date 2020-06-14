using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_MovingNPC_Ids : Script_MovingNPC
{
    public float runSpeed;
    public float walkSpeed;
    public override void SetMoveSpeedRun()
    {
        speed = runSpeed;
    }

    public override void SetMoveSpeedWalk()
    {
        speed = walkSpeed;
    }
}
