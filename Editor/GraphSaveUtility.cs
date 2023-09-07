using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.iOS;
using UnityEngine;
using UnityEngine.UIElements;


/// <summary>
/// 图形化界面保存
/// </summary>
public class GraphSaveUtility
{
    private DialogueGraphView _targetGraphView;
    private DialogueContainer _contaninerCache;
    private List<Edge> Edges => _targetGraphView.edges.ToList();
    private List<DialogueNode> Nodes => _targetGraphView.nodes.ToList().Cast<DialogueNode>().ToList();


    public static GraphSaveUtility GetInstance(DialogueGraphView targetGraphView)
    {
        return new GraphSaveUtility
        {
            _targetGraphView = targetGraphView
        };
    }

    public void SaveGraph(string fileName)
    {
        if (!Edges.Any()) return;
        // 定义一个对话的容器用来存储数据
        var dialogueContainer = ScriptableObject.CreateInstance<DialogueContainer>();
        
        //if (!SaveNodes(fileName, dialogueContainer)) return;
        //SaveExposedProperties(dialogueContainer);
        //SaveCommentBlocks(dialogueContainer);

        if (!AssetDatabase.IsValidFolder("Assets/Resources"))
            AssetDatabase.CreateFolder("Assets", "Resources");

        var connectedPorts = Edges.Where(x => x.input.node != null).ToArray();
        for(int i = 0; i < connectedPorts.Length; i++)
        {
            DialogueNode outputNode = connectedPorts[i].output.node as DialogueNode;
            DialogueNode inputNode = connectedPorts[i].input.node as DialogueNode;

            dialogueContainer.NodeLinks.Add(new NodeLinkData
            {
                BaseNodeGUID = outputNode.GUID,
                PortName = connectedPorts[i].output.portName,
                TargetNodeGUID = inputNode.GUID
            }) ;
        }

        foreach(var dialogueNode in Nodes.Where(node => !node.EntryPoint))
        {
            dialogueContainer.DialogueNodeData.Add(new DialogueNodeData
            {
                NodeGUID = dialogueNode.GUID,
                DialogueText = dialogueNode.DialogueText,
                Position = dialogueNode.GetPosition().position
            });
        }

        AssetDatabase.CreateAsset(dialogueContainer,$"Assets/Resources/{fileName}.asset");
        AssetDatabase.SaveAssets();
    }

    private bool SaveNodes(string fileName, DialogueContainer dialogueContainerObject)
    {


        return true;
    }

    private void SaveExposedProperties(DialogueContainer dialogueContainer)
    {

    }

    private void SaveCommentBlocks(DialogueContainer dialogueContainer)
    {

    }

    public void LoadGraph(string fileName)
    {
        _contaninerCache = Resources.Load<DialogueContainer>(fileName);
        if (_contaninerCache == null)
        {
            EditorUtility.DisplayDialog("File Not Found","该文件名不存在","OK");
            return;
        }
        ClearGraph();
        GenerateDialogueNodes();
        ConnectDialogueNodes();
        //AddExposedProperties();
        //GenerateCommentBlocks();
    }

    /// <summary>
    /// Set Entry point GUID then Get All Nodes, remove all and their edges. Leave only the entrypoint node. (Remove its edge too)
    /// </summary>
    private void ClearGraph()
    {
        Nodes.Find(x => x.EntryPoint).GUID = _contaninerCache.NodeLinks[0].BaseNodeGUID; 
        foreach (var perNode in Nodes)
        {
            if (perNode.EntryPoint) continue;
            Edges.Where(x => x.input.node == perNode).ToList()
                .ForEach(edge => _targetGraphView.RemoveElement(edge));
            _targetGraphView.RemoveElement(perNode);
        }
    }

    /// <summary>
    /// Create All serialized nodes and assign their guid and dialogue text to them
    /// </summary>
    private void GenerateDialogueNodes()
    {
        foreach (var perNode in _contaninerCache.DialogueNodeData)
        {
            var tempNode = _targetGraphView.CreateDialogueNode(perNode.DialogueText);
            tempNode.GUID = perNode.NodeGUID;
            _targetGraphView.AddElement(tempNode);

            var nodePorts = _contaninerCache .NodeLinks.Where(x => x.BaseNodeGUID == perNode.NodeGUID).ToList();
            nodePorts.ForEach(x => _targetGraphView.AddChoicePort(tempNode, x.PortName));
        }
    }

    private void ConnectDialogueNodes()
    {
        for (var i = 0; i < Nodes.Count; i++)
        {
            var k = i; //Prevent access to modified closure
            var connections = _contaninerCache.NodeLinks.Where(x => x.BaseNodeGUID == Nodes[k].GUID).ToList();
            for (var j = 0; j < connections.Count(); j++)
            {
                var targetNodeGUID = connections[j].TargetNodeGUID;
                var targetNode = Nodes.First(x => x.GUID == targetNodeGUID);
                LinkNodes(Nodes[i].outputContainer[j].Q<Port>(), (Port)targetNode.inputContainer[0]);
                targetNode.SetPosition(new Rect(
                    _contaninerCache.DialogueNodeData.First(x => x.NodeGUID == targetNodeGUID).Position,
                    _targetGraphView.defaultNodeSize));
            }
        }
    }

    private void LinkNodes(Port outputSocket, Port inputSocket)
    {
        var tempEdge = new Edge()
        {
            output = outputSocket,
            input = inputSocket
        };
        tempEdge?.input.Connect(tempEdge);
        tempEdge?.output.Connect(tempEdge);
        _targetGraphView.Add(tempEdge);
    }

    private void AddExposedProperties()
    {

    }

    private void GenerateCommentBlocks()
    {
        
    }
}
