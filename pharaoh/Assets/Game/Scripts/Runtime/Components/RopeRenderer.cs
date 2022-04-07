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

        public Transform leftHandSocket;
        public Transform rightHandSocket;

        private void Awake()
        {
            _lineRenderer = GetComponent<LineRenderer>();
            _lineRenderer.positionCount = 3;
            _lineRenderer.enabled = false;
        }

        private void Update()
        {
            if (!_ropeShot || _target == null)
                return;

            _lineRenderer.SetPositions(new Vector3[] { leftHandSocket.position, rightHandSocket.position, _target.position });
        }

        public void ShootRope(Transform target)
        {
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

    }
}
