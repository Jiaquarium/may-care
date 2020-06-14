using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Script_Arrow : MonoBehaviour
{
    public string type;
    public Vector3 startLocation;
    public Vector3 endLocation;
    public Vector3 secondEndLocation;
    public float t;
    public float t2;
    public bool isClicked = false;
    public Sprite clickedSprite;
    
    
    private Script_DDRManager DDRManager;
    private float timeToReachEndLocation;
    private bool isMoving;
    private bool isPassingOutline;
    private bool isReported;
    private float tierNeg1Buffer;
    private float tier1Buffer;
    private float tier2Buffer;
    private float tier3Buffer;

    void Update()
    {
        if (isMoving)
        {
            if (isPassingOutline)
            {
                SecondRise();
                return;
            }

            Rise();
        }

        if (isClicked)
        {
            GetComponent<Image>().sprite = clickedSprite; 
        }
    }
    
    void Rise()
    {
        t += Time.deltaTime / timeToReachEndLocation;
        GetComponent<RectTransform>().localPosition = Vector3.Lerp(
                startLocation,
                endLocation,
                t
            );

        if (Debug.isDebugBuild && Const_Dev.IsDevMode)
        {
            ChangeColorOnRise();
        }

        // continue to move arrow off screen when done lerping
        if (t >= 1f)
        {
            isPassingOutline = true;
            startLocation = GetComponent<RectTransform>().localPosition;
        }
    }

    void ChangeColorOnRise()
    {
        if (1f - t <= tier1Buffer)
        {
            GetComponent<Image>().color = Color.green;
        }
        else if (1f - t <= tier2Buffer)
        {
            GetComponent<Image>().color = Color.yellow;
        }
        else if (1f - t <= tier3Buffer)
        {
            GetComponent<Image>().color = Color.red;
        }
    }

    // to continue to move offscreen
    void SecondRise()
    {
        t2 += Time.deltaTime / timeToReachEndLocation;
        GetComponent<RectTransform>().localPosition = Vector3.Lerp(
                startLocation,
                secondEndLocation,
                t2
            );

        if (Debug.isDebugBuild && Const_Dev.IsDevMode)
        {
            ChangeColorOnSecondRise();
        }

        // case where arrow passes outline and is not clicked in time
        if (t2 > tierNeg1Buffer)
        {
            if (!isReported)
            {
                DDRManager.ReportArrowTier(this);
                if (type == "left")             DDRManager.nextLeftArrowIndex++;
                else if (type == "down")        DDRManager.nextDownArrowIndex++;
                else if (type == "up")          DDRManager.nextUpArrowIndex++;
                else if (type == "right")       DDRManager.nextRightArrowIndex++;
            }
        }

        // destroy arrow when done with second lerp
        if (t2 >= 1f)
        {
            DestroyArrow();
        }
    }

    public float ReportTier()
    {
        isReported = true;

        if (isPassingOutline)
        {
            return -t2;
        }
        else
        {
            return 1f - t;
        }
    }

    void ChangeColorOnSecondRise()
    {
        if (t2 > tierNeg1Buffer)
        {
            GetComponent<Image>().color = Color.red;
        }
    }

    public void BeginRising()
    {
        isMoving = true;
    }

    public void DestroyArrow()
    {
        if (this != null && this.gameObject != null)
        {
            Destroy(this.gameObject);
        }
    }
    
    public void Setup(
        Vector3 target,
        float time,
        float _tierNeg1Buffer,
        float _tier1Buffer,
        float _tier2Buffer,
        float _tier3Buffer,
        Script_DDRManager _DDRManager
    )
    {
        tierNeg1Buffer          = _tierNeg1Buffer;
        tier1Buffer             = _tier1Buffer;
        tier2Buffer             = _tier2Buffer;
        tier3Buffer             = _tier3Buffer;
        DDRManager              = _DDRManager;
        t                       = 0;
        startLocation           =  GetComponent<RectTransform>().localPosition;
        timeToReachEndLocation  = time;
        endLocation             = target;
        secondEndLocation       = endLocation + (endLocation - startLocation);
    }
}
