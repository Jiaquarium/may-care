using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[CreateAssetMenu(fileName = "TMPInputValidator", menuName = "TMPInputValidator")]
public class Script_TMPInputValidator : TMP_InputValidator
{
    public override char Validate(ref string text, ref int pos, char ch)
    {
        int ASCIICode = (int)ch;
        
        if (
            ASCIICode >= Const_InputValidation.Entry.minASCII
            && ASCIICode <= Const_InputValidation.Entry.maxASCII
        )
        {
            text = text.Insert(pos, ch.ToString());
            pos++;
            return ch;
        }
        
        return ch;
    }
}
