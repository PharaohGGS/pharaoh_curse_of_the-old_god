using System;
using UnityEditor;
using UnityEngine;

namespace Pharaoh.Gameplay.Components
{

    [CustomEditor(typeof(DetectionComponent))]
    public class DetectionComponentEditor : Editor
    {
        private SerializedProperty _is2DProp;

        private SerializedProperty _collider3DProp;
        private SerializedProperty _colliders3DProp;
        private SerializedProperty _layeredColliders3DProp;

        private SerializedProperty _collider2DProp;
        private SerializedProperty _colliders2DProp;
        private SerializedProperty _layeredColliders2DProp;

        private void OnEnable()
        {
            _is2DProp = serializedObject.FindProperty("_is2D");

            _collider3DProp = serializedObject.FindProperty("collider3D");
            _colliders3DProp = serializedObject.FindProperty("colliders3D");
            _layeredColliders3DProp = serializedObject.FindProperty("layeredColliders3D");

            _collider2DProp = serializedObject.FindProperty("collider2D");
            _colliders2DProp = serializedObject.FindProperty("colliders2D");
            _layeredColliders2DProp = serializedObject.FindProperty("layeredColliders2D");
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            serializedObject.Update();
            
            if (_is2DProp != null)
            {
                bool is2D = _is2DProp.boolValue;

                var collProperty = !is2D ? _collider3DProp : _collider2DProp;
                if (collProperty != null) EditorGUILayout.PropertyField(collProperty);

                var collsProperty = !is2D ? _colliders3DProp : _colliders2DProp;
                if (collsProperty != null) EditorGUILayout.PropertyField(collsProperty);

                var dicoProperty = !is2D ? _layeredColliders3DProp : _layeredColliders2DProp;
                if (dicoProperty != null) EditorGUILayout.PropertyField(dicoProperty);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}