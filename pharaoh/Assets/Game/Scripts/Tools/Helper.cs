using UnityEditor;
using UnityEngine;

namespace Pharaoh.Tools
{
    public static class Helper
    {
        public static bool IsSharingSameInstance(this UnityEngine.MonoBehaviour objectToCompare,
            UnityEngine.GameObject comparisonObject)
        {
            return objectToCompare.gameObject.GetInstanceID() == comparisonObject.GetInstanceID();
        }

        public static bool IsInLayerMask(this GameObject go, LayerMask mask) => (mask.value & (1 << go.layer)) > 0;
    }
}