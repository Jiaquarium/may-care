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
    void Update()
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
    protected virtual void HandleDialogueAction()
    {
        if (Input.GetButtonDown("Action1") && game.state == "cut-scene")
        {
            game.HandleContinuingDialogueActions("Action1");
        }

        if (Input.GetButtonDown("Submit") && game.state == "cut-scene")
        {
            game.HandleContinuingDialogueActions("Submit");
        }
    }

    protected virtual void OnDisable() {}
    public virtual void EatDemon(int Id) {}
    public virtual void SetSwitchState(int Id, bool isOn) {}

    // called from Script_Exits() to change state after fade in
    public virtual void InitGameState()
    {
        game.ChangeStateInteract();
    }

    public virtual void Setup()
    {
        print("setting up levelBehavior");
        game.EnableExits();
        // game.CreateNPCs();
        // game.CreateInteractableObjects();
        // game.CreateDemons();
    }
}
