using UnityEditor;

namespace Pharaoh.AI.Actions
{

    [CanEditMultipleObjects]
    [CustomEditor(typeof(TaskDelay))]
    public class TaskDelayEditor : Editor
    {
        protected SerializedProperty _useGearProp;
        protected SerializedProperty _delayProp;
        protected SerializedProperty _gearDataProp;

        protected virtual void OnEnable()
        {
            _useGearProp = serializedObject.FindProperty("useGearDelay");
            _delayProp = serializedObject.FindProperty("delay");
            _gearDataProp = serializedObject.FindProperty("gearData");
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
            EditorGUILayout.PropertyField(_useGearProp?.boolValue == false ? _delayProp : _gearDataProp);
        }
    }

}
