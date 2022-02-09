using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace BehaviourTree.Tools
{
    //[CustomEditor(typeof(Blackboard))]
    //[CanEditMultipleObjects]
    //public class BlackboardEditor : Editor
    //{
    //    private SerializedProperty _dataListProperty;
    //    private List<SerializedProperty> _serializedProperties;

    //    private int _listSize;

    //    private void OnEnable()
    //    {
    //        _dataListProperty = serializedObject.FindProperty("debugData");
    //        Debug.Log($"Found property valueData : {(_dataListProperty != null ? "List<Data>" : "NULL")}");
    //    }

    //    public override void OnInspectorGUI()
    //    {
    //        serializedObject.Update();

    //        //_listSize = _dataListProperty.arraySize;
    //        //_listSize = EditorGUILayout.IntField("List Size", _listSize);
    //        //Debug.Log($"debugData.size : {_listSize}");

    //        //if (_listSize != _dataListProperty.arraySize)
    //        //{
    //        //    while (_listSize > _dataListProperty.arraySize)
    //        //    {
    //        //        _dataListProperty.InsertArrayElementAtIndex(_dataListProperty.arraySize);
    //        //    }
    //        //    while (_listSize < _dataListProperty.arraySize)
    //        //    {
    //        //        _dataListProperty.DeleteArrayElementAtIndex(_dataListProperty.arraySize - 1);
    //        //    }
    //        //}

    //        //for (int i = 0; i < _dataListProperty.arraySize; i++)
    //        //{
    //        //    var listRef = _dataListProperty.GetArrayElementAtIndex(i);

    //        //    Debug.Log($"debugData[{i}].type : {listRef.type}");

    //        //    switch (listRef.type)
    //        //    {
    //        //        case "bool":
    //        //            EditorGUILayout.Toggle(listRef.boolValue);
    //        //            break;
    //        //        case "int":
    //        //            EditorGUILayout.IntField(listRef.intValue);
    //        //            break;
    //        //        case "float":
    //        //            EditorGUILayout.FloatField(listRef.floatValue);
    //        //            break;
    //        //        case "string":
    //        //            EditorGUILayout.TextField(listRef.stringValue);
    //        //            break;
    //        //        case "Vector2":
    //        //            EditorGUILayout.Vector2Field("Vec2", listRef.vector2Value);
    //        //            break;
    //        //        case "Vector3":
    //        //            EditorGUILayout.Vector3Field("Vec3", listRef.vector3Value);
    //        //            break;
    //        //        case "Quaternion":
    //        //            EditorGUILayout.Vector4Field("Vec4", listRef.vector4Value);
    //        //            break;
    //        //        case "GameObject":
    //        //            EditorGUILayout.ObjectField(listRef.objectReferenceValue, typeof(Object), true);
    //        //            break;
    //        //        default:
    //        //            Debug.LogWarning($"Don't know the type of this object : {listRef.type}");
    //        //            break;
    //        //    }

    //        //}

            
    //        serializedObject.ApplyModifiedProperties();
    //    }
    //}
}