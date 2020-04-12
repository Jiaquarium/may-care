using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_StaticNPC : MonoBehaviour
{
    public int StaticNPCId;
    public Script_Game game;
    private Script_DialogueManager dialogueManager;
    public Model_Dialogue dialogue;
    

    // Update is called once per frame
    void Update()
    {
        AdjustRotation();
    }

    public void TriggerDialogue()
    {
        dialogueManager.StartDialogue(dialogue, null);
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
        transform.forward = Camera.main.transform.forward;
    }

    public virtual void Move() {}

    public virtual void Setup(
        Sprite sprite,
        Model_Dialogue _dialogue,
        Model_MoveSet[] _moveSets
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
