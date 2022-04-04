using Pharaoh.Gameplay.Components;
using UnityEditor;
using UnityEngine;

[CanEditMultipleObjects]
[CustomEditor(typeof(Damager))]
public class DamagerEditor : Editor
{
    protected SerializedProperty _damagingLayersProp;
    protected SerializedProperty _onTriggerHitProp;

    protected SerializedProperty _collidingLayersProp;
    protected SerializedProperty _onCollisionHitProp;
    
    
    private void OnEnable()
    {
        _damagingLayersProp = serializedObject.FindProperty("damagingLayers");
        _onTriggerHitProp = serializedObject.FindProperty("onTriggerHit");

        _collidingLayersProp = serializedObject.FindProperty("collidingLayers");
        _onCollisionHitProp = serializedObject.FindProperty("onCollisionHit");
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        serializedObject.Update();

        if (_damagingLayersProp?.intValue > 0) EditorGUILayout.PropertyField(_onTriggerHitProp);
        if (_collidingLayersProp?.intValue > 0) EditorGUILayout.PropertyField(_onCollisionHitProp);

        serializedObject.ApplyModifiedProperties();
    }
}