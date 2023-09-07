using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
public class DialogueNode : UnityEditor.Experimental.GraphView.Node
{
    public string DialogueText;
    public string GUID;
    public bool EntryPoint = false;

}