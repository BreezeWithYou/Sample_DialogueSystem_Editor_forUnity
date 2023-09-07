using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DialogueSystem))]
public class M_DialogueSystem : Editor
{
   
    public SerializedProperty m_filename;
    public SerializedProperty m_dialogueContainerUse;
    public SerializedProperty m_sourceDialogueData;
    private void OnEnable()
    {
        m_filename = serializedObject.FindProperty("DialogueName");
        m_dialogueContainerUse = serializedObject.FindProperty("container");
        m_sourceDialogueData = serializedObject.FindProperty("SourceDialogueData");
    }

    public override void OnInspectorGUI()
    {
        ShowSerializingProperties();
        OnButtonClickOpenDialoguePanel();
        // 应用修改
        serializedObject.ApplyModifiedProperties();
        if (GUI.changed)
        {
            // 标记对象为已修改
            EditorUtility.SetDirty(target);
            Undo.RecordObject(target, "Save Changes");
        }
    }

    private void ShowSerializingProperties()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(this.m_filename);
        if (m_filename.stringValue == "")
        {
            EditorGUILayout.HelpBox("You need to input filename", MessageType.Warning);//黄色警告号
        }

        if (m_sourceDialogueData.objectReferenceValue != null && (m_filename.stringValue != m_sourceDialogueData.objectReferenceValue.name))
        {
            m_filename.stringValue = m_sourceDialogueData.objectReferenceValue.name;
        }
        EditorGUILayout.PropertyField(this.m_dialogueContainerUse);
        EditorGUILayout.PropertyField(this.m_sourceDialogueData);
    }

    private void OnButtonClickOpenDialoguePanel()
    {
        Object file1 = AssetDatabase.LoadAssetAtPath<Object>($"Assets/Resources/" + DialogueResourceLoad.LoadFolder + $"/{m_filename.stringValue}.asset");
        if (file1 == null && m_filename.stringValue != "")
        {
            if (GUILayout.Button("创建对话文件"))
            {
                DialogueContainer dialogueContainerTemp = new DialogueContainer();
                DialogueResourceLoad.CreateAsset(dialogueContainerTemp, m_filename.stringValue);
            }
        }
        else if(file1 != null && m_filename.stringValue != "")
        {
            if (GUILayout.Button("打开对话编辑器"))
            {
                Object filetemp = AssetDatabase.LoadAssetAtPath<Object>($"Assets/Resources/" + DialogueResourceLoad.LoadFolder + $"/{m_filename.stringValue}.asset");
                if (filetemp == null)
                {
                    EditorUtility.DisplayDialog("Invalid File name", "Please Enter a valid filename", "OK");
                    return;
                }
                DialogueGraph window = new DialogueGraph();
                window.OpenDialogueGraphViewWindow(false, m_filename.stringValue);
            }
        }
    }

}
