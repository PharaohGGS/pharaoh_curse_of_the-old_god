using Pharaoh.Gameplay.Components;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(MeleeGear))]
public class MeleeGearEditor : Editor
{
    protected SerializedProperty _dataProp;
    protected SerializedProperty _onWeaponThrownProp;
    
    protected void OnEnable()
    {
        _dataProp = serializedObject.FindProperty("data");
        _onWeaponThrownProp = serializedObject.FindProperty("onWeaponThrown");
    }
    
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        serializedObject.Update();
        
        var data = _dataProp.objectReferenceValue as MeleeGearData;

        if (data && data.throwable)
        {
            EditorGUILayout.PropertyField(_onWeaponThrownProp);
        }

        serializedObject.ApplyModifiedProperties();
    }
}