using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_ReflectionCreator : MonoBehaviour
{
    [SerializeField] private Script_Game game;
    private Script_Player player;

    public void SetupPlayerReflection(Transform reflection)
    {
        player = game.GetPlayer();
        Script_PlayerReflection pr = reflection.GetComponent<Script_PlayerReflection>();
        pr.Setup(
            player.GetPlayerGhost(),
            player,
            pr.axis // TODO: remove once we don't have to use CreateReflection
        );
        // pr.transform.SetParent(game.playerContainer, false);
        player.SetPlayerReflection(pr);
    }
}
