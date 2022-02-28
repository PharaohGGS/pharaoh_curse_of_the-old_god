using System.Collections;
using System.Collections.Generic;
using Pharaoh.Gameplay.Components;
using UnityEditor;
using UnityEngine;

[CanEditMultipleObjects]
[CustomEditor(typeof(DamagerData), true)]
public class DamagerDataEditor : Editor
{
    private SerializedProperty _isThrowable;
    private SerializedProperty _throwableRangeProp;
    private SerializedProperty _throwablePickingTimeProp;
    
    private void OnEnable()
    {
        _isThrowable = serializedObject.FindProperty("throwable");
        _throwableRangeProp = serializedObject.FindProperty("throwableRange");
        _throwablePickingTimeProp = serializedObject.FindProperty("throwablePickingTime");
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        serializedObject.Update();

        if (_isThrowable?.boolValue == true)
        {
            EditorGUILayout.PropertyField(_throwableRangeProp);
            EditorGUILayout.PropertyField(_throwablePickingTimeProp);
        }
        
        serializedObject.ApplyModifiedProperties();
    }
}
