using UnityEngine;

namespace Alteracia.Patterns.ScriptableObjects
{
    [CreateAssetMenu(fileName = "ScriptableEventsRegistry", menuName = "AltEvents/ScriptableEventsRegistry", order = 1)]
    public class ScriptableEventsRegistry : RootScriptableObject
    {
       
#if UNITY_EDITOR

        void Awake()
        {
            OnUpdateNestedList();
        }
        
        public override void OnUpdateNestedList()
        {
            if (!ScriptableEventsRegistryBuss.Registries.Contains(this))
                ScriptableEventsRegistryBuss.Registries.Add(this);
            if (ScriptableEventsRegistryBuss.Instance) 
                ScriptableEventsRegistryBuss.Instance.AddRegistry(this);
        }
        
        [ContextMenu("Add Int Object Event")]
        private void AddIntObjectEvent() => AddNested<Events.IntObjectEvent>();
        [ContextMenu("Add Int Two State Event")]
        private void AddIntTwoStateEvent() => AddNested<Events.IntTwoStateEvent>();
        
        [ContextMenu("Add Float Object Event")]
        private void AddFloatObjectEvent() => AddNested<Events.FloatObjectEvent>();
        [ContextMenu("Add Float Two State Event")]
        private void AddFloatTwoStateEvent() => AddNested<Events.FloatTwoStateEvent>();
        
        [ContextMenu("Add String Object Event")]
        private void AddStringObjectEvent() => AddNested<Events.StringObjectEvent>();
        [ContextMenu("Add String Two State Event")]
        private void AddStringTwoStateEvent() => AddNested<Events.StringTwoStateEvent>();

        [ContextMenu("Add Vector3 Object Event")]
        private void AddVector3ObjectEvent() => AddNested<Events.Vector3ObjectEvent>();
        [ContextMenu("Add Vector3 Two State Event")]
        private void AddVector3TwoStateEvent() => AddNested<Events.Vector3TwoStateEvent>();
        
        [ContextMenu("Add Quaternion Object Event")]
        private void AddQuaternionObjectEvent() => AddNested<Events.QuaternionObjectEvent>();
        [ContextMenu("Add Quaternion Two State Event")]
        private void AddQuaternionTwoStateEvent() => AddNested<Events.QuaternionTwoStateEvent>();
        
        [ContextMenu("Add ScriptableObject Object Event")]
        private void AddScriptableObjectObjectEvent() => AddNested<Events.ScriptableObjectObjectEvent>();
        [ContextMenu("Add ScriptableObject Two State Event")]
        private void AddScriptableObjectTwoStateEvent() => AddNested<Events.ScriptableObjectTwoStateEvent>();
        
        [ContextMenu("Add Mesh Object Event")]
        private void AddMeshObjectEvent() => AddNested<Events.MeshObjectEvent>();
        [ContextMenu("Add Mesh Two State Event")]
        private void AddMeshTwoStateEvent() => AddNested<Events.MeshTwoStateEvent>();

        
        [ContextMenu("Add Material Object Event")]
        private void AddMaterialObjectEvent() => AddNested<Events.MaterialObjectEvent>();
        [ContextMenu("Add Material Two State Event")]
        private void AddMaterialTwoStateEvent() => AddNested<Events.MaterialTwoStateEvent>();
        
        
        [ContextMenu("Add Transform Object Event")]
        private void AddTransformObjectEvent() => AddNested<Events.TransformObjectEvent>();
        [ContextMenu("Add Transform Component Event")]
        private void AddTransformComponentEvent() => AddNested<Events.TransformComponentEvent>();
        [ContextMenu("Add Transform Two State Event")]
        private void AddTransformTwoStateEvent() => AddNested<Events.TransformTwoStateEvent>();
        
        [ContextMenu("Add Renderer Object Event")]
        private void AddRendererObjectEvent() => AddNested<Events.MeshRendererObjectEvent>();
        [ContextMenu("Add Renderer Component Event")]
        private void AddRendererComponentEvent() => AddNested<Events.MeshRendererComponentEvent>();
        [ContextMenu("Add Renderer Two State Event")]
        private void AddRendererTwoStateEvent() => AddNested<Events.MeshRendererTwoStateEvent>();

#endif
        
    }
}