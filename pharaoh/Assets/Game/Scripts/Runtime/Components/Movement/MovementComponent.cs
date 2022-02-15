using UnityEngine;

namespace Pharaoh.Gameplay.Components
{
    public class MovementComponent : MonoBehaviour
    {
        [field: SerializeField, Range(1, 100)] public float moveSpeed { get; private set; } = 5;
        [field: SerializeField] public Transform[] waypoints { get; private set; }
    }
}