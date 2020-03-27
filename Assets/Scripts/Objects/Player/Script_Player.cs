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
    public AnimationCurve progressCurve;
    private Script_PlayerAction playerActionHandler;
    private Script_PlayerThoughtManager playerThoughtManager;
    
    
    public float glitchDuration;
    public float speed;


    private Sprite currentSprite;
    // storing soundFX here and not in manager because only 1 player exists
    private Vector3 startLocation;
    private Vector3 location;
    private Script_Game game;
    private Tilemap tileMap;
    private Tilemap exitsTileMap;
    private bool isTalking = false;
    private float progress;
    private string facingDirection;
    private string localState = "interact";
    private Vector3[] MovingNPCLocations = new Vector3[0];
    private Vector3[] DemonLocations = new Vector3[0];
    private Animator animator;
    private const string PlayerGlitch = "Base Layer.Player_Glitch";
    private Dictionary<string, Vector3> Directions = new Dictionary<string, Vector3>()
    {
        {"up"       , new Vector3(0f, 0f, 1f)},
        {"down"     , new Vector3(0f, 0f, -1f)},
        {"left"     , new Vector3(-1f, 0f, 0f)},
        {"right"    , new Vector3(1f, 0f, 0f)}
    };

    // Start is called before the first frame update
    void Start()
    {
        
    }

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

            if (progress == 1f) HandleMoveInput();
        }
     
        if(localState == "move")	ActuallyMove();
    }

    void HandleMoveInput()
    {
        if(Input.GetAxis("Vertical") > 0f)
        {
            facingDirection = "up";
            
            Move(Directions[facingDirection]);
            AnimatorSetDirection(0f, 1f);
        }
        else if(Input.GetAxis("Vertical") < 0f)
        {
            facingDirection = "down";
            
            Move(Directions[facingDirection]);
            AnimatorSetDirection(0f, -1f);
        }
        else if(Input.GetAxis("Horizontal") > 0f)
        {
            facingDirection = "right";

            Move(Directions[facingDirection]);
            AnimatorSetDirection(1f, 0f);
        }
        else if(Input.GetAxis("Horizontal") < 0f)
        {
            facingDirection = "left";
            
            Move(Directions[facingDirection]);
            AnimatorSetDirection(-1f, 0f);
        }
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

    void AnimatorSetDirection(float x, float z)
    {
        animator.SetFloat("LastMoveX", x);
        animator.SetFloat("LastMoveZ", z);
        animator.SetFloat("MoveX", x);
        animator.SetFloat("MoveZ", z);
    }

    public void FaceDirection(string direction)
    {
        facingDirection = direction;

        if (direction == "down")        AnimatorSetDirection(0  , -1f);
        else if (direction == "up")     AnimatorSetDirection(0  ,  1f);
        else if (direction == "left")   AnimatorSetDirection(-1f,  0f);
        else if (direction == "right")  AnimatorSetDirection(1f ,  0f );
    }

    void Move(Vector3 desiredDirection)
    {
        int desiredX = (int)Mathf.Round((location + desiredDirection).x);
        int desiredZ = (int)Mathf.Round((location + desiredDirection).z);
        
        // tiles map from (xyz) to (xz)
        if (
            !tileMap.HasTile(new Vector3Int(desiredX, desiredZ, 0))
            && !exitsTileMap.HasTile(new Vector3Int(desiredX, desiredZ, 0))
        ) 
        {
            return;
        }

        // if NPC is moving check if NPC is occupying space          
        // don't check nonmoving NPCs b/c we do that in tileMap and they're static
        MovingNPCLocations = game.GetMovingNPCLocations();
        if (MovingNPCLocations.Length != 0)
        {
            foreach (Vector3 loc in MovingNPCLocations)
            {
                if (desiredX == loc.x && desiredZ == loc.z) return;    
            }
        }

        // if Demons on map check if occupying space
        DemonLocations = game.GetDemonLocations();
        if (DemonLocations.Length != 0)
        {
            foreach (Vector3 loc in DemonLocations)
            {
                if (desiredX == loc.x && desiredZ == loc.z) return;
            }
        }

        startLocation = location;
        location += desiredDirection;
        
        // actually begin to move
        localState = "move";
        progress = 0f;
    }

    void ActuallyMove()
    {
        progress += speed;
        transform.position = Vector3.Lerp(
            startLocation,
            location,
            progressCurve.Evaluate(progress)
        );

        if (progress >= 1f)
        {
            localState = "interact";
            progress = 1f;
            transform.position = location;

            if (exitsTileMap.HasTile(
                new Vector3Int(
                    (int)Mathf.Round(location.x),
                    (int)Mathf.Round(location.z),
                    0
                )
            ))
            {
                game.HandleLevelExit();
            }
        }
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
        
        playerActionHandler = GetComponent<Script_PlayerAction>();
        playerThoughtManager = GetComponent<Script_PlayerThoughtManager>();
        playerActionHandler.Setup(game, Directions);
        playerThoughtManager.Setup();

        tileMap = _tileMap;
        exitsTileMap = _exitsTileMap;

        animator = GetComponent<Animator>();
        
        progress = 1f;
        location = transform.position;
        currentSprite = GetComponent<SpriteRenderer>().sprite;

        AdjustRotation();
        FaceDirection(direction);
    }
}
