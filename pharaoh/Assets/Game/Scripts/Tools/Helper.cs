namespace Pharaoh.Tools
{
    public static class Helper
    {
        public static bool IsSharingSameInstance(this UnityEngine.MonoBehaviour objectToCompare,
            UnityEngine.GameObject comparisonObject)
        {
            return objectToCompare.gameObject.GetInstanceID() == comparisonObject.GetInstanceID();
        }
    }
}