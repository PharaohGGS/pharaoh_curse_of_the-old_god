using UnityEngine;

namespace Pharaoh.Gameplay.Components
{
    public class DetectionComponent : MonoBehaviour
    {
        [field: SerializeField, Range(1, 20)] public float fovRange { get; private set; } = 6;
        [field: SerializeField, Range(0.5f, 10)] public float attackRange { get; private set; } = 1;
        [field: SerializeField] public LayerMask detectionLayer { get; private set; } = 1 << 6;
    }
}