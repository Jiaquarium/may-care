using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Script_HealthHeart : MonoBehaviour
{
    public Sprite filledSprite;
    public Sprite emptySprite;
    
    // add in coroutine for animation
    // TODO: throttle fills and empties
    public void Fill()
    {
        GetComponent<Image>().sprite = filledSprite;
    }

    public void Empty()
    {
        GetComponent<Image>().sprite = emptySprite;
    }

    public void Setup(bool isFilled)
    {
        if (isFilled)   Fill();
        else            Empty();
    }
}
