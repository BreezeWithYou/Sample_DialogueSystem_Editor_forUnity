using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEditor;
using UnityEngine;

public class DialogueResourceLoad
{
    static readonly public string LoadFolder = "DialogueFile";
    static readonly public string AllLoadPath = "Assets/Resources/DialogueFile";
    static public void LoadAsset()
    {

    }
    /// <summary>
    /// 创建默认的存放文件的文件夹
    /// </summary>
    static public void CreateDefaultFolder()
    {
        if (!AssetDatabase.IsValidFolder("Assets/Resources"))
            AssetDatabase.CreateFolder("Assets", "Resources");

        if (!AssetDatabase.IsValidFolder(AllLoadPath))
            AssetDatabase.CreateFolder("Assets/Resources", LoadFolder);
    }

    static public void CreateAsset(DialogueContainer dialogueContainer,string fileName)
    {
        AssetDatabase.CreateAsset(dialogueContainer, AllLoadPath+ $"/{fileName}.asset");
    }

    static public DialogueContainer ResourcesLoad(string fileName)
    {
        DialogueContainer dialogueData = Resources.Load<DialogueContainer>(LoadFolder +"/"+ fileName);
        Debug.Log(LoadFolder + "/" + fileName);
        return dialogueData;
    }

}
