// from KasperGameDev/Nested-Scriptable-Objects-Example 
// https://github.com/KasperGameDev/Nested-Scriptable-Objects-Example/blob/main/Assets/Scripts/DamageType.cs

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Alteracia.Patterns.ScriptableObjects
{
    public abstract class NestedScriptableObject : ScriptableObject
    {
        [SerializeField] private string rename;
        [HideInInspector][SerializeField] private RootScriptableObject root;
        
        public void Initialise(RootScriptableObject newRoot) => root = newRoot;

#if UNITY_EDITOR
        
        [ContextMenu("Rename")]
        private void SaveThis()
        {
            if (!root)
            {
                Debug.LogWarning("Can't rename not nested asset");
            }
            Undo.RecordObject(this, $"Rename {this.rename}");
            this.name = rename;
            AssetDatabase.SaveAssets();

            EditorUtility.SetDirty(root);
            EditorUtility.SetDirty(this);
            root.OnUpdateNestedList();
        }

        [ContextMenu("Delete")]
        private void DeleteThis()
        {
            root.Nested.Remove(this);
            Undo.DestroyObjectImmediate(this);
            AssetDatabase.SaveAssets();
            
            EditorUtility.SetDirty(root);
            root.OnUpdateNestedList();
            
//            EditorUtility.SetDirty(this);
        }
#endif
    }
}