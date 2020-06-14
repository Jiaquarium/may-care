using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Script_StaticNPC : Script_Interactable
{
    public int StaticNPCId;
    public Script_Game game;
    public Model_Dialogue dialogue;
    public Script_DialogueNode[] dialogueNodes;
    [SerializeField] protected Transform rendererChild;

    private int dialogueIndex;
    private Script_DialogueManager dialogueManager;
    private bool isMute;
    private Coroutine fadeOutCo;

    // Update is called once per frame
    void Update()
    {
        AdjustRotation();
    }

    public virtual void TriggerDialogue()
    {
        if (isMute)   return;
        
        dialogueManager.StartDialogueNode(dialogueNodes[dialogueIndex]);
        HandleDialogueNodeIndex();
    }

    public void SetMute(bool _isMute)
    {
        isMute = _isMute;
    }

    public void SetVisibility(bool isVisible)
    {
        rendererChild.GetComponent<SpriteRenderer>().enabled = isVisible;
    }

    Action OnFadeOut()
    {
        return new Action(() => {
            Destroy(this.gameObject);
        });
    }

    public virtual void FadeOut(Action cb)
    {
        // if no callback passed in, uses default OnFadeOut()
        fadeOutCo = StartCoroutine(
            rendererChild.GetComponent<Script_SpriteFadeOut>().FadeOutCo(
                cb == null ? OnFadeOut() : () => cb()
            )
        );
    }

    void HandleDialogueNodeIndex()
    {
        if (dialogueIndex == dialogueNodes.Length - 1)
        {
            dialogueIndex = 0;    
        }
        else
        {
            dialogueIndex++;
        }
    }

    public void ContinueDialogue()
    {
        dialogueManager.DisplayNextDialoguePortion();
    }

    public void SkipTypingSentence()
    {
        dialogueManager.SkipTypingSentence();
    }

    public void AdjustRotation()
    {
        rendererChild.transform.forward = Camera.main.transform.forward;
    }

    public virtual void Move() {}
    public virtual void Glimmer() {}
    public virtual void Freeze(bool isFrozen) {}

    public virtual void Setup(
        Model_Dialogue _dialogue,
        Script_DialogueNode[] _dialogueNodes,
        Model_MoveSet[] _moveSets
    )
    {
        game = FindObjectOfType<Script_Game>();
        dialogueManager = FindObjectOfType<Script_DialogueManager>();
        dialogue = _dialogue;
        dialogueNodes = _dialogueNodes;

        AdjustRotation();
    }
}
