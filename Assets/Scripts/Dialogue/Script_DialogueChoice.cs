public class Script_DialogueChoice : Script_UIChoice
{
    public Script_ChoiceManager choiceManager;

    public override void HandleSelect()
    {
        // call choice manager to input this choice
        choiceManager.InputChoice(Id);
    }
}
