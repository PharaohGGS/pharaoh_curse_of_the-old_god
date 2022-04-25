using System;
using Pharaoh.Gameplay.Components;
using Pharaoh.Tools;
using UnityEngine;
using UnityEngine.Events;

namespace Pharaoh.Gameplay
{
    [RequireComponent(typeof(TrapDetection))]
    public class TrapCapacity : MonoBehaviour
    {
        [SerializeField] private TrapBehaviour _behaviour;
        [SerializeField] private LayerMask whatIsTarget;

        private TrapDetection _trapDetection;

        protected virtual void Awake()
        {
            _trapDetection = GetComponent<TrapDetection>();
            if (!_behaviour) _behaviour = GetComponentInChildren<TrapBehaviour>();
        }

        private void OnEnable()
        {
            if (!_trapDetection) return;
            _trapDetection.onOverlapEnter += OnOverlapEnter;
            _trapDetection.onOverlapExit += OnOverlapExit;
        }
        private void OnDisable()
        {
            if (!_trapDetection) return;
            _trapDetection.onOverlapEnter -= OnOverlapEnter;
            _trapDetection.onOverlapExit -= OnOverlapExit;
        }

        private void OnOverlapEnter(Collider2D other)
        {
            if (!_behaviour || _trapDetection.overlappedCount <= 0) return;
            _behaviour.Enable();
        }

        private void OnOverlapExit(Collider2D other)
        {
            if (!_behaviour || _trapDetection.overlappedCount > 0) return;
            _behaviour.Disable();
        }
    }
}