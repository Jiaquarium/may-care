using TMPro;

public class Script_SaveEntryChoice : Script_UIChoice
{
    public Script_SaveManager saveManager;
    public TMP_InputField inputField;
    
    /// <summary>
    /// 
    /// called from OnClick
    /// </summary>
    public override void HandleSelect()
    {
        saveManager.InputSaveEntryChoice(Id, inputField.text);
    }
}
