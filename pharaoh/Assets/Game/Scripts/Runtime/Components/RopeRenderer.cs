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
        public AnimationEventsReceiver animationEventsReceiver;
        public Transform pullingSocketRight;
        public Transform pullingSocketLeft;

        private void Awake()
        {
            _lineRenderer = GetComponent<LineRenderer>();
            _lineRenderer.positionCount = 3;
            _lineRenderer.enabled = false;

            animationEventsReceiver.switchHands += SwitchHands;
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
            if (!_ropeShot || _target == null)
                return;

            _lineRenderer.SetPositions(new Vector3[] { _leftHand.position, _rightHand.position, _target.position });
        }

        public void ShootRope(Transform target)
        {
            _rightHand = pullingSocketRight;
            _leftHand = pullingSocketLeft;
            _target = target;
            _lineRenderer.enabled = true;
            _ropeShot = true;
        }

        public void RetrieveRope()
        {
            _lineRenderer.enabled = false;
            _ropeShot = false;
            _target = null;
        }

        public void SwitchHands()
        {
            Transform temp = _rightHand;
            _rightHand = _leftHand;
            _leftHand = temp;
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
