using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class Script_AwaitFile
{
    public static void AwaitFile(string path)
    {
        //Your File
        var file  = new FileInfo(path);

        //While File is not accesable because of writing process
        while (IsFileLocked(file)) { }

        //File is available here
    }

    static bool IsFileLocked(FileInfo file)
    {
        FileStream stream = null;

        try
        {
            stream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
        }
        catch (IOException)
        {
            return true;
        }
        finally
        {
            if (stream != null)
                stream.Close();
        }

        //file is not locked
        return false;
    }
}
