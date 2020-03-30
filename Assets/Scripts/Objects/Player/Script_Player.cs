﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Script_Player : MonoBehaviour
{
    /*
        persistent data, start
    */
    /*
        persistent data, end
    */
    private Script_PlayerAction playerActionHandler;
    private Script_PlayerThoughtManager playerThoughtManager;
    private Script_PlayerMovement playerMovementHandler;
    
    
    public float glitchDuration;
    public string facingDirection;
    public Vector3 location;
    public Vector3 startLocation;
    public string localState = "interact";


    private Sprite currentSprite;
    // storing soundFX here and not in manager because only 1 player exists
    private Script_Game game;
    private Tilemap exitsTileMap;
    private Tilemap tileMap;
    private bool isTalking = false;
    private Animator animator;
    private const string PlayerGlitch = "Base Layer.Player_Glitch";
    private Dictionary<string, Vector3> Directions = new Dictionary<string, Vector3>()
    {
        {"up"       , new Vector3(0f, 0f, 1f)},
        {"down"     , new Vector3(0f, 0f, -1f)},
        {"left"     , new Vector3(-1f, 0f, 0f)},
        {"right"    , new Vector3(1f, 0f, 0f)}
    };

    // Update is called once per frame
    void Update()
    {   
        AdjustRotation();

        if (game.state == "cut-scene")              return;
        if (game.state == "cut-scene_npc-moving")   return;
        
        bool isInventoryOpen = game.GetIsInventoryOpen();

        if (isInventoryOpen)
        {
            playerActionHandler.HandleInventoryActionsInput();
            return;
        }
        
        playerActionHandler.HandleActionInput(facingDirection, location);
        
        if (!isTalking)
        {
            // move animation when direction button down 
            animator.SetBool(
                "PlayerMoving",
                Input.GetAxis("Vertical") != 0f || Input.GetAxis("Horizontal") != 0f
            );

            playerMovementHandler.HandleMoveInput();
        }
     
        // if(localState == "move")	playerMovementHandler.ActuallyMove();
    }

    public void SetIsTalking()
    {
        isTalking = true;
        animator.SetBool("PlayerMoving", false);
    }

    public void SetIsNotTalking()
    {
        isTalking = false;
    }

    public bool GetIsTalking()
    {
        return isTalking;
    }

    public void AnimatorSetDirection(string dir)
    {
        facingDirection = dir;

        if (dir == "up")
        {
            animator.SetFloat("LastMoveX", 0f);
            animator.SetFloat("LastMoveZ", 1f);
            animator.SetFloat("MoveX", 0f);
            animator.SetFloat("MoveZ", 1f);
        }
        else if (dir == "down")
        {
            animator.SetFloat("LastMoveX", 0f);
            animator.SetFloat("LastMoveZ", -1f);
            animator.SetFloat("MoveX", 0f);
            animator.SetFloat("MoveZ", -1f);
        }
        else if (dir == "left")
        {
            animator.SetFloat("LastMoveX", -1f);
            animator.SetFloat("LastMoveZ", 0f);
            animator.SetFloat("MoveX", -1f);
            animator.SetFloat("MoveZ", 0f);
        }
        else if (dir == "right")
        {
            animator.SetFloat("LastMoveX", 1f);
            animator.SetFloat("LastMoveZ", 0f);
            animator.SetFloat("MoveX", 1f);
            animator.SetFloat("MoveZ", 0f);
        }
    }

    public void FaceDirection(string direction)
    {
        AnimatorSetDirection(direction);
    }

    public void AdjustRotation()
    {
        transform.forward = Camera.main.transform.forward;
    }

    // public IEnumerator glitch()
    // {
    //     if (animator != null) {
    //         animator.enabled = true;
    //         animator.Play(PlayerGlitch);
            
    //         yield return new WaitForSeconds(glitchDuration);

    //         animator.enabled = false;
    //         Sprite sprite = GetComponent<SpriteRenderer>().sprite;
    //         if (
    //             sprite != playerUpSprite
    //             && sprite != playerDownSprite
    //             && sprite != playerLeftSprite
    //             && sprite != playerRightSprite
    //         ) {
    //             GetComponent<SpriteRenderer>().sprite = currentSprite;
    //         }
    //     }
    // }
    
    public void Setup(
        Tilemap _tileMap,
        Tilemap _exitsTileMap,
        string direction,
        Model_PlayerState playerState
    )
    {   
        game = Object.FindObjectOfType<Script_Game>();
        tileMap = _tileMap;
        exitsTileMap = _exitsTileMap;
        
        playerMovementHandler = GetComponent<Script_PlayerMovement>();
        playerActionHandler = GetComponent<Script_PlayerAction>();
        playerThoughtManager = GetComponent<Script_PlayerThoughtManager>();
        playerMovementHandler.Setup(
            game,
            Directions,
            tileMap,
            exitsTileMap
        );
        playerActionHandler.Setup(game, Directions);
        playerThoughtManager.Setup();


        animator = GetComponent<Animator>();
        
        location = transform.position;
        currentSprite = GetComponent<SpriteRenderer>().sprite;

        AdjustRotation();
        FaceDirection(direction);
    }
}
