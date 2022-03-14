using System.Collections.Generic;
using UnityEngine;

namespace Pharaoh.Gameplay.Components
{
    public class MovementComponent : MonoBehaviour
    {
        [field: SerializeField, Range(1, 100)] public float moveSpeed { get; private set; } = 5;
        [field: SerializeField, Range(1, 100)] public float closeDistance { get; private set; } = 0.01f;
        [field: SerializeField, Range(1, 100)] public float fleeDistance { get; private set; } = 2;
        [field: SerializeField, Range(1, 100)] public float timeBetweenWaypoints { get; private set; } = 2;
        [field: SerializeField] public List<Transform> waypoints { get; private set; } = new List<Transform>();
    }
}