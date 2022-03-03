using System.Collections;
using System.Collections.Generic;
using Pharaoh.Gameplay.Components;
using UnityEditor;
using UnityEngine;

[CanEditMultipleObjects]
[CustomEditor(typeof(MeleeGearData))]
public class MeleeGearDataEditor : GearDataEditor
{
    protected SerializedProperty _isThrowable;
    protected SerializedProperty _throwableRangeProp;
    protected SerializedProperty _throwablePickingTimeProp;
    
    protected override void OnEnable()
    {
        base.OnEnable();
        _isThrowable = serializedObject.FindProperty("throwable");
        _throwableRangeProp = serializedObject.FindProperty("throwableRange");
        _throwablePickingTimeProp = serializedObject.FindProperty("throwablePickingTime");
    }

    protected override void DrawSpecificProperties()
    {
        base.DrawSpecificProperties();

        if (_isThrowable?.boolValue != true) return;
        EditorGUILayout.PropertyField(_throwableRangeProp);
        EditorGUILayout.PropertyField(_throwablePickingTimeProp);
    }
}
