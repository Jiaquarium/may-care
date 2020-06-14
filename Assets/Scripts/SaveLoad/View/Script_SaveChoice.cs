public class Script_SaveChoice : Script_UIChoice
{
    public Script_SaveManager saveManager;
    
    public override void HandleSelect()
    {
        saveManager.InputChoice(Id);
    }
}