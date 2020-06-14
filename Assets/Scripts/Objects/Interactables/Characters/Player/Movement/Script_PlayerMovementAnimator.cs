using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_PlayerMovementAnimator : MonoBehaviour
{
    public void AdjustRotation()
    {
        transform.forward = Camera.main.transform.forward;
    }

    public void Setup()
    {
        AdjustRotation();
    }
}
