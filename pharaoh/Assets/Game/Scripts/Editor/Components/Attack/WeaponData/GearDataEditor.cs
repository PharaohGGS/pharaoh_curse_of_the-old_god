using Pharaoh.Gameplay.Components;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(GearData), true)]
public class GearDataEditor : Editor
{
    protected SerializedProperty _canAttack;
    protected SerializedProperty _delayProp;
    protected SerializedProperty _rateProp;
    protected SerializedProperty _rangeProp;
    
    protected virtual void OnEnable()
    {
        _canAttack = serializedObject.FindProperty("canAttack");
        _delayProp = serializedObject.FindProperty("delay");
        _rateProp = serializedObject.FindProperty("rate");
        _rangeProp = serializedObject.FindProperty("range");
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
        if (_canAttack?.boolValue != true) return;
        EditorGUILayout.PropertyField(_delayProp);
        EditorGUILayout.PropertyField(_rateProp);
        EditorGUILayout.PropertyField(_rangeProp);
    }
}