using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_DontTouchPianoScene : MonoBehaviour
{
    public Script_Game game;
    public Vector3[] triggerLocations;
    public Model_Dialogue dialogue;
    private bool isDone = false;
    
    // Start is called before the first frame update
    void Start()
    {
        game.exitsHandler.DisableExits();
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < triggerLocations.Length; i++)
        {
            if (
                game.GetPlayerLocation() == triggerLocations[i]
                && game.state == "interact"
                && !isDone
            )
            {
                game.PauseBgMusic();
                game.PlayEroTheme();
                game.ChangeStateCutScene();
                
                game.PlayerFaceDirection("down");
                game.ChangeCameraTargetToNPC(0);
                game.StartDialogue(dialogue);
            }
        }

        if (game.state == "cut-scene" && !game.GetPlayerIsTalking())
        {
            isDone = true;

            game.exitsHandler.EnableExits();
            game.ChangeStateCutSceneNPCMoving();
            game.TriggerMovingNPCMove(0);

            // ero then leaves through door
        }

        if (Input.GetButtonDown("Action1") && game.state == "cut-scene" && !isDone)
        {
            game.HandleContinuingDialogueActions("Action1");
        }

        if (Input.GetButtonDown("Submit") && game.state == "cut-scene" && !isDone)
            game.HandleContinuingDialogueActions("Submit");
    }

    void OnDisable()
    {
        print("enabling Exits");
        game.exitsHandler.EnableExits();        
    }
}
