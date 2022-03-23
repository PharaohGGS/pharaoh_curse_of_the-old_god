using System;
using UnityEditor;
using UnityEngine;

namespace BehaviourTree.Editor
{
    public static class BehaviourTreeEditorHelper
    {
        public static void RemoveElement(this SerializedProperty list, int index)
        {
            if (list == null)
                throw new ArgumentNullException ();
         
            if(!list.isArray)
                throw new ArgumentException("Property is not an array");
         
            if(index<0||index>=list.arraySize)
                throw new IndexOutOfRangeException ();
         
            list.GetArrayElementAtIndex(index).SetPropertyValue(null);
            list.DeleteArrayElementAtIndex(index);
         
            list.serializedObject.ApplyModifiedProperties();
        }
 
        public static void SetPropertyValue(this SerializedProperty property,object value)
        {
            switch(property.propertyType)
            {
             
                case SerializedPropertyType.AnimationCurve:
                    property.animationCurveValue = value as AnimationCurve;
                    break;
 
                case SerializedPropertyType.ArraySize:
                    property.intValue = Convert.ToInt32(value);
                    break;
 
                case SerializedPropertyType.Boolean:
                    property.boolValue = Convert.ToBoolean(value);
                    break;
 
                case SerializedPropertyType.Bounds:
                    property.boundsValue = (value==null)
                        ? new Bounds()
                        : (Bounds)value;
                    break;
 
                case SerializedPropertyType.Character:
                    property.intValue = Convert.ToInt32(value);
                    break;
 
                case SerializedPropertyType.Color:
                    property.colorValue = (value==null)
                        ? new Color()
                        : (Color)value;
                    break;
 
                case SerializedPropertyType.Float:
                    property.floatValue = Convert.ToSingle(value);
                    break;
 
                case SerializedPropertyType.Integer:
                    property.intValue = Convert.ToInt32(value);
                    break;
 
                case SerializedPropertyType.LayerMask:
                    property.intValue = (value is LayerMask)?((LayerMask)value).value: Convert.ToInt32(value);
                    break;
 
                case SerializedPropertyType.ObjectReference:
                    property.objectReferenceValue = value as UnityEngine.Object;
                    break;
 
                case SerializedPropertyType.Quaternion:
                    property.quaternionValue = (value==null)
                        ? Quaternion.identity
                        :(Quaternion)value;
                    break;
 
                case SerializedPropertyType.Rect:
                    property.rectValue = (value==null)
                        ? new Rect()
                        :(Rect)value;
                    break;
 
                case SerializedPropertyType.String:
                    property.stringValue = value as string;
                    break;
 
                case SerializedPropertyType.Vector2:
                    property.vector2Value = (value==null)
                        ? Vector2.zero
                        :(Vector2)value;
                    break;
 
                case SerializedPropertyType.Vector3:
                    property.vector3Value = (value==null)
                        ? Vector3.zero
                        :(Vector3)value;
                    break;
 
                case SerializedPropertyType.Vector4:
                    property.vector4Value = (value==null)
                        ? Vector4.zero
                        :(Vector4)value;
                    break;
 
            }
        }

        public static int RemoveEmptyArrayElements(this SerializedProperty list)
        {
            var elementsRemoved = 0;
            if (list.serializedObject != null)
            {
                for (int i = list.arraySize - 1; i >= 0; i--)
                {
                    if (list.GetArrayElementAtIndex(i).objectReferenceValue == null)
                    {
                        list.RemoveElement(i);
                        elementsRemoved++;
                    }
                }
            }

            return elementsRemoved;
        }

    }
}