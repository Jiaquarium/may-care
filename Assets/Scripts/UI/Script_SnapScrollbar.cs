using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Scrollbar))]
public class Script_SnapScrollbar : MonoBehaviour
{
    public Transform slotHolder;
    
    [SerializeField] private int childCount;
    [SerializeField] private int selectedChildIndex;

    // Update is called once per frame
    void Update()
    {
        // get selected one
        // find the id of selected
        Transform selectedChild = EventSystem.current.currentSelectedGameObject.transform;
        // find out which child this is
        childCount = slotHolder.childCount;

        for (int i = 0; i < childCount; i++)
        {
            if (slotHolder.GetChild(i) == selectedChild)    selectedChildIndex = i;
        }
        // do id + 1 / total # of children
        
        float scrollPosition = ((float)childCount - (float)selectedChildIndex) / (float)childCount;
        if (selectedChildIndex + 1 == childCount)   scrollPosition = 0;
        
        GetComponent<Scrollbar>().value = scrollPosition;
    }
}
