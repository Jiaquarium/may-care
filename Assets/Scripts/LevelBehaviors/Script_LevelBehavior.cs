using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_LevelBehavior : MonoBehaviour
{
    public Script_Game game;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        HandleTriggerLocations();
        HandleAction();
        HandlePuzzle();
        HandleOnEntrance();
    }

    protected virtual void HandleTriggerLocations() {}
    protected virtual void HandleAction() {}
    protected virtual void HandlePuzzle() {}
    protected virtual void HandleOnEntrance() {}
    // TODO: MOVE THIS LOGIC INTO PLAYERACTION
    protected virtual void HandleDialogueAction()
    {
        if (
            Input.GetButtonDown(Const_KeyCodes.Action1)
            && game.state == "cut-scene"
        )
        {
            game.HandleContinuingDialogueActions(Const_KeyCodes.Action1);
        }

        if (
            Input.GetButtonDown(Const_KeyCodes.Skip)
            && game.state == "cut-scene"
        )
        {
            game.HandleContinuingDialogueActions(Const_KeyCodes.Skip);
        }
    }

    protected virtual void OnDisable() {}
    public virtual void EatDemon(int Id) {}
    public virtual void SetSwitchState(int Id, bool isOn) {}
    public virtual void HandleMovingNPCCurrentMovesDone() {}
    public virtual void HandleMovingNPCAllMovesDone() {}
    public virtual void HandleDDRArrowClick(int t) {}
    public virtual void OnDoorLockUnlock(int id) {}
    public virtual void OnCloseInventory() {}
    public virtual void HandleExitCutScene() {}
    public virtual void HandleMovingNPCOnApproachedTarget(int i) {}
    public virtual void HandleDialogueNodeAction(string a) {}
    public virtual void HandleDialogueNodeUpdateAction(string a) {}
    public virtual void ActivateTrigger(string Id) {}
    public virtual void ActivateObjectTrigger(string Id, Collider col) {}
    public virtual void Cleanup(){}
    public virtual void Setup()
    {
        // game.CreateNPCs();
        // game.CreateInteractableObjects();
        // game.CreateDemons();
    }
}
