using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Script_SBookViewController : MonoBehaviour
{
    public Script_SBookOverviewController sBookController;
    public Transform slotsHolder;
    [SerializeField] protected Transform[] slots;
    
    [SerializeField] protected int lastSlotIndex;
    [SerializeField] protected Transform lastSelected;
    [SerializeField] protected Script_SBookViewInputManager inputManager;

    // Update is called once per frame
    protected virtual void Update()
    {
        ShowActiveSlot();
        HandleExitInput();
    }

    protected virtual void HandleExitInput() {
        inputManager.HandleExitInput();    
    }

    void ShowActiveSlot()
    {
        if (
            EventSystem.current.currentSelectedGameObject != null
            && lastSelected != null
            && lastSelected.gameObject != null
            && EventSystem.current.currentSelectedGameObject != lastSelected.gameObject
        )
        {   
            SetLast();
        }
    }

    protected virtual void SetLast() {
        lastSelected = EventSystem.current.currentSelectedGameObject.transform;
        // lastSlotIndex = lastSelected.Id;
    }

    public void RehydrateState()
    {
        if (lastSelected == null)
        {
            InitializeState();
        }
        EventSystem.current.SetSelectedGameObject(lastSelected.gameObject);
    }

    void InitializeState()
    {
        lastSlotIndex = 0;
        lastSelected = slots[lastSlotIndex];
    }

    public void UpdateSlots()
    {
        Transform _slots = Script_Utils.GetChildren(slotsHolder.gameObject);
        
        slots = new Transform[_slots.childCount];
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i] = _slots.GetChild(i);
        }
    }

    public virtual void Setup()
    {
        UpdateSlots();

        inputManager = GetComponent<Script_SBookViewInputManager>();
        inputManager.Setup(sBookController);
    }
}
