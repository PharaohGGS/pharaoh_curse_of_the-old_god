using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Tree = BehaviourTree.Tools.Tree;

namespace BehaviourTree.Editor
{
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
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/BehaviourTree/Editor/BehaviourTreeEditor.uxml");
            visualTree.CloneTree(root);

            // A stylesheet can be added to a VisualElement.
            // The style will be applied to the VisualElement and all of its children.
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/BehaviourTree/Editor/BehaviourTreeEditor.uss");
            root.styleSheets.Add(styleSheet);

            treeView = root.Q<BehaviourTreeView>();
            inspectorView = root.Q<InspectorView>();
            treeView.OnNodeSelected = OnNodeSelectionChanged;
            OnSelectionChange();
        }

        private void OnSelectionChange()
        {
            var tree  = Selection.activeObject as Tree;
            if (tree && AssetDatabase.CanOpenAssetInEditor(tree.GetInstanceID()))
            {
                treeView.PopulateView(tree);
            }
        }

        private void OnNodeSelectionChanged(NodeView nodeView)
        {
            inspectorView.UpdateSelection(nodeView);
        }
    }

}