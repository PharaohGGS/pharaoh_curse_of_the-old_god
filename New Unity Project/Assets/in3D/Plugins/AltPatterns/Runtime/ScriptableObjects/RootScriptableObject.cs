// from KasperGameDev/Nested-Scriptable-Objects-Example 
// https://github.com/KasperGameDev/Nested-Scriptable-Objects-Example/blob/main/Assets/Scripts/ContainerDamageType.cs
// 

using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Alteracia.Patterns.ScriptableObjects
{
    public abstract class RootScriptableObject : ScriptableObject
    {
        [HideInInspector][SerializeField] private List<NestedScriptableObject> nested = new List<NestedScriptableObject>();
        [SerializeField] private NestedScriptableObject toAdd;

        public List<NestedScriptableObject> Nested
        {
            get => nested;
            set => nested = value;
        }

#if UNITY_EDITOR

        [ContextMenu("Add new", false, 100)]
        private void AddNew()
        {
            if (!toAdd)
            {
                Debug.LogWarning("No object in \"To Add\" field");
                return;
            }
            AddNested(toAdd);
            
            Debug.LogWarning($"Asset {toAdd.name} destroyed! Please restore references.");
            AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(toAdd));
           
            toAdd = null;
        }
        
        protected void AddNested<T>() where T : NestedScriptableObject
        {
            var newNested = ScriptableObject.CreateInstance<T>();
            newNested.name = typeof(T).Name;
            AddNewNested(newNested);
        }
        
        public void AddNested<T>(T toCopy) where T : NestedScriptableObject
        {
            var newNested = ScriptableObject.Instantiate(toCopy);
            newNested.name = toCopy.name;
            AddNewNested(newNested);
        }
        
        private void AddNewNested<T>(T newNested) where T : NestedScriptableObject
        {
            newNested.Initialise(this);
            nested.Add(newNested);

            AssetDatabase.AddObjectToAsset(newNested, this);
            AssetDatabase.SaveAssets();

            EditorUtility.SetDirty(this);
            EditorUtility.SetDirty(newNested);
            
            OnUpdateNestedList();
        }

        public virtual void OnUpdateNestedList() { }

        [ContextMenu("Delete all", false, 99)]
        protected void DeleteAll()
        {
            for (int i = nested.Count; i-- > 0;)
            {
                NestedScriptableObject tmp = nested[i];

                nested.Remove(tmp);
                Undo.DestroyObjectImmediate(tmp);
            }

            AssetDatabase.SaveAssets();
        }

#endif
    }
}