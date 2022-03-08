using Pharaoh.Gameplay.Components;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(MeleeGear))]
public class MeleeGearEditor : GearEditor
{
    protected SerializedProperty _dataProp;
    protected SerializedProperty _onWeaponThrownProp;
    
    protected override void OnEnable()
    {
        base.OnEnable();
        _dataProp = serializedObject.FindProperty("data");
        _onWeaponThrownProp = serializedObject.FindProperty("onWeaponThrown");
    }

    protected override void DrawSpecificProperties()
    {
        base.DrawSpecificProperties();

        var data = _dataProp.objectReferenceValue as MeleeGearData;

        if (!data || !data.throwable) return;
        EditorGUILayout.PropertyField(_onWeaponThrownProp);
    }
}