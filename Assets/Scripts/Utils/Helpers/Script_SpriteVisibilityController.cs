using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_SpriteVisibilityController : MonoBehaviour
{
    public Vector3 targetLoc;
    public bool isAxisZ;
    public float timer;
    public float maxTimer;
    [SerializeField]
    private Script_Game g;
    [SerializeField]
    private Transform t;
    
    void Awake()
    {
        t = Script_Utils.GetChildren(this.gameObject);
        g = Object.FindObjectOfType<Script_Game>();

        if (targetLoc == Vector3.zero)
        {
            targetLoc = GetComponent<Transform>().position;
        }
    }
    
    // Update is called once per frame
    void LateUpdate()
    {
        timer -= Time.deltaTime;
        
        if (timer <= 0f)
        {
            timer = maxTimer;
            HandleVisibility();
        }
    }

    void HandleVisibility()
    {
        if (g.GetPlayer() != null)
        {
            Vector3 playerLoc = g.GetPlayerLocation();
            
            if (isAxisZ)
            {
                if (playerLoc.z >= targetLoc.z)     HandleVisibilityChange(false);
                else                                HandleVisibilityChange(true);
            }
            else
            {
                if (playerLoc.x >= targetLoc.x)     HandleVisibilityChange(false);
                else                                HandleVisibilityChange(true);
            }
        }
    }

    void HandleVisibilityChange(bool isOn)
    {
        if (t.childCount > 0)
        {
            foreach (Transform tr in t)
            {
                if (tr.GetComponent<Renderer>() != null)
                    tr.GetComponent<Renderer>().enabled = isOn;
            }
        }
        else
        {
            if (GetComponent<Renderer>() != null)
                GetComponent<Renderer>().enabled = isOn;
        }
    }
}
