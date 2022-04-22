using System;
using Pharaoh.Gameplay;
using UnityEngine;

namespace Pharaoh.Gameplay
{
    [RequireComponent(typeof(LineRenderer))]
    public class RopeRenderer : MonoBehaviour
    {

        private LineRenderer _lineRenderer;
        private bool _ropeShot = false;
        private Transform _target;
        private Transform _rightHand;
        private Transform _leftHand;

        [SerializeField, Header("Hook Events")]
        private HookBehaviourEvents hookEvents;

        [Header("Animation Event")]
        public Transform pullingSocketRight;
        public Transform pullingSocketLeft;

        private void Awake()
        {
            _lineRenderer = GetComponent<LineRenderer>();
            _lineRenderer.positionCount = 3;
            _lineRenderer.enabled = false;
        }

        private void OnEnable()
        {
            HookAddListener();
        }

        private void OnDisable()
        {
            HookRemoveListener();
        }

        private void Update()
        {
            if (!_ropeShot || _target == null || _lineRenderer.positionCount == 0) return;

            _lineRenderer.SetPositions(new Vector3[] { _leftHand.position, _rightHand.position, _target.position });
        }

        public void ShootRope(Transform target)
        {
            _rightHand = pullingSocketRight;
            _leftHand = pullingSocketLeft;
            _target = target;
            _lineRenderer.enabled = true;
            _lineRenderer.positionCount = 3;
            _ropeShot = true;
        }

        public void RetrieveRope()
        {
            _rightHand = null;
            _leftHand = null;
            _target = null;
            _lineRenderer.enabled = false;
            _lineRenderer.positionCount = 0;
            _ropeShot = false;
        }

        public void SwitchHands()
        {
            (_rightHand, _leftHand) = (_leftHand, _rightHand);
        }

        #region Hook

        private void HookAddListener()
        {
            if (!hookEvents) return;
            hookEvents.started += OnHookStarted;
            hookEvents.ended += OnHookEnded;
            hookEvents.released += OnHookReleased;
        }

        private void HookRemoveListener()
        {
            if (!hookEvents) return;
            hookEvents.started -= OnHookStarted;
            hookEvents.ended -= OnHookEnded;
            hookEvents.released -= OnHookReleased;
        }

        private void OnHookStarted(HookBehaviour behaviour)
        {
            if (!behaviour.isCurrentTarget) return;

            switch (behaviour)
            {
                case GrappleHookBehaviour grapple:
                    ShootRope(behaviour.transform);
                    break;
                case PullHookBehaviour pull:
                    ShootRope(behaviour.transform);
                    break;
                case SnatchHookBehaviour snatch:
                    ShootRope(behaviour.transform);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(behaviour));
            }
        }

        private void OnHookEnded(HookBehaviour behaviour)
        {
            if (!behaviour.isCurrentTarget) return;

            switch (behaviour)
            {
                case GrappleHookBehaviour grapple:
                    RetrieveRope();
                    break;
                case PullHookBehaviour pull:
                    RetrieveRope();
                    break;
                case SnatchHookBehaviour snatch:
                    RetrieveRope();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(behaviour));
            }
        }

        private void OnHookReleased(HookBehaviour behaviour)
        {
            if (!behaviour.isCurrentTarget) return;
            
            switch (behaviour)
            {
                case GrappleHookBehaviour grapple:
                    RetrieveRope();
                    break;
                case PullHookBehaviour pull:
                    RetrieveRope();
                    break;
                case SnatchHookBehaviour snatch:
                    RetrieveRope();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(behaviour));
            }
        }

        #endregion

    }
}
