using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Script_DialogueChoice : MonoBehaviour
{
    public Image outline;
    public bool isSelected;
    public int Id;

    void Update()
    {
        if (EventSystem.current.currentSelectedGameObject == this.gameObject)
        {
            outline.enabled = true;
            isSelected = true;
        }
        else
        {
            outline.enabled = false;
            isSelected = false;
        }
    }

    public void HandleSelect()
    {
        print("my id is: " + Id);

        // call choice manager to input this choice
    }
}
