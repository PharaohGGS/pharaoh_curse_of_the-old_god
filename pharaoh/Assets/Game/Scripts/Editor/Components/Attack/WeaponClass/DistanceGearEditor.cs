using Pharaoh.Gameplay.Components;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(DistanceGear))]
public class DistanceGearEditor : Editor
{
    protected SerializedProperty _dataProp;
    protected SerializedProperty _onGearShootProp;

    protected void OnEnable()
    {
        _dataProp = serializedObject.FindProperty("data");
        _onGearShootProp = serializedObject.FindProperty("onGearShoot");
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        serializedObject.Update();

        var data = _dataProp.objectReferenceValue as DistanceGearData;

        if (data)
        {
            EditorGUILayout.PropertyField(_onGearShootProp);
        }

        serializedObject.ApplyModifiedProperties();
    }
}