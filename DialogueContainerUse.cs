using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class DialogueContainerUse
{
    public DialogueData dialogueData;

    public void ProceedToDialogue(DialogueContainer dialogue,string narrativeDataGUID)
    {
        var text = dialogue.DialogueNodeData.Find(x => x.NodeGUID == narrativeDataGUID).DialogueText;
        var choices = dialogue.NodeLinks.Where(x => x.BaseNodeGUID == narrativeDataGUID);
        
        dialogueData.dialogueText = text;
        dialogueData.choiceText.Clear();
        dialogueData.nextNodeGUID.Clear();
        
        foreach (var choice in choices)
        {
            dialogueData.choiceText.Add(choice.PortName);
            dialogueData.nextNodeGUID.Add(choice.TargetNodeGUID);
        }
    }

    public void ProceedToDialogue(DialogueContainer dialogue,int num)
    {
        if (num < dialogueData.nextNodeGUID.Count)
            return;
        var narrativeDataGUID = dialogueData.nextNodeGUID[num];
        ProceedToDialogue(dialogue, narrativeDataGUID);
    }
}

[Serializable]
public struct DialogueData
{
    public string dialogueText;
    public List<string> choiceText;
    [HideInInspector]
    public List<string> nextNodeGUID;
}