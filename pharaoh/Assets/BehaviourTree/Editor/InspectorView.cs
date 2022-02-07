using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace BehaviourTree.Editor
{
    public class InspectorView : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<InspectorView, VisualElement.UxmlTraits>
        {
        }

        private UnityEditor.Editor _editor;

        public InspectorView()
        {

        }

        public void UpdateSelection(NodeView nodeView)
        {
            Clear();

            UnityEngine.Object.DestroyImmediate(_editor);
            _editor = UnityEditor.Editor.CreateEditor(nodeView.node);
            IMGUIContainer container = new IMGUIContainer(() => { _editor.OnInspectorGUI(); });
            Add(container);

        }
    }
}
