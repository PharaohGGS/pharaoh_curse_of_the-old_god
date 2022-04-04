using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BehaviourTree.Tools;
using UnityEngine;

namespace Pharaoh.Gameplay.Components
{
    public class AiMovement : MonoBehaviour
    {
        
        [field: SerializeField, Range(1f, 100f)] public float moveSpeed { get; private set; } = 5;
        [field: SerializeField, Range(0.01f, 100.0f)] public float closeDistance { get; private set; } = 0.01f;
        [field: SerializeField, Range(1f, 100f)] public float fleeDistance { get; private set; } = 2;
        [field: SerializeField, Range(1f, 100f)] public float timeBetweenWaypoints { get; private set; } = 2;
        [field: SerializeField] public Transform waypointHolder { get; private set; }

        public bool isStunned { get; private set; }
        public StunData lastStunData { get; private set; }

        private Collider2D[] _colliders;

        private void Awake()
        {
            _colliders = GetComponents<Collider2D>();
        }

        public void HitStun(Damager damager, Collider2D other)
        {
            if (_colliders.Length <= 0 || _colliders.All(col => col != other)) return;

            var data = damager.stunData;
            isStunned = data != null;
            lastStunData = data;
        }

        public void DashStun(GameObject target, StunData data)
        {
            if ((!isStunned && target != gameObject) || (isStunned && target)) return;

            isStunned = data != null;
            lastStunData = data;
        }

        public void EndStun()
        {
            isStunned = false;
            lastStunData = null;
        }
    }
}