using System;
using System.IO;
using System.Reflection;
using System.Text;
using BehaviourTree.Tools;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Callbacks;

namespace BehaviourTree.Editor
{
    public static class EditorHelper
    {
        public static int RemoveEmptyArrayElements(this SerializedProperty list)
        {
            var elementsRemoved = 0;
            if (list.serializedObject != null)
            {
                for (int i = list.arraySize - 1; i >= 0; i--)
                {
                    if (list.GetArrayElementAtIndex(i).objectReferenceValue == null)
                    {
                        list.DeleteArrayElementAtIndex(i);
                        elementsRemoved++;
                    }
                }
            }
     
            return elementsRemoved;
        }
    }

    public class BehaviourTreeEditor : EditorWindow
    {
        private BehaviourTreeView _treeView;
        private InspectorView _inspectorView;
        private IMGUIContainer _blackboardView;

        private SerializedObject _treeObject;
        private SerializedProperty _blackboardProperty;
        
        [MenuItem("BehaviourTreeEditor/Editor ...")]
        public static void OpenWindow()
        {
            BehaviourTreeEditor wnd = GetWindow<BehaviourTreeEditor>();
            wnd.titleContent = new GUIContent("BehaviourTreeEditor");
        }

        [OnOpenAsset]
        public static bool OnOpenAsset(int instanceId, int line)
        {
            if (Selection.activeObject is BTree tree)
            {
                OpenWindow();
                return true;
            }

            return false;
        }

        public void CreateGUI()
        {
            // Each editor window contains a root VisualElement object
            VisualElement root = rootVisualElement;
            var uiBuilderPath = this.GetAssetPath().Replace("Editor/BehaviourTreeEditor.cs", "UiBuilder");

            // Import UXML
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>($"{uiBuilderPath}/BehaviourTreeEditor.uxml");
            visualTree.CloneTree(root);

            // A stylesheet can be added to a VisualElement.
            // The style will be applied to the VisualElement and all of its children.
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>($"{uiBuilderPath}/BehaviourTreeEditor.uss");
            root.styleSheets.Add(styleSheet);

            _treeView = root.Q<BehaviourTreeView>();
            _inspectorView = root.Q<InspectorView>();
            _blackboardView = root.Q<IMGUIContainer>();
            _blackboardView.onGUIHandler = () =>
            {
                if (_treeObject == null) return;

                _treeObject?.Update();
                EditorGUILayout.PropertyField(_blackboardProperty, true);
                _treeObject?.ApplyModifiedProperties();
            };

            _treeView.OnNodeSelected = OnNodeSelectionChanged;
            OnSelectionChange();
        }

        private void OnEnable()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private void OnDisable()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        }

        private void OnPlayModeStateChanged(PlayModeStateChange playModeStateChange)
        {
            switch (playModeStateChange)
            {
                case PlayModeStateChange.EnteredEditMode:
                    OnSelectionChange();
                    break;
                case PlayModeStateChange.ExitingEditMode:
                    break;
                case PlayModeStateChange.EnteredPlayMode:
                    OnSelectionChange();
                    break;
                case PlayModeStateChange.ExitingPlayMode:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(playModeStateChange), playModeStateChange, null);
            }
        }

        private void OnSelectionChange()
        {
            BehaviourTreeRunner runner = null;
            BTree tree  = Selection.activeObject as BTree;

            if (tree == null && Selection.activeGameObject?.TryGetComponent(out runner) == true)
            {
                tree = runner.tree;
            }

            if (tree == null) return;

            if (Application.isPlaying)
            {
                _treeView?.PopulateView(tree);
            }
            else
            {
                if (AssetDatabase.CanOpenAssetInEditor(tree.GetInstanceID()))
                {
                    _treeView?.PopulateView(tree);
                }
            }

            _treeObject = new SerializedObject(tree);
            _blackboardProperty = _treeObject.FindProperty("blackboard");

            if (_blackboardProperty.isArray)
            {
                _blackboardProperty?.RemoveEmptyArrayElements();
            }
        }

        private void OnNodeSelectionChanged(NodeView nodeView)
        {
            _inspectorView.UpdateSelection(nodeView);
        }

        private void OnInspectorUpdate()
        {
            _treeView.UpdateNodeStates();
        }
    }
}