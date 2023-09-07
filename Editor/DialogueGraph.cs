using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class DialogueGraph : EditorWindow 
{
    private DialogueGraphView _graphView;

    private string _fileName = "New Narrative";

    [MenuItem("LJQ/DialoguePanel")]
    public static void OpenDialogueGraphViewWindow()
    {
        var window = GetWindow<DialogueGraph>();
        window.titleContent = new GUIContent("Dialogue Graph");
    }

    private void OnEnable()
    {
        ConstructGraphView();
        GenerateToolbar();
        GenerateMiniMap();
    }
    private void OnDestroy()
    {
        rootVisualElement.Remove(_graphView);
    }

    /// <summary>
    /// �������ǵı༭����ͼ
    /// </summary>
    private void ConstructGraphView()
    {
        _graphView = new DialogueGraphView
        {
            name = "Dialogue"
        };
        _graphView.StretchToParentSize();
        rootVisualElement.Add(_graphView);
    }

    
    private void GenerateToolbar()
    {
        Toolbar toolbar = new Toolbar();

        // ����ļ���
        var fileNameTextField = new TextField("File Name:");
        fileNameTextField.SetValueWithoutNotify(_fileName);
        fileNameTextField.MarkDirtyRepaint();
        fileNameTextField.RegisterValueChangedCallback(evt => _fileName = evt.newValue);
        toolbar.Add(fileNameTextField);

        // ��Ӽ��أ����水ť
        toolbar.Add(new Button(() => { RequestDataOperation(true); }) { text = "Save Data" }); ;
        toolbar.Add(new Button(() => { RequestDataOperation(false); }) { text = "Load Data" });
        var nodeCreateButton = new Button(() => {_graphView.CreateNode("DialogueNode"); });
        nodeCreateButton.text = "Create Node";
        
        toolbar.Add(nodeCreateButton);  
        rootVisualElement.Add(toolbar);
    }

    private void RequestDataOperation(bool save)
    {
        if (!string.IsNullOrEmpty(_fileName))
        {
            var saveUtility = GraphSaveUtility.GetInstance(_graphView);
            if (save)
                saveUtility.SaveGraph(_fileName);
            else
                saveUtility.LoadGraph(_fileName);
        }
        else
        {
            EditorUtility.DisplayDialog("Invalid File name", "Please Enter a valid filename", "OK");
        }
    }

    private void GenerateMiniMap()
    {
        MiniMap miniMap = new MiniMap
        {
            anchored = true
        };
        miniMap.SetPosition(new Rect(10, 30, 200, 140));
        _graphView.Add(miniMap);


    }
}
