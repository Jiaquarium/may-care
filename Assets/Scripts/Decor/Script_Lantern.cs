using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_Lantern : MonoBehaviour
{
    public Light light;
    public Vector3 up;
    public Vector3 right;
    public Vector3 down;
    public Vector3 left;
    public float timerMax;
    
    private Vector3 upPos;
    private Vector3 rightPos;
    private Vector3 downPos;
    private Vector3 leftPos;
    private float timer;
    [SerializeField]
    private Vector3[] locs;
    [SerializeField]
    private int i;
    [SerializeField]
    private float progress;
    private Vector3 startLoc;
    
    void Awake()
    {
        upPos       = light.transform.position + up;
        rightPos    = light.transform.position + right;
        downPos     = light.transform.position + down;
        leftPos     = light.transform.position + left;
        locs = new Vector3[]{
            upPos, rightPos, downPos, leftPos
        };

        startLoc = locs[i];
        light.transform.position = startLoc;
        i++;
    }

    void Update()
    {
        progress += Time.deltaTime / timerMax;
        
        if (progress >= 1f)    progress = 1f;

        light.transform.position = Vector3.Lerp(
            startLoc,
            locs[i],
            progress
        );

        if (progress == 1f)
        {
            startLoc = locs[i];
            i++;
            if (i >= locs.Length)
            {
                i = 0;
            }
            progress = 0;
        }
    }
}
