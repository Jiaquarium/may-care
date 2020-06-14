using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Script_DDRManager : MonoBehaviour
{
    public Script_Game game;
    public CanvasGroup DDRCanvasGroup;
    public Script_Arrow ArrowPrefabLeft;
    public Script_Arrow ArrowPrefabDown;
    public Script_Arrow ArrowPrefab;
    public Script_Arrow ArrowPrefabRight;
    public Transform ArrowOutlineLeft;
    public Transform ArrowOutlineDown;
    public Transform ArrowOutline;
    public Transform ArrowOutlineRight;
    public Transform ArrowsContainer;
    public Script_TierComment Tier1Comment;
    public Script_TierComment Tier2Comment;
    public Script_TierComment Tier3Comment;


    public Model_SongMoves songMoves;
    public float tierNeg1Buffer;
    public float tier1Buffer;
    public float tier2Buffer;
    public float tier3Buffer;
    public float focusTimeLength;
    public float tierCommentActivationLength;
    public float outlineLightUpTimeLength;


    public Script_Arrow[] activeLeftArrows;
    public Script_Arrow[] activeDownArrows;
    public Script_Arrow[] activeUpArrows;
    public Script_Arrow[] activeRightArrows;

    public int leftArrowsCounter;
    public int downArrowsCounter;
    public int upArrowsCounter;
    public int rightArrowsCounter;
    public int nextLeftArrowIndex;
    public int nextDownArrowIndex;
    public int nextUpArrowIndex;
    public int nextRightArrowIndex;


    private float timer;
    private bool isTimerOn;
    private int leftMoveCount;
    private int upMoveCount;
    private int downMoveCount;
    private int rightMoveCount;
    private Script_ArrowOutline ScriptArrowOutlineLeft;
    private Script_ArrowOutline ScriptArrowOutlineDown;
    private Script_ArrowOutline ScriptArrowOutlineUp;
    private Script_ArrowOutline ScriptArrowOutlineRight;
    public int mistakes;
    public int mistakesAllowed;


    public float timeToRise;
    public bool didFail;

    /*
        for dev
    */
    public Text timerText;
    
    void Update()
    {
        if (isTimerOn)
        {
            timer += Time.deltaTime;
            timerText.text = timer.ToString();
        }
        HandleLeftArrowSpawn();
        HandleDownArrowSpawn();
        HandleUpArrowSpawn();
        HandleRightArrowSpawn();
        
        HandleAction();

        HandleMistakes();
    }
    
    public void Activate()
    {
        this.gameObject.SetActive(true);
        game.ManagePlayerViews(Const_States_PlayerViews.DDR);

        DDRCanvasGroup.alpha = 1f;
        DDRCanvasGroup.blocksRaycasts = true;
        DDRCanvasGroup.gameObject.SetActive(true);
    }

    public void Deactivate()
    {
        game.StopMovingNPCThemes();
        isTimerOn = false;
        ClearState();

        DDRCanvasGroup.alpha = 1f;
        DDRCanvasGroup.blocksRaycasts = false;
        DDRCanvasGroup.gameObject.SetActive(false);

        this.gameObject.SetActive(false);
    }

    void ClearState()
    {
        DestroyArrows(activeLeftArrows);
        DestroyArrows(activeDownArrows);
        DestroyArrows(activeUpArrows);
        DestroyArrows(activeRightArrows);
        
        activeLeftArrows        = null;
        activeDownArrows        = null;
        activeUpArrows          = null;
        activeRightArrows       = null;

        leftMoveCount           = 0;
        downMoveCount           = 0;
        upMoveCount             = 0;
        rightMoveCount          = 0;
        
        leftArrowsCounter       = 0;
        downArrowsCounter       = 0;
        upArrowsCounter         = 0;
        rightArrowsCounter      = 0;

        nextLeftArrowIndex      = 0;
        nextDownArrowIndex      = 0;
        nextUpArrowIndex        = 0;
        nextRightArrowIndex     = 0;

        mistakes = 0;
    }

    void DestroyArrows(Script_Arrow[] arrows)
    {
        foreach (Script_Arrow a in arrows)
        {
            if (a != null)  Destroy(a.gameObject);
        }
    }

    void StartTimer()
    {
        isTimerOn = true;
    }

    void HandleLeftArrowSpawn()
    {
        if (leftMoveCount >= songMoves.leftMoveTimes.Length)    return;
        float nextLeftMoveTime = songMoves.leftMoveTimes[leftMoveCount];
        if (timer >= nextLeftMoveTime - timeToRise)
        {
            StartLeftArrow();
            leftMoveCount++;
        }
    }
    
    void HandleDownArrowSpawn()
    {
        if (downMoveCount >= songMoves.downMoveTimes.Length)    return;

        float nextDownMoveTime = songMoves.downMoveTimes[downMoveCount];
        if (timer >= nextDownMoveTime - timeToRise)
        {
            StartDownArrow();
            downMoveCount++;
        }
    }

    void HandleUpArrowSpawn()
    {
        if (upMoveCount >= songMoves.upMoveTimes.Length)    return;

        float nextUpMoveTime = songMoves.upMoveTimes[upMoveCount];
        if (timer >= nextUpMoveTime - timeToRise)
        {
            StartUpArrow();
            upMoveCount++;
        }
    }

    void HandleRightArrowSpawn()
    {
        if (rightMoveCount >= songMoves.rightMoveTimes.Length)    return;

        float nextRightMoveTime = songMoves.rightMoveTimes[rightMoveCount];
        if (timer >= nextRightMoveTime - timeToRise)
        {
            StartRightArrow();
            rightMoveCount++;
        }
    }

    void StartLeftArrow()
    {
        Script_Arrow arrow = Instantiate(ArrowPrefabLeft) as Script_Arrow;
        activeLeftArrows[leftArrowsCounter] = arrow;
        arrow.transform.SetParent(ArrowsContainer, false);
        SetupArrow(arrow, ArrowOutlineLeft);
        leftArrowsCounter++;
    }

    void StartDownArrow()
    {
        Script_Arrow arrow = Instantiate(ArrowPrefabDown) as Script_Arrow;
        activeDownArrows[downArrowsCounter] = arrow;
        arrow.transform.SetParent(ArrowsContainer, false);
        SetupArrow(arrow, ArrowOutlineDown);
        downArrowsCounter++;
    }
    
    void StartUpArrow()
    {
        Script_Arrow arrow = Instantiate(ArrowPrefab) as Script_Arrow;
        activeUpArrows[upArrowsCounter] = arrow;
        arrow.transform.SetParent(ArrowsContainer, false);
        SetupArrow(arrow, ArrowOutline);
        upArrowsCounter++;
    }

    void StartRightArrow()
    {
        Script_Arrow arrow = Instantiate(ArrowPrefabRight) as Script_Arrow;
        activeRightArrows[rightArrowsCounter] = arrow;
        arrow.transform.SetParent(ArrowsContainer, false);
        SetupArrow(arrow, ArrowOutlineRight);
        rightArrowsCounter++;
    }

    void SetupArrow(Script_Arrow arrow, Transform arrowOutline)
    {
        arrow.Setup(
            arrowOutline.GetComponent<RectTransform>().localPosition,
            timeToRise,
            tierNeg1Buffer,
            tier1Buffer,
            tier2Buffer,
            tier3Buffer,
            this
        );
        arrow.BeginRising();
    }

    void HandleAction()
    {
        // ask arrow where it is, and manager will post that event
        // for game -> level behavior to handle
        if (
            Input.GetButtonDown(Const_KeyCodes.Left)
        )
        {
            ScriptArrowOutlineLeft.Focus();
            
            if (
                nextLeftArrowIndex < activeLeftArrows.Length
                && activeLeftArrows[nextLeftArrowIndex] != null
                && !activeLeftArrows[nextLeftArrowIndex].isClicked
            )
            {
                int tier = ReportArrowTier(activeLeftArrows[nextLeftArrowIndex]);
                if (tier > 0)
                {
                    if (tier == 1)  ScriptArrowOutlineLeft.LightUp();
                    nextLeftArrowIndex++;
                }
            } 
        }
        if (Input.GetButtonDown(Const_KeyCodes.Down))
        {
            ScriptArrowOutlineDown.Focus();
            if (
                nextDownArrowIndex < activeDownArrows.Length
                && activeDownArrows[nextDownArrowIndex] != null
                && !activeDownArrows[nextDownArrowIndex].isClicked
            )
            {
                int tier = ReportArrowTier(activeDownArrows[nextDownArrowIndex]);
                if (tier > 0)
                {
                    if (tier == 1)  ScriptArrowOutlineDown.LightUp();
                    nextDownArrowIndex++;
                }
            } 
        }
        if (Input.GetButtonDown(Const_KeyCodes.Up))
        {
            ScriptArrowOutlineUp.Focus();
            if (
                nextUpArrowIndex < activeUpArrows.Length
                && activeUpArrows[nextUpArrowIndex] != null
                && !activeUpArrows[nextUpArrowIndex].isClicked
            )
            {
                int tier = ReportArrowTier(activeUpArrows[nextUpArrowIndex]);
                if (tier > 0)
                {
                    if (tier == 1)  ScriptArrowOutlineUp.LightUp();
                    nextUpArrowIndex++;
                }
            } 
        }
        if (Input.GetButtonDown(Const_KeyCodes.Right))
        {
            ScriptArrowOutlineRight.Focus();
            if (
                nextRightArrowIndex < activeRightArrows.Length
                && activeRightArrows[nextRightArrowIndex] != null
                && !activeRightArrows[nextRightArrowIndex].isClicked
            )
            {
                int tier = ReportArrowTier(activeRightArrows[nextRightArrowIndex]);
                if (tier > 0)
                {
                    if (tier == 1)  ScriptArrowOutlineRight.LightUp();
                    nextRightArrowIndex++;
                }
            }
        }
    }

    public int ReportArrowTier(Script_Arrow arrow)
    {
        float t = arrow.ReportTier();
        
        if (t <= tier1Buffer)
        {
            // handle case if it's passed the outline and not tier1
            if (t < 0 && Mathf.Abs(t) > tierNeg1Buffer)
            {
                // report tier3 to game
                arrow.isClicked = true;
                game.HandleDDRArrowClick(3);
                Tier3Comment.Activate();

                mistakes++;

                return 3;
            }
            
            // report tier1 to game
            arrow.isClicked = true;
            arrow.DestroyArrow();
            game.HandleDDRArrowClick(1);
            
            Tier1Comment.Activate();
            
            return 1;
        }
        else if (t <= tier2Buffer)
        {
            // report tier2 to game
            arrow.isClicked = true;
            game.HandleDDRArrowClick(2);
            Tier2Comment.Activate();

            mistakes++;

            return 2;
        }
        else if (t <= tier3Buffer)
        {
            // report tier3 to game
            arrow.isClicked = true;
            game.HandleDDRArrowClick(3);
            Tier3Comment.Activate();
            
            mistakes++;

            return 3;
        }

        return -1;
    }

    void HandleMistakes()
    {
        if (mistakesAllowed != -1 && mistakes >= mistakesAllowed)
        {
            didFail = true;
            Deactivate();
        }
    }

    public void StartMusic(
        Model_SongMoves _songMoves,
        Script_BgThemePlayer bgThemePlayer,
        int _mistakesAllowed // set to -1 to ignore mistakes
    )
    {
        upMoveCount = 0;
        downMoveCount = 0;
        timer = 0;
        mistakes = 0;
        didFail = false;

        songMoves = _songMoves;
        mistakesAllowed = _mistakesAllowed;

        leftArrowsCounter = 0;
        downArrowsCounter = 0;
        upArrowsCounter = 0;
        rightArrowsCounter = 0;
        
        activeLeftArrows            = new Script_Arrow[songMoves.leftMoveTimes.Length];
        activeDownArrows            = new Script_Arrow[songMoves.downMoveTimes.Length];
        activeUpArrows              = new Script_Arrow[songMoves.upMoveTimes.Length];
        activeRightArrows           = new Script_Arrow[songMoves.rightMoveTimes.Length];
        
        ScriptArrowOutlineLeft      = ArrowOutlineLeft.GetComponent<Script_ArrowOutline>();
        ScriptArrowOutlineDown      = ArrowOutlineDown.GetComponent<Script_ArrowOutline>();
        ScriptArrowOutlineUp        = ArrowOutline.GetComponent<Script_ArrowOutline>();
        ScriptArrowOutlineRight     = ArrowOutlineRight.GetComponent<Script_ArrowOutline>();
        
        ScriptArrowOutlineLeft.Setup(focusTimeLength, outlineLightUpTimeLength);
        ScriptArrowOutlineDown.Setup(focusTimeLength, outlineLightUpTimeLength);
        ScriptArrowOutlineUp.Setup(focusTimeLength, outlineLightUpTimeLength);
        ScriptArrowOutlineRight.Setup(focusTimeLength, outlineLightUpTimeLength);

        Tier1Comment.Setup(tierCommentActivationLength);
        Tier2Comment.Setup(tierCommentActivationLength);
        Tier3Comment.Setup(tierCommentActivationLength);

        game.PlayNPCBgTheme(bgThemePlayer);

        StartTimer();
    }
}
