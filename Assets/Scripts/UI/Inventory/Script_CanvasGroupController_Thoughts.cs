using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_CanvasGroupController_Thoughts : Script_CanvasGroupController
{
    public Script_Game game;
    public GameObject thoughts;
    public GameObject emptyThoughts;
    
    public override void Open()
    {
        HandleThoughtsState();
        base.Open();
    }

    void HandleThoughtsState()
    {
        if (game.GetThoughtsCount() > 0)
        {
            thoughts.SetActive(true);
            emptyThoughts.SetActive(false);
        }
        else
        {
            thoughts.SetActive(false);
            emptyThoughts.SetActive(true);
        }
    }

    public override void Setup()
    {
        // TODO: we can update this in the game when we get a thought
        // there is still a bit of lag on first open
        HandleThoughtsState();
    }
}
