using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_LevelBehavior_12_BioArt : Script_LevelBehavior
{
    public bool isComplete;
    public Script_Exits exitsHandler;
    public Script_DialogueManager dm;
    public Script_DialogueNode symbioticNode;

    public float entranceWaitTime;
    public float kickOutRoomWaitTime;
    public Vector3 level11SpawnPosition;

    private bool kickOutRoom;
    private bool shoutAtPlayer;


    protected override void HandleOnEntrance() {
        if (!exitsHandler.isFadeIn && !shoutAtPlayer)
        {
            shoutAtPlayer = true;
            StartCoroutine(WaitToShoutAtPlayer());
        }
    }

    protected override void HandleTriggerLocations()
    {
        if (
            Script_Utils.CheckLastNodeActionCutScene(game, dm, "kick-out")
            && !kickOutRoom
        )
        {
            kickOutRoom = true;
            StartCoroutine(WaitToKickOutRoom());
        }
    }

    IEnumerator WaitToShoutAtPlayer()
    {
        yield return new WaitForSeconds(entranceWaitTime);

        game.ChangeStateCutScene();
        dm.StartDialogueNode(symbioticNode);
    }

    IEnumerator WaitToKickOutRoom()
    {
        yield return new WaitForSeconds(kickOutRoomWaitTime);

        isComplete = true;
        game.Exit(11, level11SpawnPosition, "down", true);
    }
    
    public override void Setup()
    {
        kickOutRoom = false;
        shoutAtPlayer = false;
    }    
}
