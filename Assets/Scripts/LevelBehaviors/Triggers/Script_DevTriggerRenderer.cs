using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_DevTriggerRenderer : MonoBehaviour
{
    void Awake()
    {
        this.gameObject.SetActive(false);
        if (Debug.isDebugBuild && Const_Dev.IsDevMode)
            this.gameObject.SetActive(true);
    }
}
