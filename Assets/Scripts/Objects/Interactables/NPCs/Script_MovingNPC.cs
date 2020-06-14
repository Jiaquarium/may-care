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
    public int moveSetIndex;


    public Model_MoveSet[] moveSets = new Model_MoveSet[0];
    // use this to keep reference to model moveSets
    public Script_MovingNPCMoveSets movingNPCMoveSets;
    
    public AnimationCurve progressCurve;
    public Dictionary<string, Vector3> Directions;
    public Queue<string> currentMoves = new Queue<string>();
    public Queue<string[]> allMoves = new Queue<string[]>();


    private Animator animator;
    [SerializeField] private bool isApproachingTarget; // bool used to know if current moves are result of appraoching
    [SerializeField] private string lastFacingDirection;    
    
    void OnEnable() {
        if (string.IsNullOrEmpty(lastFacingDirection) || animator == null)  return;
        FaceDirection(lastFacingDirection);
    }
    
    // Update is called once per frame
    protected virtual void Update()
    {   
        if (game.state == "cut-scene_npc-moving" && localState == "move")
        {
            ActuallyMove();
        }
        // else if (game.state == "cut-scene_npc-moving" && inProgress)
        // {
        //     ActuallyMove();
        // }
    }

    public override void TriggerDialogue()
    {
        Vector3 playerLoc = game.GetPlayerLocation();
        Vector3 diff = location - playerLoc;
        
        if (diff.x < 0)         FaceDirection("right");
        else if (diff.x > 0)    FaceDirection("left");
        else if (diff.z < 0)    FaceDirection("up");
        else if (diff.z > 0)    FaceDirection("down");

        base.TriggerDialogue();
    }

    void QueueUpAllMoves()
    {
        allMoves.Clear();
        
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

    public void QueueMoves()
    {
        QueueUpAllMoves();
        QueueUpCurrentMoves();
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

    public void ActuallyMove()
    {
        progress += speed * Time.deltaTime;
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
                
                string endCommand = moveSets[moveSetIndex].endFaceDirection;
                FaceDirection(endCommand);
                
                game.ChangeStateInteract();
                game.CurrentMovesDoneAction();
                

                if (allMoves.Count == 0)
                {
                    game.ChangeStateInteract();
                    game.AllMovesDoneAction(MovingNPCId);

                    if (isApproachingTarget)
                    {
                        isApproachingTarget = false;
                        // allow LB to handle this event
                        game.OnApproachedTarget(MovingNPCId);
                    }

                    if (endCommand == Const_Commands_MovingNPC.Exit)
                    {
                        Exit();
                    }
                    
                    return;
                }
                // TODO: this might need to happen before calling current moves done
                moveSetIndex++;
                QueueUpCurrentMoves();
                
                return;
            }
            Move();
        }
    }

    void Exit()
    {
        game.DestroyMovingNPC(MovingNPCId);
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

        lastFacingDirection = direction;
    }

    public void ChangeSpeed(float _speed)
    {
        speed = _speed;
    }

    public void ForceMove(Model_MoveSet _moveSet)
    {
        moveSets = new Model_MoveSet[]{ _moveSet };
        moveSetIndex = 0;
        
        QueueUpAllMoves();
        QueueUpCurrentMoves();

        if (_moveSet.moves.Length == 0)
        {
            // also face direction
            FaceDirection(_moveSet.endFaceDirection); 
            game.OnApproachedTarget(MovingNPCId);
        }
        else
        {
            isApproachingTarget = true;
            Move();
        }
    }

    public void ApproachTarget(
        Vector3 target,
        Vector3 adjustment,
        string endFaceDirection
    )
    {
        Vector3 toMove = (target + adjustment) - location;
        string xMove;
        string zMove;
        string[] moves;

        int x = (int)Mathf.Round(toMove.x);
        int z = (int)Mathf.Round(toMove.z);
        int absX = Mathf.Abs(x);
        int absZ = Mathf.Abs(z);

        if (x < 0)  xMove = "left";
        else        xMove = "right";

        if (z < 0)  zMove = "down";
        else        zMove = "up";
        
        moves = new string[absX + absZ];

        for (int i = 0; i < moves.Length; i++)
        {
            if (i < absX)   moves[i] = xMove;
            else            moves[i] = zMove;   
        }

        ForceMove(new Model_MoveSet(
            moves,
            endFaceDirection
        ));
    }

    public virtual void SetMoveSpeedRun(){}
    public virtual void SetMoveSpeedWalk(){}

    public override void Setup(
        Model_Dialogue dialogue,
        Script_DialogueNode[] dialogueNodes,
        Model_MoveSet[] _moveSets
    )
    {
        Directions = Script_Utils.GetDirectionToVectorDict();
        moveSets = _moveSets;
        
        // call Setup from base layer (StaticNPC)
        base.Setup(dialogue, dialogueNodes, new Model_MoveSet[0]);

        animator = rendererChild.GetComponent<Animator>();
        animator.SetBool("NPCMoving", false);
        if (!string.IsNullOrEmpty(lastFacingDirection))
        {
            FaceDirection(lastFacingDirection);
        }

        progress = 1f;
        location = transform.position;

        // first queue up moves
        if (moveSets.Length != 0)
        {
            QueueUpAllMoves();
            QueueUpCurrentMoves();
        }
    }
}
