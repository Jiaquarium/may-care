using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Script_CloseSBookButton : MonoBehaviour, ISelectHandler
{
    public Script_SBookOverviewController SBookController;
    public GameObject fromLeftButton;
    public GameObject fromRightButton;
    [SerializeField]
    private GameObject lastSelectedNotThis;
    

    private void Update() {
        if (
            EventSystem.current.currentSelectedGameObject != lastSelectedNotThis
            && EventSystem.current.currentSelectedGameObject != this.gameObject
        )
        {
            lastSelectedNotThis = EventSystem.current.currentSelectedGameObject;
        }
    }

    // OnSelect always happens before Update but before safe I added the line
    // && EventSystem.current.currentSelectedGameObject != this.gameObject
    public void OnSelect(BaseEventData e)
    {
        if (lastSelectedNotThis == fromLeftButton)
        {
            Navigation btnNav = GetComponent<Selectable>().navigation;
            btnNav.selectOnDown = fromLeftButton.GetComponent<Button>();
            GetComponent<Selectable>().navigation = btnNav;
        }
        else if (lastSelectedNotThis == fromRightButton)
        {
            Navigation btnNav = GetComponent<Selectable>().navigation;
            btnNav.selectOnDown = fromRightButton.GetComponent<Button>();
            GetComponent<Selectable>().navigation = btnNav;
        }
    }

    // called on button "click" (enter on button)
    public void OnEnter()
    {
        SBookController.ExitSBook();
    }
}
