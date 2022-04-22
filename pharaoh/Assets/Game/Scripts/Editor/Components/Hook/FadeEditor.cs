using System;
using UnityEditor;

namespace Pharaoh.Gameplay
{
    [CustomEditor(typeof(Fade))]
    public class FadeEditor : UnityEditor.Editor
    {
        private SerializedProperty _type;
        private SerializedProperty _maxFloat;
        private SerializedProperty _minFloat;
        private SerializedProperty _maxInteger;
        private SerializedProperty _minInteger;
        private SerializedProperty _maxColor;
        private SerializedProperty _minColor;
        private SerializedProperty _maxVector2;
        private SerializedProperty _minVector2;
        private SerializedProperty _maxVector3;
        private SerializedProperty _minVector3;
        private SerializedProperty _maxVector4;
        private SerializedProperty _minVector4;

        private void OnEnable()
        {
            _type       = serializedObject.FindProperty("type");
            _maxFloat   = serializedObject.FindProperty("maxFloat");
            _minFloat   = serializedObject.FindProperty("minFloat");
            _maxInteger = serializedObject.FindProperty("maxInteger");
            _minInteger = serializedObject.FindProperty("minInteger");
            _maxColor   = serializedObject.FindProperty("maxColor");
            _minColor   = serializedObject.FindProperty("minColor");
            _maxVector2 = serializedObject.FindProperty("maxVector2");
            _minVector2 = serializedObject.FindProperty("minVector2");
            _maxVector3 = serializedObject.FindProperty("maxVector3");
            _minVector3 = serializedObject.FindProperty("minVector3");
            _maxVector4 = serializedObject.FindProperty("maxVector4");
            _minVector4 = serializedObject.FindProperty("minVector4");
        }


        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            serializedObject.Update();

            if (_type != null)
            {
                switch (_type.enumValueIndex)
                {
                    case 0:
                        EditorGUILayout.PropertyField(_maxFloat);
                        EditorGUILayout.PropertyField(_minFloat);
                        _maxInteger.intValue = _minInteger.intValue = default;
                        _maxColor.colorValue = _minColor.colorValue = default;
                        _maxVector2.vector2Value = _minVector2.vector2Value = default;
                        _maxVector3.vector3Value = _minVector3.vector3Value = default;
                        _maxVector4.vector4Value = _minVector4.vector4Value = default;
                        break;
                    case 1:
                        EditorGUILayout.PropertyField(_maxInteger);
                        EditorGUILayout.PropertyField(_minInteger);
                        _maxFloat.floatValue = _minFloat.floatValue = default;
                        _maxColor.colorValue = _minColor.colorValue = default;
                        _maxVector2.vector2Value = _minVector2.vector2Value = default;
                        _maxVector3.vector3Value = _minVector3.vector3Value = default;
                        _maxVector4.vector4Value = _minVector4.vector4Value = default;
                        break;
                    case 2:
                        EditorGUILayout.PropertyField(_maxColor);
                        EditorGUILayout.PropertyField(_minColor);
                        _maxFloat.floatValue = _minFloat.floatValue = default;
                        _maxInteger.intValue = _minInteger.intValue = default;
                        _maxVector2.vector2Value = _minVector2.vector2Value = default;
                        _maxVector3.vector3Value = _minVector3.vector3Value = default;
                        _maxVector4.vector4Value = _minVector4.vector4Value = default;
                        break;
                    case 3:
                        EditorGUILayout.PropertyField(_maxVector2);
                        EditorGUILayout.PropertyField(_minVector2);
                        _maxFloat.floatValue = _minFloat.floatValue = default;
                        _maxInteger.intValue = _minInteger.intValue = default;
                        _maxColor.colorValue = _minColor.colorValue = default;
                        _maxVector3.vector3Value = _minVector3.vector3Value = default;
                        _maxVector4.vector4Value = _minVector4.vector4Value = default;
                        break;
                    case 4:
                        EditorGUILayout.PropertyField(_maxVector3);
                        EditorGUILayout.PropertyField(_minVector3);
                        _maxFloat.floatValue = _minFloat.floatValue = default;
                        _maxInteger.intValue = _minInteger.intValue = default;
                        _maxColor.colorValue = _minColor.colorValue = default;
                        _maxVector2.vector2Value = _minVector2.vector2Value = default;
                        _maxVector4.vector4Value = _minVector4.vector4Value = default;
                        break;
                    case 5:
                        EditorGUILayout.PropertyField(_maxVector4);
                        EditorGUILayout.PropertyField(_minVector4);
                        _maxFloat.floatValue = _minFloat.floatValue = default;
                        _maxInteger.intValue = _minInteger.intValue = default;
                        _maxColor.colorValue = _minColor.colorValue = default;
                        _maxVector2.vector2Value = _minVector2.vector2Value = default;
                        _maxVector3.vector3Value = _minVector3.vector3Value = default;
                        break;
                }
            }
            
            serializedObject.ApplyModifiedProperties();
        }
    }
}