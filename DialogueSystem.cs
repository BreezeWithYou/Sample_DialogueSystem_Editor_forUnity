using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DialogueSystem : MonoBehaviour
{
    public string DialogueName;
    public DialogueContainerUse container = new DialogueContainerUse();
    public DialogueContainer SourceDialogueData;

    private void Start()
    {
        LoadSourceDialogueData();
        LoadFirstDialogue();
    }

    private void LoadSourceDialogueData()
    {
        if(SourceDialogueData == null)
        {
            Debug.LogWarning("SourceDialogueData 需要被初始化");
        }
        else
        {
            DialogueName = SourceDialogueData.name;
        }
    }

    private void LoadFirstDialogue()
    {
        container.ProceedToDialogue(SourceDialogueData, SourceDialogueData.NodeLinks.First().TargetNodeGUID);
    }

    public void LoadDialogue(int num)
    {
        container.ProceedToDialogue(SourceDialogueData, num);
    }

}
