using Codice.CM.Client.Differences;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEditor.Graphs;
using UnityEngine;
using UnityEngine.UIElements;

public class DialogueGraphView : GraphView
{
    public readonly Vector2 defaultNodeSize = new Vector2(150,200);
    public DialogueGraphView()
    {
        // �� Resource �ļ����¼��� Uss �ļ�
        styleSheets.Add(Resources.Load<StyleSheet>("DialogueGraph"));
        // ���÷Ŵ���С
        SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());
        //this.AddManipulator(new FreehandSelector());



        var grid = new GridBackground();
        Insert(0, grid);
        grid.StretchToParentSize();

        AddElement(GenerateEntryPointNode());
    }
    /// <summary>
    /// ��д�����ӽӿں���
    /// </summary>
    /// <param name="startPort"></param>
    /// <param name="nodeAdapter"></param>
    /// <returns></returns>
    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        var compatiblePorts = new List<Port>();
        var startPortView = startPort;

        ports.ForEach((port) =>
        {
            var portView = port;
            // ���������ӵ�������ͳ�ʼ�����
            if (startPortView != portView && startPortView.node != portView.node)
                compatiblePorts.Add(port);
        });

        return compatiblePorts;
    }
    private Port GeneratePort(DialogueNode node, Direction nodeDirection,
        Port.Capacity capacity = Port.Capacity.Single)
    {
        return node.InstantiatePort(Orientation.Horizontal, nodeDirection, capacity, typeof(float));
    }
    private DialogueNode GenerateEntryPointNode()
    {
        var node = new DialogueNode
        {
            title = "Start",
            GUID = Guid.NewGuid().ToString(),
            DialogueText = "EntryPoint",
            EntryPoint = true
        };

        // ������ߵĽ��
        var generatedPort = GeneratePort(node, Direction.Output);
        generatedPort.portName = "Next";
        node.outputContainer.Add(generatedPort);

        // ���������ƶ�����ɾ��
        node.capabilities &= ~Capabilities.Movable;
        node.capabilities &= ~Capabilities.Deletable;

        node.RefreshExpandedState();
        node.RefreshPorts();
        node.SetPosition(new Rect(100,200,100,150));
        return node;
    }

    public void CreateNode(string nodeName)
    {
        AddElement(CreateDialogueNode(nodeName));
    }

    /// <summary>
    /// �����Ի����
    /// </summary>
    /// <param name="nodeName">�������</param>
    /// <returns></returns>
    public DialogueNode CreateDialogueNode(string nodeName)
    {
        var node = new DialogueNode
        {
            title = nodeName,
            DialogueText = nodeName,
            GUID = Guid.NewGuid().ToString(),
        };
        var inputProt = GeneratePort(node,Direction.Input,Port.Capacity.Multi);
        inputProt.portName = "Input";
        node.inputContainer.Add(inputProt);

        // ��� USS �ļ�
        node.styleSheets.Add(Resources.Load<StyleSheet>("Node"));


        // ��Ӱ�ť
        var button = new Button(() => { AddChoicePort(node); });
        button.text = "New Choice";
        node.titleContainer.Add(button);


        var textField = new TextField(String.Empty);
        textField.RegisterValueChangedCallback(evt =>
        {
            node.DialogueText = evt.newValue;
            node.title = evt.newValue;
        });
        textField.SetValueWithoutNotify(node.title);
        node.mainContainer.Add(textField);

        node.RefreshExpandedState();
        node.RefreshPorts();
        node.SetPosition(new Rect(Vector2.zero, defaultNodeSize));
        return node;
    }

    public DialogueNode AddChoicePort(DialogueNode dialogueNode,string overriddenPortName = "")
    {
        var generatePort = GeneratePort(dialogueNode, Direction.Output);
        
        // �Ƴ�
        var oldLable = generatePort.contentContainer.Q<Label>("type");
        generatePort.contentContainer.Remove(oldLable);

        // ��������
        var outPortCount = dialogueNode.outputContainer.Query("connector").ToList().Count;
        string choicePortName = string.IsNullOrEmpty(overriddenPortName)
            ? $"Choic {outPortCount + 1}"
            : overriddenPortName;


        var textField = new TextField
        {
            name = string.Empty,
            value = choicePortName
        };
        textField.RegisterValueChangedCallback(evt => generatePort.portName = evt.newValue);
        generatePort.contentContainer.Add(new Label(" "));
        generatePort.contentContainer.Add(textField);
        // ɾ�����
        var deleteButton = new Button(() => RemovePort(dialogueNode, generatePort))
        {
            text = "X",
        };
        generatePort.contentContainer.Add(deleteButton);
        generatePort.portName = choicePortName;
        dialogueNode.outputContainer.Add(generatePort);
        dialogueNode.RefreshExpandedState();
        dialogueNode.RefreshPorts();
        return dialogueNode;
    }

    private void RemovePort(DialogueNode dialogueNode,Port generatePort)
    {
        var targetEdge = edges.ToList()
               .Where(x => x.output.portName == generatePort.portName && x.output.node == generatePort.node);
        if (targetEdge.Any())
        {
            var edge = targetEdge.First();
            edge.input.Disconnect(edge);
            RemoveElement(targetEdge.First());
        }

        dialogueNode.outputContainer.Remove(generatePort);
        dialogueNode.RefreshPorts();
        dialogueNode.RefreshExpandedState();
    }
}
