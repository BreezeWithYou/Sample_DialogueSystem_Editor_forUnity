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
        // Ӧ���޸�
        serializedObject.ApplyModifiedProperties();
        if (GUI.changed)
        {
            // ��Ƕ���Ϊ���޸�
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
            EditorGUILayout.HelpBox("You need to input filename", MessageType.Warning);//��ɫ�����
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
            if (GUILayout.Button("�����Ի��ļ�"))
            {
                DialogueContainer dialogueContainerTemp = new DialogueContainer();
                DialogueResourceLoad.CreateAsset(dialogueContainerTemp, m_filename.stringValue);
            }
        }
        else if(file1 != null && m_filename.stringValue != "")
        {
            if (GUILayout.Button("�򿪶Ի��༭��"))
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
