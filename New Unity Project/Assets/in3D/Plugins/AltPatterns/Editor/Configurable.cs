using UnityEditor;
using UnityEngine;

namespace Alteracia.Patterns.Editor
{
    [CanEditMultipleObjects]
    public abstract class Configurable<T0, T1> : UnityEditor.Editor where T0 : ScriptableObjects.ConfigurableController<T0, T1>
        where T1 : ScriptableObject
    {
        protected T0 instance;

        private void OnEnable()
        {
            instance = target as T0;
            this.Initiate();
        }

        protected abstract void Initiate();

        public override void OnInspectorGUI()
        {
            OnInspectorCustomGUI();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Create config"))
                instance.CreateConfiguration();
            if (instance.Configuration)
            {
                if (GUILayout.Button("Read from json"))
                    instance.ReadConfigurationFromJson();
                if (GUILayout.Button("Save to json"))
                    instance.SaveConfigurationToJson();
            }
            GUILayout.EndHorizontal();
        }

        protected virtual void OnInspectorCustomGUI()
        {
            base.OnInspectorGUI();
        }
    }
}
