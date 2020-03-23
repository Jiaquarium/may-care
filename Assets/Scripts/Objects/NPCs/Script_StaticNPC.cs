using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_StaticNPC : MonoBehaviour
{
    public int StaticNPCId;
    public Script_Game game;
    private Script_DialogueManager dialogueManager;
    public Script_Dialogue dialogue;
    

    // Update is called once per frame
    void Update()
    {
        AdjustRotation();
    }

    public void TriggerDialogue()
    {
        dialogueManager.StartDialogue(dialogue);
    }

    public void ContinueDialogue()
    {
        dialogueManager.DisplayNextSentence();
    }

    public void SkipTypingSentence()
    {
        dialogueManager.SkipTypingSentence();
    }

    public void AdjustRotation()
    {
        transform.forward = Camera.main.transform.forward;
    }

    public virtual void Move() {}

    public virtual void Setup(
        Sprite sprite,
        Script_Dialogue _dialogue,
        Script_MoveSetModel[] _moveSets
    )
    {
        Debug.Log("setup in StaticNPC");
        
        game = FindObjectOfType<Script_Game>();
        dialogueManager = FindObjectOfType<Script_DialogueManager>();
        dialogue = _dialogue;

        GetComponent<SpriteRenderer>().sprite = sprite;

        AdjustRotation();
    }
}
