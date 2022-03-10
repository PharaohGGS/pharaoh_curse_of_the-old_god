using Pharaoh.Gameplay.Components;
using UnityEditor;
using UnityEngine;

[CanEditMultipleObjects]
[CustomEditor(typeof(Damager), true)]
public class DamagerEditor : Editor
{
    protected SerializedProperty _damagingLayersProp;
    protected SerializedProperty _onTriggerHitProp;

    protected SerializedProperty _collidingLayersProp;
    protected SerializedProperty _onCollisionHitProp;

    protected SerializedProperty _maxOverlappedCollidersProp;
    protected SerializedProperty _onOverlapHitProp;
    
    protected virtual void OnEnable()
    {
        _damagingLayersProp = serializedObject.FindProperty("damagingLayers");
        _onTriggerHitProp = serializedObject.FindProperty("onTriggerHit");

        _collidingLayersProp = serializedObject.FindProperty("collidingLayers");
        _onCollisionHitProp = serializedObject.FindProperty("onCollisionHit");

        _maxOverlappedCollidersProp = serializedObject.FindProperty("maxOverlappedColliders");
        _onOverlapHitProp = serializedObject.FindProperty("onOverlapHit");
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
        if (serializedObject.targetObject is not Damager damager || !damager.TryGetComponent(out Collider2D collider2D))
        {
            Debug.LogError($"Collider2D property is null");
            return;
        }

        if (!collider2D.isTrigger)
        {
            if (_collidingLayersProp?.intValue > 0) EditorGUILayout.PropertyField(_onCollisionHitProp);
            return;
        }

        if (_damagingLayersProp?.intValue > 0) EditorGUILayout.PropertyField(_onTriggerHitProp);

        if (_collidingLayersProp?.intValue > 0)
        {
            EditorGUILayout.PropertyField(_maxOverlappedCollidersProp);
            EditorGUILayout.PropertyField(_onOverlapHitProp);
        }
    }
}