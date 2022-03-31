#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace BehaviourTree.Tools
{
    public static class BehaviourTreeHelpers
    {
        public static string GetAssetPath(this Object script)
        {
            MonoScript monoScript = null;

            switch (script)
            {
                case ScriptableObject so:
                    monoScript = MonoScript.FromScriptableObject(so);
                    break;
                case MonoBehaviour mb:
                    monoScript = MonoScript.FromMonoBehaviour(mb);
                    break;
            }

            return monoScript != null ? AssetDatabase.GetAssetPath(monoScript) : null;
        }
    }
}
#endif