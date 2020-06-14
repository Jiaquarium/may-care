using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Script_MoveDirection : MonoBehaviour
{
    public Transform t;
    [SerializeField]
    private Vector3 startLoc;
    [SerializeField]
    private float progress;
    
    void Awake()
    {
        if (t == null)
        {
            t = GetComponent<Transform>();
        }
    }

    public IEnumerator MoveSmooth(float maxTime, Vector3 moveOffset, Action cb)
    {
        if (t == null)
        {
            Debug.LogError("Script_MoveDirection Transform t is not set.");
            yield return null;
        }
        // move lights up
        startLoc = t.position;
        progress = 0;
        while (progress < 1f)
        {
            progress += Time.deltaTime / maxTime;

            if (progress >= 1f)
            {
                progress = 1f;
            }
            else
            {
                t.position = Vector3.Lerp(
                    startLoc,
                    startLoc + moveOffset,
                    progress    
                );
            }

            yield return null;
        }
        
        t.position = Vector3.Lerp(
            startLoc,
            startLoc + moveOffset,
            progress    
        );
        
        if (cb != null)    cb();
    }
}
