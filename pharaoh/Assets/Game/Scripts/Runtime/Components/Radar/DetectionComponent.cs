using System;
using UnityEngine;

namespace Pharaoh.Gameplay.Components
{
    public class DetectionComponent : MonoBehaviour
    {
        [field: SerializeField, Range(1, 20)] public float fovRange { get; private set; } = 6;
        [field: SerializeField, Range(1, 20)] public float pickupRange { get; private set; } = 1;
        [field: SerializeField] public LayerMask detectionLayer { get; private set; } = 1 << 6;
        [field: SerializeField] public LayerMask weaponLayer { get; private set; } = 1 << 6;

#if UNITY_EDITOR

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, fovRange);
        }


#endif
    }
}