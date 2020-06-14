using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_LevelBehavior_14_BioArt : Script_LevelBehavior
{
    public bool isComplete;
    public GameObject MelzSpotlight;
    public Script_DialogueNode StartNode;
    public Script_DialogueManager dm;
    public Model_Locations triggerLocs;
    public AudioSource audioSource;
    public AudioClip SpotlightSFX;
    public float spotlightVolumeScale;
    public float WaitForDialogueTime;
    
    
    private bool startDialogue;
    private bool isTriggerActivated;
    private bool shouldEnd;
    private bool isFinishDialogue;

    protected override void HandleTriggerLocations()
    {
        foreach (Vector3 loc in triggerLocs.locations)
        {
            if (
                game.GetPlayerLocation() == loc
                && game.state == "interact"
                && !isTriggerActivated
            )
            {
                audioSource.PlayOneShot(SpotlightSFX, spotlightVolumeScale);
                MelzSpotlight.SetActive(true);
                isTriggerActivated = true;
                game.ChangeStateCutScene();
                game.GetNPC(0).gameObject.SetActive(true);
                game.GetNPC(1).gameObject.SetActive(true);

                StartCoroutine(WaitForDialogue());
            }
        }

        if (
            game.state == "cut-scene"
            && !game.GetPlayerIsTalking()
            && !isFinishDialogue
            && shouldEnd
        )
        {
            isFinishDialogue = false;
            shouldEnd = false;
            isComplete = true;
            game.ChangeStateInteract();
        }
    }

    IEnumerator WaitForDialogue()
    {
        yield return new WaitForSeconds(WaitForDialogueTime);

        shouldEnd = true;
        dm.StartDialogueNode(StartNode);
    }
    
    protected override void HandleAction()
    {
        base.HandleDialogueAction();
    }
    
    public override void Setup()
    {
        startDialogue = false;
        isTriggerActivated = false;
        isFinishDialogue = false;
        shouldEnd = false;
        MelzSpotlight.SetActive(false);

        game.CreateNPCs();
        game.GetNPC(0).gameObject.SetActive(false);
        game.GetNPC(1).gameObject.SetActive(false);
    }    
}
