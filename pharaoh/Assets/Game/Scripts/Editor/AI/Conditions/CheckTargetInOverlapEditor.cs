using UnityEditor;

namespace Pharaoh.AI.Actions
{
    [CustomEditor(typeof(CheckTargetInOverlap))]
    public class CheckTargetInOverlapEditor : Editor
    {
        private SerializedProperty _is2DProp;

        private SerializedProperty _colliders3DProp;
        private SerializedProperty _colliders2DProp;

        private void OnEnable()
        {
            _is2DProp = serializedObject.FindProperty("is2D");
            _colliders3DProp = serializedObject.FindProperty("colliders3D");
            _colliders2DProp = serializedObject.FindProperty("colliders2D");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            base.OnInspectorGUI();

            EditorGUILayout.PropertyField(_is2DProp?.boolValue == true ? _colliders2DProp : _colliders3DProp);
            serializedObject.ApplyModifiedProperties();
        }
    }
}