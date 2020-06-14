using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class Script_CanvasGroupController : MonoBehaviour
{
    public virtual void Open()
    {
        CanvasGroup c = GetComponent<CanvasGroup>();
        c.alpha = 1f;
        c.gameObject.SetActive(true);
    }

    public virtual void Close()
    {
        CanvasGroup c = GetComponent<CanvasGroup>();
        c.alpha = 0f;
        c.gameObject.SetActive(false);
    }

    public virtual void Setup()
    {

    } 
}
