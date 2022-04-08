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

    }
}
