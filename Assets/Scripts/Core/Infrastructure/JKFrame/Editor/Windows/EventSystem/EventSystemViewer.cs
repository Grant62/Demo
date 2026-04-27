using UnityEditor;
using UnityEngine.UIElements;

public class EventSystemViewer : EditorWindow
{
    //[MenuItem("JKFrame/EventSystemViewer")]
    //public static void ShowExample()
    //{
    //    //生成窗口
    //    EventSystemViewer wnd = GetWindow<EventSystemViewer>();
    //    wnd.titleContent = new GUIContent("EventSystemViewer");
    //}

    public void CreateGUI()
    {
        //获取当前窗口的根UI元素容器
        VisualElement root = rootVisualElement;

        // 导入UXML，把读出来的visualTree实例化
        VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/JKFrame/Editor/Windows/EventSystem/EventSystemViewer.uxml");
        VisualElement elementUXML = visualTree.Instantiate();

        // 导入USS样式，绑定到实例化对象上,添加到UI根元素容器下
        StyleSheet styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/JKFrame/Editor/Windows/EventSystem/EventSystemViewer.uss");
        elementUXML.styleSheets.Add(styleSheet);
        root.Add(elementUXML);
    }
}