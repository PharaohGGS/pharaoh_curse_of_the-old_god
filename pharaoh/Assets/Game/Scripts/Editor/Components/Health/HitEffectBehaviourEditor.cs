using Pharaoh.Gameplay.Components;
using UnityEditor;
using UnityEngine;

[CanEditMultipleObjects]
[CustomEditor(typeof(HitEffectBehaviour))]
public class HitEffectBehaviourEditor : Editor
{
    [Header("Blink")] 
    private SerializedProperty _useBlinkProperty;
    private SerializedProperty _blkRenderersProperty;
    private SerializedProperty _blkDurationProperty;
    private SerializedProperty _blkCountProperty;
    private SerializedProperty _blkIntensityProperty;
    private SerializedProperty _blkColorProperty;
    
    [Header("KnockBack")] 
    private SerializedProperty _useKnockBackProperty;
    private SerializedProperty _kbForceProperty;
    private SerializedProperty _kbDurationProperty;
    private SerializedProperty _onKnockBackStartProperty;
    private SerializedProperty _onKnockBackEndProperty;
    
    [Header("ScreenShake")] 
    private SerializedProperty _useScreenShakeProperty;
    private SerializedProperty _ssDampingSpeedProperty;
    private SerializedProperty _ssMagnitudeProperty;
    private SerializedProperty _ssDurationProperty;
    private SerializedProperty _onScreenShakeProperty;
    
    
    private void OnEnable()
    {
        // blink
        _useBlinkProperty     = serializedObject.FindProperty("useBlink");
        _blkRenderersProperty = serializedObject.FindProperty("blkRenderers");
        _blkDurationProperty  = serializedObject.FindProperty("blkDuration");
        _blkCountProperty     = serializedObject.FindProperty("blkCount");
        _blkIntensityProperty = serializedObject.FindProperty("blkIntensity");
        _blkColorProperty     = serializedObject.FindProperty("blkColor");
        // knockback
        _useKnockBackProperty     = serializedObject.FindProperty("useKnockBack");
        _kbForceProperty          = serializedObject.FindProperty("kbForce");
        _kbDurationProperty       = serializedObject.FindProperty("kbDuration");
        _onKnockBackStartProperty = serializedObject.FindProperty("onKnockBackStart");
        _onKnockBackEndProperty   = serializedObject.FindProperty("onKnockBackEnd");
        // screenshake
        _useScreenShakeProperty = serializedObject.FindProperty("useScreenShake");
        _ssDampingSpeedProperty = serializedObject.FindProperty("ssDampingSpeed");
        _ssMagnitudeProperty    = serializedObject.FindProperty("ssMagnitude");
        _ssDurationProperty     = serializedObject.FindProperty("ssDuration");
        _onScreenShakeProperty  = serializedObject.FindProperty("onScreenShake");

    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        serializedObject.Update();
        
        EditorGUILayout.PropertyField(_useBlinkProperty);
        if (_useBlinkProperty?.boolValue == true)
        {
            EditorGUILayout.PropertyField(_blkRenderersProperty);
            EditorGUILayout.PropertyField(_blkDurationProperty);
            EditorGUILayout.PropertyField(_blkCountProperty);
            EditorGUILayout.PropertyField(_blkIntensityProperty);
            EditorGUILayout.PropertyField(_blkColorProperty);
        }
        
        EditorGUILayout.PropertyField(_useKnockBackProperty);
        if (_useKnockBackProperty?.boolValue == true)
        {
            EditorGUILayout.PropertyField(_kbForceProperty);
            EditorGUILayout.PropertyField(_kbDurationProperty);
            EditorGUILayout.PropertyField(_onKnockBackStartProperty);
            EditorGUILayout.PropertyField(_onKnockBackEndProperty);
        }
        
        EditorGUILayout.PropertyField(_useScreenShakeProperty);
        if (_useScreenShakeProperty?.boolValue == true)
        {
            EditorGUILayout.PropertyField(_ssDampingSpeedProperty);
            EditorGUILayout.PropertyField(_ssMagnitudeProperty);
            EditorGUILayout.PropertyField(_ssDurationProperty);
            EditorGUILayout.PropertyField(_onScreenShakeProperty);
        }
        
        serializedObject.ApplyModifiedProperties();
    }
}