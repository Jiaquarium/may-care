using System.Collections;
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
    public Animator animator;


    private Script_PlayerAction playerActionHandler;
    private Script_PlayerThoughtManager playerThoughtManager;
    private Script_PlayerMovement playerMovementHandler;
    private Script_PlayerDemonActions playerDemonActions;
    
    
    public float glitchDuration;
    public string facingDirection;
    public Vector3 location;
    public Vector3 startLocation;


    private Sprite currentSprite;
    // storing soundFX here and not in manager because only 1 player exists
    private Script_Game game;
    private Tilemap exitsTileMap;
    private Tilemap entrancesTileMap;
    private Tilemap tileMap;
    private bool isTalking = false;
    private bool isEating = false;
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

        playerMovementHandler.TrackPlayerGhost();

        if (game.state != "interact")
        {
            // playerMovementHandler.FinishMoveAnimation();
            animator.SetBool("PlayerMoving", false);   
            return;
        }

        // once we know we move onto an exit space
        // begin fading the screen out
        playerMovementHandler.HandleExitTile();

        if (isEating)
        {
            animator.SetBool("PlayerMoving", false);
            return;
        }

        bool isInventoryOpen = game.GetIsInventoryOpen();

        if (isInventoryOpen)
        {
            playerActionHandler.HandleInventoryActionsInput();
            return;
        }

        playerActionHandler.HandleActionInput(facingDirection, location);
        
        if (isTalking)
        {
            animator.SetBool("PlayerMoving", false);
            return;
        }        
        
        playerMovementHandler.HandleMoveInput();
    }

    public void EatDemon()
    {
        playerDemonActions.EatDemon();
    }

    public void EatHeart()
    {
        playerDemonActions.EatHeart();
    }

    public void SetIsTalking()
    {
        isTalking = true;
        animator.SetBool("PlayerMoving", false);
    }

    public void SetIsEating()
    {
        isEating = true;
        animator.SetBool("PlayerMoving", false);
    }

    public void SetIsNotEating()
    {
        isEating = false;
    }

    public void SetIsNotTalking()
    {
        isTalking = false;
    }

    public bool GetIsTalking()
    {
        return isTalking;
    }

    public bool GetIsEating()
    {
        return isEating;
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
        Tilemap _entrancesTileMap,
        string direction,
        Model_PlayerState playerState,
        bool isLightOn
    )
    {   
        game = Object.FindObjectOfType<Script_Game>();
        animator = GetComponent<Animator>();
        tileMap = _tileMap;
        exitsTileMap = _exitsTileMap;
        entrancesTileMap = _entrancesTileMap;
        
        playerMovementHandler = GetComponent<Script_PlayerMovement>();
        playerActionHandler = GetComponent<Script_PlayerAction>();
        playerThoughtManager = GetComponent<Script_PlayerThoughtManager>();
        playerDemonActions = GetComponent<Script_PlayerDemonActions>();
        playerMovementHandler.Setup(
            game,
            Directions,
            tileMap,
            exitsTileMap,
            entrancesTileMap,
            isLightOn
        );
        playerActionHandler.Setup(game, Directions);
        playerThoughtManager.Setup();
        playerDemonActions.Setup(game);
        
        location = transform.position;
        currentSprite = GetComponent<SpriteRenderer>().sprite;

        AdjustRotation();
        FaceDirection(direction);
    }
}
