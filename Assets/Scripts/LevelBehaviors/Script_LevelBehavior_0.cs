using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_LevelBehavior_0 : Script_LevelBehavior
{
    public Script_ProximityFader fader;
    [SerializeField]
    private SpriteRenderer fadingPlayer;
    [SerializeField]
    private SpriteRenderer playerGhostToFade;
    private Script_PlayerMovementAnimator playerMovementAnimator;
    private Script_PlayerGhost playerGhost;

    public override void Setup()
    {
        playerMovementAnimator = game.GetPlayerMovementAnimator();
        playerGhost = game.GetPlayerGhost();
        fadingPlayer = playerMovementAnimator.GetComponent<SpriteRenderer>();
        playerGhostToFade = playerGhost.GetComponent<SpriteRenderer>();

        SpriteRenderer[] toFade = new SpriteRenderer[]{
            fadingPlayer,
            playerGhostToFade
        };

        fader.Setup(fadingPlayer, toFade);
    }
}
