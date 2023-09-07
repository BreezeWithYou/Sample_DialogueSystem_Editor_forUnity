

# Sample_DialogueSystem_Editor_forUnity

This is a simple dialogue system editor for Unity, you can configure the dialogue content through a graphical interface

# 可视化对话系统插件的使用

* 添加对应的脚本

![](E:\各类文件\marktext\image\2023-09-07-13-14-04-image.png)

* 输入文件名来创建对应的文件（默认路径是Assets/Resources/DialogueFile）

![](E:\各类文件\marktext\image\2023-09-07-13-14-53-image.png)

* 创建完成后可以打开对话编辑器

![](E:\各类文件\marktext\image\2023-09-07-13-16-27-image.png)

* 通过CreateNode 按钮可以创建结点，一定在编辑后通过Save Data 进行保存![](E:\各类文件\marktext\image\2023-09-07-13-18-04-image.png)

![](E:\各类文件\marktext\image\2023-09-07-13-18-33-image.png)

* 选择同名文件

![](E:\各类文件\marktext\image\2023-09-07-13-19-06-image.png)

* 在游戏运行之后可以通过，Container 进行访问

![](E:\各类文件\marktext\image\2023-09-07-13-19-43-image.png)

* 设计人员可以在Button 上绑定如下函数实现对对话的切换

![](E:\各类文件\marktext\image\2023-09-07-13-20-52-image.png)

![](E:\各类文件\marktext\image\2023-09-07-13-21-22-image.png)

# 插件的功能实现原理

## 实现的目标：

本文参考Youtute的对话编辑器系统（会在后文中给出链接），实现了一个简单的对话系统的可视化编辑器，可以实现如下的效果：进入对话之后，有若干选项以继续对话。如我最近玩的遗迹2中的图片：

![](E:\各类文件\marktext\image\2023-09-05-08-58-21-image.png)

![](E:\各类文件\marktext\image\2023-09-05-08-58-34-image.png)

可视化对话编辑器窗口只是仅仅实现了，该种类型对话的数据存储行为。

主要包含了以下的知识点：

* 自定义检查器窗口

* Unity 中 EditorWindow GraphView Node 等类型

* 使用 ScriptObject 进行数据的存储

本文主要参考了 youtube 上大佬的对话系统的编辑器，所以我在此不会过多的解释每句话的意义而是从大体上理解该插件。

## 插件文件结构与主体文件的解释

#### Editor (Folder)

        *该文件夹主要为对话编辑器系统的视图逻辑部分。注：打包的时候Unity 不会打包Editor目录下的文件*

* Resource 
  
  *存放USS(一种类似CSS的前端视图文件)文件*
  
  * DialogueGraph.uss *对话编辑器背景*
  
  * Node.uss *结点颜色*

* DialogueGraph.cs
  
  *编辑器视图打开*

* DialogueGraphView.cs
  
  *编辑器编辑内容*

* DialogueNode.cs
  
  *编辑器的结点*

* GraphSaveUtilty.cs
  
  *保存功能的实现*

* M_DialogueSystem.cs
  
  *自定义检查器面板*

#### Rumtime

       *存放编辑器编辑完成的数据*

* DialogueContainer.cs

* DialogueNodeData.cs

* DialogueResourceLoad.cs

* NodeLinkData.cs

#### DialogueContainer.cs

       *用于封装对话结点保存的数据取出的一些函数*

#### DialogueSystem.cs

     *用户使用对话系统时添加的脚本*

## DialogueGraph.cs

该类继承于Unity EditorWindow类

EditorWindow 是 Unity 编辑器中的一个类，它允许你创建自定义的编辑器窗口。这些窗口可以包含你自己编写的 GUI 元素，用于构建自定义的编辑器界面。

![](E:\各类文件\marktext\image\2023-09-05-09-38-19-image.png)

如上图可以通过菜单栏中的该内容打开

