using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BehaviourTree.Tools;
using Mono.Cecil;
using Pharaoh.Tools;
using Pharaoh.Tools.Debug;
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

        private Collider2D[] _colliders;
        private Rigidbody2D _rigidbody;

        private void Awake()
        {
            _colliders = GetComponents<Collider2D>();
            if (!TryGetComponent(out _rigidbody))
            {
                LogHandler.SendMessage("No rigidbody on this ai", MessageType.Warning);
            }
        }

        public void HitStun(Damager damager, Collider2D other)
        {
            if (_colliders.Length <= 0 || _colliders.All(col => col != other)) return;

            var data = damager.stunData;
            isStunned = data != null;
            StartCoroutine(WaitStunTime(data.time));
        }

        public void DashStun(GameObject target, StunData data)
        {
            if (target != gameObject) return;

            isStunned = data != null;
            StartCoroutine(WaitStunTime(data.time));
        }

        public void Move(Vector3 target)
        {
            if (!_rigidbody) return;
            var direction = (Vector2)target - _rigidbody.position;
            _rigidbody.AddForce(direction.normalized * moveSpeed/* * Time.fixedDeltaTime*/, ForceMode2D.Force);
        }

        public void LookAt(Vector3 target)
        {
            target.z = transform.position.z;
            transform.LookAt2D(target);
        }

        private IEnumerator WaitStunTime(float time)
        {
            if (!_rigidbody) yield break;

            _rigidbody.velocity = Vector2.zero;
            _rigidbody.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
            
            yield return new WaitForSeconds(time);
            
            isStunned = false;
            _rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
    }
}