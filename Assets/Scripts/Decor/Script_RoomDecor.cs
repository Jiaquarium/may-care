using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_RoomDecor : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        AdjustRotation();    
    }
    // Update is called once per frame
    void Update()
    {
        AdjustRotation();    
    }

    public void AdjustRotation()
    {
        transform.forward = Camera.main.transform.forward;
    }
}