```C#
public class DialogueGraph : EditorWindow 
{
    private DialogueGraphView _graphView;

    private string _fileName = "New Narrative";

    // 通过菜单栏打开
    [MenuItem("LJQ/DialoguePanel")]
    public static void OpenDialogueGraphViewWindow()
    {
        // 先看看有没有对应的文件夹，没有就创建一个新的文件夹
        DialogueResourceLoad.CreateDefaultFolder();
        var window = GetWindow<DialogueGraph>();
        window.titleContent = new GUIContent("Dialogue Graph");
    }
    
    // 通过名字访问存储的文件
    public void OpenDialogueGraphViewWindow(bool save,string fileName)
    {
        ...... 
    }

    // 打开时会调用下面的文件
    private void OnEnable()
    {
        ConstructGraphView();
        GenerateToolbar();
    }
    private void OnDestroy()
    {
        rootVisualElement.Remove(_graphView);
    }

    // 构造我们的编辑器视图
    private void ConstructGraphView()
    {
        _graphView = new DialogueGraphView
        {
            name = "Dialogue"
        };
        _graphView.StretchToParentSize();
        rootVisualElement.Add(_graphView);
    }

    // 在工具栏创建内容
    private void GenerateToolbar()
    {
        Toolbar toolbar = new Toolbar();

        ...... ......

        toolbar.Add(nodeCreateButton);  
        rootVisualElement.Add(toolbar);
    }
    // （通过 DialogueSystem 打开编辑器的时候可以调用）
    private void GenerateToolbar(bool value)
    {
        .......
    }
}
```

在上述文件中我们主要调用了`rootVisualElement.Add()` 方法为编辑器窗口容器中添加工具栏、编辑视图（DialogueGraphView下面会说）、缩略图。

## DialogueGraphView.cs 和 DialogueNode.cs

该部分主要实现下面的功能、结点显示、连线、视图缩放移动相关操作

![](E:\各类文件\marktext\image\2023-09-05-09-50-54-image.png)

DialogueGraphView 类继承于 GraphView 类型，在 GraphView 类之中Unity 提供的很多的方便的API。

DialogueGraphView.cs 主要是编写如结点相关的函数比如创建，连接结点，

```C#
public class DialogueGraphView : GraphView
{
    public readonly Vector2 defaultNodeSize = new Vector2(150,200);
    public DialogueGraphView()
    {
        // 初始化
    }

    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {

    }
    private Port GeneratePort(DialogueNode node, Direction nodeDirection,
        Port.Capacity capacity = Port.Capacity.Single)
    {
        return node.InstantiatePort(Orientation.Horizontal, nodeDirection, capacity, typeof(float));
    }
    private DialogueNode GenerateEntryPointNode()
    {

    }

    public void CreateNode(string nodeName)
    {
        AddElement(CreateDialogueNode(nodeName));
    }

    // 创建对话结点
    public DialogueNode CreateDialogueNode(string nodeName)
    {

    }
    // 增加选择结点
    public DialogueNode AddChoicePort(DialogueNode dialogueNode,string overriddenPortName = "")
    {

    }
    // 移除连线
    private void RemovePort(DialogueNode dialogueNode,Port generatePort)
    {
        ......
    }
}
```

## DialogueSystem.cs DialogueContainerUse.cs

DialogueContainerUse 脚本主要封装了对话切换的功能

```C#
public class DialogueContainerUse
{
    // DialogueData 是一个结构体用于存放对话数据
    public DialogueData dialogueData;
    
    // 用于切换对话
    // dialogue 是我们定义的编辑器生成的数据，我们需要将这些数据转换成我们自定义的数据
    // narrativeDataGUID 是我们要切换的目标对话结点的 GUID
    public void ProceedToDialogue(DialogueContainer dialogue,string narrativeDataGUID)
    {
      .........
    }

    // 点击对应选项切换对话
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
```

DialogueSystem.cs 文件很简单就不过多解释。主要是对于 DialogueSystem 的自定义检查器面板

```C#
[CustomEditor(typeof(DialogueSystem))]
public class M_DialogueSystem : Editor
{
    public SerializedProperty m_filename;
    public SerializedProperty m_dialogueContainerUse;
    public SerializedProperty m_sourceDialogueData;
    private void OnEnable()
    {
       绑定 SerializedProperty  属性
    }
    // 绘制检查器面板
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
    // 显示原本数据中的字段与属性
    private void ShowSerializingProperties()
    {
        ......
    }
    // 创建检查器面板上的 Button
    private void OnButtonClickOpenDialoguePanel()
    {
        ......
    }
}
```

参考资料：

https://www.youtube.com/watch?v=OMDfr1dzBco&list=PLF3U0rzFKlTGzz-AUFacf9_OKiW_hGYIR&index=2
