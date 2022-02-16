using System;
using UnityEditor;

namespace Pharaoh.Gameplay.Components
{

    [CustomEditor(typeof(RangeWeaponData))]
    public class RangeWeaponDataEditor : Editor
    {
        private SerializedProperty useGravityProperty;

        private SerializedProperty gravityProperty;
        private SerializedProperty heightProperty;

        private void OnEnable()
        {
            useGravityProperty = serializedObject.FindProperty("useGravity");
            gravityProperty = serializedObject.FindProperty("gravity");
            heightProperty = serializedObject.FindProperty("height");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(useGravityProperty);
            var useGravityToggle = useGravityProperty.boolValue;

            if (useGravityToggle)
            {
                EditorGUILayout.PropertyField(gravityProperty);
                EditorGUILayout.PropertyField(heightProperty);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
    
}
