using Pharaoh.Gameplay.Components;
using UnityEditor;
using UnityEngine;

[CanEditMultipleObjects]
[CustomEditor(typeof(Gear), true)]
public class GearEditor : Editor
{
    protected SerializedProperty _collidingLayersProp;
    protected SerializedProperty _onGroundHitProp;
    
    protected virtual void OnEnable()
    {
        _collidingLayersProp = serializedObject.FindProperty("collidingLayers");
        _onGroundHitProp = serializedObject.FindProperty("onGroundHit");
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        serializedObject.Update();

        DrawSpecificProperties();
        
        serializedObject.ApplyModifiedProperties();
    }

    protected virtual void DrawSpecificProperties()
    {
        if (_collidingLayersProp?.intValue <= 0) return;
        EditorGUILayout.PropertyField(_onGroundHitProp);
    }
}