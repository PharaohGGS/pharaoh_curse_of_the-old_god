using System;
using Pharaoh.Tools.BehaviourTree.ScriptableObjects;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;


public class BehaviourTreeEditor : EditorWindow
{
    private BehaviourTreeView treeView;
    private InspectorView inspectorView;


    [MenuItem("BehaviourTreeEditor/Editor ...")]
    public static void OpenWindow()
    {
        BehaviourTreeEditor wnd = GetWindow<BehaviourTreeEditor>();
        wnd.titleContent = new GUIContent("BehaviourTreeEditor");
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;
        
        // Import UXML
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Game/Scripts/Editor/BehaviourTree/BehaviourTreeEditor.uxml");
        visualTree.CloneTree(root);

        // A stylesheet can be added to a VisualElement.
        // The style will be applied to the VisualElement and all of its children.
        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Game/Scripts/Editor/BehaviourTree/BehaviourTreeEditor.uss");
        root.styleSheets.Add(styleSheet);

        treeView = root.Q<BehaviourTreeView>();
        inspectorView = root.Q<InspectorView>();
    }

    private void OnSelectionChange()
    {
        BehaviourTree bt  = Selection.activeObject as BehaviourTree;
        if (bt)
        {
            treeView.PopulateView(bt);
        }
    }
}