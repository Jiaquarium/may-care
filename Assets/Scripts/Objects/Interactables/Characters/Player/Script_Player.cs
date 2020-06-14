using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Script_PlayerStats))]
public class Script_Player : Script_Character
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
    private Script_PlayerEffect playerEffect;
    private Script_PlayerMovementAnimator playerMovementAnimator;
    private Script_InteractionBoxController interactionBoxController;
    private Script_PlayerStats playerStats;
    
    
    public float glitchDuration;
    public string facingDirection;
    public Vector3 location;
    public Vector3 startLocation;


    [SerializeField]
    private string state;
    [SerializeField]
    private string lastState;
    // storing soundFX here and not in manager because only 1 player exists
    private Script_Game game;
    private Script_PlayerReflection reflection;
    private bool isPlayerGhostMatchSortingLayer = false;
    private const string PlayerGlitch = "Base Layer.Player_Glitch";
    private Dictionary<string, Vector3> Directions;

    // Update is called once per frame
    void Update()
    {   
        /* ========================================================
            visuals
        ======================================================== */
        if (isPlayerGhostMatchSortingLayer)
        {
            playerMovementHandler.PlayerGhostSortOrder(
                Script_Utils.FindComponentInChildWithTag<SpriteRenderer>(
                    this.gameObject, Const_Tags.PlayerAnimator
                ).sortingOrder
            );
        }
        playerMovementHandler.TrackPlayerGhost();
        /* ======================================================== */

        if (game.state == "ddr")
        {
            animator.SetBool("PlayerMoving", false);
            playerMovementHandler.HandleMoveInput();
        }
        
        if (game.state == "interact")
        {
            // playerMovementHandler.FinishMoveAnimation();
            animator.SetBool("PlayerMoving", false);
            playerActionHandler.HandleActionInput(facingDirection, location);
            if (GetIsAttacking() || GetIsTalking()) {
                animator.SetBool("PlayerMoving", false);
            }
            else
            {
                // once we know we move onto an exit space
                // begin fading the screen out
                playerMovementHandler.HandleExitTile();
                playerMovementHandler.HandleMoveInput();
            }
        }
    }
    
    public void RemoveReflection()
    {
        playerMovementHandler.RemoveReflection();
    }

    public void SetState(string s)
    {
        lastState = state;
        state = s;
    }

    public void SetLastState()
    {
        SetState(lastState);
    }
    
    public void SetIsTalking()
    {
        SetState(Const_States_Player.Dialogue);
        animator.SetBool("PlayerMoving", false);
    }

    public void SetIsInteract()
    {
        SetState(Const_States_Player.Interact);
    }

    public void SetIsInventory()
    {
        SetState(Const_States_Player.Inventory);
    }

    public void SetIsAttacking()
    {
        SetState(Const_States_Player.Attack);
        animator.SetBool("PlayerMoving", false);
    }

    public bool GetIsTalking()
    {
        return state == Const_States_Player.Dialogue;
    }

    public bool GetIsAttacking()
    {
        return state == Const_States_Player.Attack;
    }

    public bool GetIsInteract()
    {
        return state == Const_States_Player.Interact;
    }

    public bool GetIsInventory()
    {
        return state == Const_States_Player.Inventory;
    }

    public void AnimatorSetDirection(string dir)
    {
        interactionBoxController.HandleActiveInteractionBox(dir);
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

    public void CreatePlayerReflection(Vector3 axis)
    {
        playerMovementHandler.CreatePlayerReflection(axis);
    }

    public void SetPlayerReflection(Script_PlayerReflection pr)
    {
        playerMovementHandler.SetPlayerReflection(pr);
    }

    public void FaceDirection(string direction)
    {
        AnimatorSetDirection(direction);
    }

    public void QuestionMark(bool isShow)
    {
        playerEffect.QuestionMark(isShow);
    }

    public void ForceSortingLayer(bool isForceSortingOrder, bool isAxisZ)
    {
        if (isForceSortingOrder)
        {
            Script_Utils.FindComponentInChildWithTag<Script_SortingOrder>(
                this.gameObject,
                Const_Tags.PlayerAnimator
            ).EnableWithOffset(
                Script_Graphics.playerSortOrderOffset,
                isAxisZ
            );
        }
        else
        {
            Script_Utils.FindComponentInChildWithTag<Script_SortingOrder>(
                this.gameObject,
                Const_Tags.PlayerAnimator
            ).DefaultSortingOrder();
        }
        PlayerGhostMatchSortingLayer();
    }

    public void PlayerGhostMatchSortingLayer()
    {
        isPlayerGhostMatchSortingLayer = true;
    }

    public Script_PlayerGhost GetPlayerGhost()
    {
        return playerMovementHandler.GetPlayerGhost();
    }

    public Script_PlayerMovementAnimator GetPlayerMovementAnimator()
    {
        return playerMovementAnimator;
    }

    public void SwitchLight(bool isOn)
    {
        playerMovementHandler.SwitchLight(isOn);
    }

    public void TryPushPushable(string dir)
    {
        playerActionHandler.TryPushPushable(dir);
    }

    public void InitializeOnLevel(
        Model_PlayerState playerState,
        bool isLightOn,
        Transform grid
    )
    {
        Vector3 spawnLocation = new Vector3(
            playerState.spawnX ?? 0f,
            playerState.spawnY ?? 0f,
            playerState.spawnZ ?? 0f
        );

        Vector3 adjustedSpawnLocation = new Vector3(
            spawnLocation.x + grid.position.x,
            spawnLocation.y, // do not adjust y value
            spawnLocation.z + grid.position.z
        );

        transform.position = adjustedSpawnLocation;
        location = transform.position;
        FaceDirection(playerState.faceDirection);
        playerMovementHandler.InitializeOnLevel(grid);
        SwitchLight(isLightOn);
    }
    
    public void Setup(
        string direction,
        Model_PlayerState playerState,
        bool isLightOn
    )
    {   
        game = Object.FindObjectOfType<Script_Game>();
        // animator = GetComponent<Animator>();
        animator = Script_Utils.FindComponentInChildWithTag<Animator>(
            this.gameObject,
            Const_Tags.PlayerAnimator
        );
        Directions = Script_Utils.GetDirectionToVectorDict();
        
        playerMovementHandler = GetComponent<Script_PlayerMovement>();
        playerActionHandler = GetComponent<Script_PlayerAction>();
        playerThoughtManager = GetComponent<Script_PlayerThoughtManager>();
        playerEffect = GetComponent<Script_PlayerEffect>();
        playerMovementAnimator = Script_Utils.FindChildWithTag(
            this.gameObject, Const_Tags.PlayerAnimator)
            .GetComponent<Script_PlayerMovementAnimator>();
        interactionBoxController = GetComponent<Script_InteractionBoxController>();
        playerStats = GetComponent<Script_PlayerStats>();

        playerMovementHandler.Setup(game, isLightOn);
        playerActionHandler.Setup(game);
        playerThoughtManager.Setup();
        playerEffect.Setup();
        
        /// Setup character stats
        base.Setup();
        
        location = transform.position;
        SetState(Const_States_Player.Interact);
        FaceDirection(direction);
    }
}
