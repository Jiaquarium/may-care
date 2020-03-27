using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_MovingNPC : Script_StaticNPC
{
    public int MovingNPCId;
    public Vector3 startLocation;
    public Vector3 location;
    public float speed;
    public float progress;
    public string localState;
    public bool shouldExit = true;
    
    
    private bool inProgress = false;

    public AnimationCurve progressCurve;
    private Animator animator;
    
    private Dictionary<string, Vector3> Directions = new Dictionary<string, Vector3>()
    {
        {"up"       , new Vector3(0f, 0f, 1f)},
        {"down"     , new Vector3(0f, 0f, -1f)},
        {"left"     , new Vector3(-1f, 0f, 0f)},
        {"right"    , new Vector3(1f, 0f, 0f)}
    };

    public Model_MoveSet[] moveSets = new Model_MoveSet[0];

    private Queue<string> currentMoves = new Queue<string>();
    public Queue<string[]> allMoves = new Queue<string[]>();
    
    // Start is called before the first frame update
    void Start()
    {
        // first queue up moves
        QueueUpAllMoves();
        QueueUpCurrentMoves();
        
        // Move(Directions[])
        if (game.state == "cut-scene_npc-moving")
        {
            Move();
        }
    }

    // Update is called once per frame
    void Update()
    {   
        if (game.state == "cut-scene_npc-moving" && localState == "move")
        {
            ActuallyMove();
        }
        if (game.state == "cut-scene_npc-moving" && inProgress)
        {
            ActuallyMove();
        }
    }

    void QueueUpAllMoves()
    {
        foreach(Model_MoveSet moveSet in moveSets)
        {
            allMoves.Enqueue(moveSet.moves);
        }
    }

    void QueueUpCurrentMoves()
    {
        string[] moves = allMoves.Dequeue();
        currentMoves.Clear();

        foreach(string move in moves)
        {
            currentMoves.Enqueue(move);
        }
    }

    public override void Move()
    {
        Vector3 desiredDirection = Directions[currentMoves.Dequeue()];

        startLocation = location;
        location += desiredDirection;
        localState = "move";

        progress = 0f;

        AnimatorSetDirection(desiredDirection.x, desiredDirection.z);
        animator.SetBool("NPCMoving", true);
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
            progress = 1f;
            transform.position = location;
            
            if (currentMoves.Count == 0) {
                localState = "interact";
                animator.SetBool("NPCMoving", false);

                if (allMoves.Count == 0)
                {
                    game.UnPauseBgMusic();
                    game.ChangeStateInteract();
                    inProgress = false;
                    /*
                        currently moveSets must match dialogues.Length to Exit
                    */    
                    if (shouldExit)
                    {
                        Exit();
                    }
                    
                    return;
                }
                inProgress = true;

                game.ChangeStateInteract();
                QueueUpCurrentMoves();
                
                return;
            }
            
            Move();
        }
    }

    void Exit()
    {
        game.DestroyMovingNPC(0);
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
        if (direction == "down")        AnimatorSetDirection(0  , -1f);
        else if (direction == "up")     AnimatorSetDirection(0  ,  1f);
        else if (direction == "left")   AnimatorSetDirection(-1f,  0f);
        else if (direction == "right")  AnimatorSetDirection(1f ,  0f );
    }

    public void ChangeSpeed(float _speed)
    {
        speed = _speed;
    }

    public override void Setup(
        Sprite sprite,
        Model_Dialogue dialogue,
        Model_MoveSet[] _moveSets
    )
    {
        moveSets = _moveSets;
        
        // call Setup from base layer (StaticNPC)
        base.Setup(sprite, dialogue, new Model_MoveSet[0]);

        animator = GetComponent<Animator>();
        animator.SetBool("NPCMoving", false);

        progress = 1f;
        location = transform.position;
    }
}
