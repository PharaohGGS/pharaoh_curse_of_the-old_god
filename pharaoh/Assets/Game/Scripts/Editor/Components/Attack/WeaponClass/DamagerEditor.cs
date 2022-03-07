using Pharaoh.Gameplay.Components;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(Damager), true)]
public class DamagerEditor : Editor
{
    protected SerializedProperty _damagingLayersProp;
    protected SerializedProperty _onTriggerHitProp;

    protected SerializedProperty _collidingLayersProp;
    protected SerializedProperty _onGroundHitProp;
    
    protected virtual void OnEnable()
    {
        _damagingLayersProp = serializedObject.FindProperty("damagingLayers");
        _onTriggerHitProp = serializedObject.FindProperty("onTriggerHit");
        _collidingLayersProp = serializedObject.FindProperty("collidingLayers");
        _onGroundHitProp = serializedObject.FindProperty("onCollidingHit");
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
        if (_damagingLayersProp?.intValue > 0)
        {
            EditorGUILayout.PropertyField(_onTriggerHitProp);
        }

        if (_collidingLayersProp?.intValue > 0)
        {
            EditorGUILayout.PropertyField(_onGroundHitProp);
        }
    }
}