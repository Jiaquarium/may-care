using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Script_EventSystemLastSelected : MonoBehaviour {
    public GameObject currentSelected;
    // gets set to NULL on inventory close
    public GameObject lastSelected;
    
    void Update () {         
        if (EventSystem.current.currentSelectedGameObject == null)
        {
            EventSystem.current.SetSelectedGameObject(currentSelected);
        }
        else
        {
            lastSelected = currentSelected;
            currentSelected = EventSystem.current.currentSelectedGameObject;
        }
    }
}