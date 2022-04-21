using System;
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
        [SerializeField] private Animator animator;

        [field: SerializeField, Range(1f, 100f)] public float moveSpeed { get; private set; } = 5;
        [field: SerializeField, Range(0.01f, 100.0f)] public float closeDistance { get; private set; } = 0.01f;
        [field: SerializeField, Range(1f, 100f)] public float fleeDistance { get; private set; } = 2;
        [field: SerializeField, Range(1f, 100f)] public float timeBetweenWaypoints { get; private set; } = 2;
        [field: SerializeField] public Transform waypointHolder { get; private set; }

        public bool isStunned { get; private set; }
        private Rigidbody2D _rigidbody;

        private Vector2 _smoothMovement;
        private Vector2 _smoothVelocity;

        private bool _canMove;

        private void Awake()
        {
            _canMove = true;
            isStunned = false;
            
            if (!TryGetComponent(out _rigidbody))
            {
                LogHandler.SendMessage("No rigidbody on this ai", MessageType.Warning);
            }

            if (!animator)
            {
                LogHandler.SendMessage("No animator on this ai", MessageType.Warning);
            }
        }

        private void Update()
        {   
            if (animator?.runtimeAnimatorController != null)
            {
                animator?.SetFloat("Horizontal Speed", _rigidbody.velocity.x);
            }
        }

        public void HitStun(Damager damager)
        {
            if (!damager) return;

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

        public void LockMovement(bool value)
        {
            _canMove = !value;
        }

        public void Move(Vector3 target)
        {
            if (!_rigidbody || !_canMove) return;

            var direction = ((Vector2)target - _rigidbody.position).normalized;

            _smoothVelocity = Vector2.zero;
            _smoothMovement = Vector2.SmoothDamp(_smoothMovement, direction, ref _smoothVelocity, 0.03f);
            _rigidbody.velocity = new Vector2(_smoothMovement.x * moveSpeed,  _rigidbody.velocity.y);
        }

        public void LookAt(Vector3 target)
        {
            target.z = transform.position.z;
            target.y = transform.position.y;
            transform.LookAt2D(target);
        }

        private IEnumerator WaitStunTime(float time)
        {
            if (!_rigidbody) yield break;

            LockMovement(true);
            //_rigidbody.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;

            yield return new WaitForSeconds(time);
            
            isStunned = false;
            LockMovement(false);
            _rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
    }
}