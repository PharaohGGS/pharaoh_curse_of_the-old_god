using UnityEngine;
using Pharaoh.Tools;

namespace Pharaoh.Gameplay.Components
{
    [CreateAssetMenu(fileName = "DetectionData", menuName = "Detection/Data", order = 52)]
    public class DetectionData : ScriptableObject
    {
        public LayerMask layer;
    }

    public static class DetectionDataHelper
    {
        public static bool HasLayer(this DetectionData data, int layer) => data.layer == (data.layer | 1 << layer);

        public static bool[] HasLayers(this DetectionData[] datas)
        {
            var hasLayers = new bool[32];
            foreach (var data in datas)
            {
                for (int i = 0; i < 32; i++)
                {
                    hasLayers[i] = data.HasLayer(i);
                }
            }

            return hasLayers;
        }

        public static int[] GetLayerIndexes(this DetectionData[] datas)
        {
            var hasLayers = new System.Collections.Generic.List<int>();
            foreach (var data in datas)
            {
                for (int i = 0; i < 32; i++)
                {
                    if (data.HasLayer(i))
                    {
                        hasLayers.Add(i);
                    }
                }
            }

            return hasLayers.ToArray();
        }

        public static int GetLayerIndex(this DetectionData data)
        {
            for (int i = 0; i < 32; i++) if (data.HasLayer(i)) return i;
            return -1;
        }

        public static LayerMask CombineLayers(this DetectionData[] datas)
        {
            if (datas is not {Length: > 0}) return default;

            LayerMask mask = default;
            for (var i = 0; i < datas.Length; i++)
            {
                var data = datas[i];
                if (i == 0) mask = data.layer;
                else mask |= data.layer;
            }

            return mask;
        }
    }
}