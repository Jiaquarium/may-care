using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Script_ArrowOutline : MonoBehaviour
{
    public Sprite defaultSprite;
    public Sprite focusSprite;
    public GameObject flash;


    private IEnumerator focusCo;
    private IEnumerator lightUpCo;
    private float focusTimeLength;
    private float lightUpTimeLength;

    public void Focus()
    {
        GetComponent<Image>().sprite = focusSprite;

        if (focusCo != null)
        {
            StopCoroutine(focusCo);
        }

        focusCo = WaitToUnfocus();
        StartCoroutine(focusCo);
    }

    IEnumerator WaitToUnfocus()
    {
        yield return new WaitForSeconds(focusTimeLength);

        GetComponent<Image>().sprite = defaultSprite;     
    }

    public void LightUp()
    {
        flash.SetActive(true);

        if (lightUpCo != null)
        {
            StopCoroutine(lightUpCo);
        }

        lightUpCo = WaitToLightDown();
        StartCoroutine(lightUpCo);        
    }

    IEnumerator WaitToLightDown()
    {
        yield return new WaitForSeconds(focusTimeLength);

        LightDown();
    }

    public void LightDown()
    {
        flash.SetActive(false);
    }

    public void Setup(float _focusTimeLength, float _lightUpTimeLength)
    {
        focusTimeLength = _focusTimeLength;
        lightUpTimeLength = _lightUpTimeLength;
        LightDown();
    }
}
