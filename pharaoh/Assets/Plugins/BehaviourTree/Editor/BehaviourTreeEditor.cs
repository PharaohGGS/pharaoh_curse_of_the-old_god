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
    public class BehaviourTreeEditor : EditorWindow
    {
        private BehaviourTreeView _treeView;
        private InspectorView _inspectorView;
        private IMGUIContainer _blackboardView;

        private SerializedObject _treeObject;
        private SerializedProperty _blackboardProperty;

        public Vector2 mousePositionInEditorWindow { get; private set; }
        
        [MenuItem("BehaviourTreeEditor/Editor ...")]
        public static void OpenWindow()
        {
            BehaviourTreeEditor wnd = GetWindow<BehaviourTreeEditor>();
            wnd.titleContent = new GUIContent("BehaviourTreeEditor");
        }

        [OnOpenAsset]
        public static bool OnOpenAsset(int instanceId, int line)
        {
            if (Selection.activeObject is not BTree) return false;

            OpenWindow();
            return true;
        }

        public void CreateGUI()
        {
            // Each editor editorWindow contains a root VisualElement object
            VisualElement root = rootVisualElement;
            var uiBuilderPath = this.GetAssetPath().Replace("Editor/BehaviourTreeEditor.cs", "UiBuilder");

            // Import UXML
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>($"{uiBuilderPath}/BehaviourTreeEditor.uxml");
            visualTree.CloneTree(root);

            // A stylesheet can be added to a VisualElement.
            // The style will be applied to the VisualElement and all of its children.
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>($"{uiBuilderPath}/BehaviourTreeEditor.uss");
            root.styleSheets.Add(styleSheet);

            rootVisualElement.RegisterCallback<MouseMoveEvent>(OnMouseMove, TrickleDown.TrickleDown);

            _treeView = root.Q<BehaviourTreeView>();
            _treeView.OnNodeSelected = OnNodeSelectionChanged;
            _treeView.editorWindow = this;

            _inspectorView = root.Q<InspectorView>();
            _blackboardView = root.Q<IMGUIContainer>();
            _blackboardView.MarkDirtyLayout();
            _blackboardView.onGUIHandler = () =>
            {
                if (_blackboardProperty?.FindPropertyRelative("debugData") == null) return;

                _treeObject?.Update();
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(_blackboardProperty);
                _treeObject?.ApplyModifiedProperties();
            };

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

        private void OnSelectionChange()
        {
            var tree  = Selection.activeObject as BTree;

            if (tree == null && Selection.activeGameObject != null && 
                Selection.activeGameObject.TryGetComponent(out BehaviourTreeRunner runner))
            {
                tree = runner.tree;
            }

            if (tree == null) return;

            switch (Application.isPlaying)
            {
                case true:
                case false when AssetDatabase.CanOpenAssetInEditor(tree.GetInstanceID()):
                    _treeView?.PopulateView(tree);
                    break;
            }
                
            _treeObject = new SerializedObject(tree);
            _blackboardProperty = _treeObject.FindProperty("blackboard");
            //_blackboardProperty?.FindPropertyRelative("debugData")?.RemoveEmptyArrayElements();
        }

        private void OnInspectorUpdate()
        {
            _treeView?.UpdateNodeStates();
        }

        private void OnMouseMove(MouseMoveEvent evt)
        {
            mousePositionInEditorWindow = evt.mousePosition;
        }

        private void OnPlayModeStateChanged(PlayModeStateChange playModeStateChange)
        {
            switch (playModeStateChange)
            {
                case PlayModeStateChange.EnteredEditMode:
                case PlayModeStateChange.EnteredPlayMode:
                    OnSelectionChange();
                    break;
                case PlayModeStateChange.ExitingEditMode:
                case PlayModeStateChange.ExitingPlayMode:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(playModeStateChange), playModeStateChange, null);
            }
        }

        private void OnNodeSelectionChanged(NodeView nodeView)
        {
            _inspectorView.UpdateSelection(nodeView);
        }
    }
}