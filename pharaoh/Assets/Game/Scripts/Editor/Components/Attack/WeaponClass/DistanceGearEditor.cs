using Pharaoh.Gameplay.Components;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(DistanceGear))]
public class DistanceGearEditor : GearEditor
{
    protected SerializedProperty _dataProp;
    protected SerializedProperty _onGearShootProp;
    
    protected override void OnEnable()
    {
        base.OnEnable();
        _dataProp = serializedObject.FindProperty("data");
        _onGearShootProp = serializedObject.FindProperty("onGearShoot");
    }

    protected override void DrawSpecificProperties()
    {
        base.DrawSpecificProperties();

        var data = _dataProp.objectReferenceValue as DistanceGearData;

        if (!data) return;
        EditorGUILayout.PropertyField(_onGearShootProp);
    }
}