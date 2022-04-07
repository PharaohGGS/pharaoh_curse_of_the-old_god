using UnityEditor;

namespace Pharaoh.GameEvents
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(AbstractGameEventListener<,>), true)]
    public class AbstractGameEventListenerEditor : UnityEditor.Editor
    {
        protected SerializedProperty _gameEventProp;
        protected SerializedProperty _responseProp;

        private void OnEnable()
        {
            _gameEventProp = serializedObject.FindProperty("gameEvent");
            _responseProp = serializedObject.FindProperty("response");
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            serializedObject.Update();

            if (_gameEventProp?.objectReferenceValue != null) EditorGUILayout.PropertyField(_responseProp);

            serializedObject.ApplyModifiedProperties();
        }
    }

    [CanEditMultipleObjects]
    [CustomEditor(typeof(AbstractGameEventListener<,,>), true)]
    public class AbstractGameEventListenerOneEditor : AbstractGameEventListenerEditor { }

    [CanEditMultipleObjects]
    [CustomEditor(typeof(AbstractGameEventListener<,,,>), true)]
    public class AbstractGameEventListenerTwoEditor : AbstractGameEventListenerEditor { }

    [CanEditMultipleObjects]
    [CustomEditor(typeof(AbstractGameEventListener<,,,,>), true)]
    public class AbstractGameEventListenerThreeEditor : AbstractGameEventListenerEditor { }
}