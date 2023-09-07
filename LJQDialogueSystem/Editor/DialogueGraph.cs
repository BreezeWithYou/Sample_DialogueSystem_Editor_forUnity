using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting.YamlDotNet.Core.Tokens;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class DialogueGraph : EditorWindow 
{
    private DialogueGraphView _graphView;
    private Toolbar toolbar;

    private string _fileName = "New Narrative";

    [MenuItem("LJQ/DialoguePanel")]
    public static void OpenDialogueGraphViewWindow()
    {
        DialogueResourceLoad.CreateDefaultFolder();
        var window = GetWindow<DialogueGraph>();
        window.titleContent = new GUIContent("Dialogue Graph");
    }

    public void OpenDialogueGraphViewWindow(bool save,string fileName)
    {
        DialogueResourceLoad.CreateDefaultFolder();
        var window = GetWindow<DialogueGraph>();
        window.titleContent = new GUIContent("Dialogue Graph");
        if(save == false)
        {
            _fileName = fileName;
            RequestDataOperation(false);
        }
        rootVisualElement.Remove(toolbar);
        GenerateToolbar(false);

    }


    private void OnEnable()
    {
        ConstructGraphView();
        GenerateToolbar();
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
        toolbar = new Toolbar();

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
    private void GenerateToolbar(bool value)
    {
        toolbar = new Toolbar();

        if(value == true)
        {
            // ����ļ���
            var fileNameTextField = new TextField("File Name:");
            fileNameTextField.SetValueWithoutNotify(_fileName);
            fileNameTextField.MarkDirtyRepaint();
            fileNameTextField.RegisterValueChangedCallback(evt => _fileName = evt.newValue);
            toolbar.Add(fileNameTextField);
            toolbar.Add(new Button(() => { RequestDataOperation(false); }) { text = "Load Data" });
        }
        toolbar.Add(new Button(() => { RequestDataOperation(true); }) { text = "Save Data" }); ;
        var nodeCreateButton = new Button(() => { _graphView.CreateNode("DialogueNode"); });
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

}
