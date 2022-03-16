using System;
using UnityEngine;

namespace Pharaoh.Gameplay
{
    public class MoveToHookBehaviour : HookBehaviour
    {
        [SerializeField, Tooltip("target position the player is going to be at the end")]
        private Transform finalMoveTarget;
        [SerializeField, Tooltip("Obstacle layers")] 
        private LayerMask whatIsObstacle = 1 << 6;
        [SerializeField, Tooltip("offset mesuring distance between player and hook point")] 
        private float offsetHook;
        [SerializeField, Tooltip("Hook gameobject speed to this object")]
        private float moveSpeed;
        [SerializeField, Tooltip("Hook movement curve from a to b")]
        private AnimationCurve smoothCurve;
        
        public UnityEngine.Events.UnityEvent onMovementEnd = new UnityEngine.Events.UnityEvent();

        private readonly WaitForFixedUpdate _waitForFixedUpdate = new WaitForFixedUpdate();
        private Coroutine _moveToCoroutine;
        
        public override void OnHook(Transform grabber, Transform target)
        {
            if (target != transform) return;
            _moveToCoroutine = StartCoroutine(MoveTo(grabber));
        }

        public override void OnRelease(Transform grabber, Transform target)
        {
            if (target != transform || _moveToCoroutine == null) return;
            StopCoroutine(_moveToCoroutine);
        }

        private System.Collections.IEnumerator MoveTo(Transform grabber)
        {
            if (!grabber || !grabber.TryGetComponent(out Rigidbody2D rb)) yield break;
            
            Vector2 startPosition = rb.position;
            float current = 0f;

            while (Vector2.Distance(finalMoveTarget.position, rb.position) > offsetHook)
            {
                Vector2 direction = (Vector2)finalMoveTarget.position - rb.position;
                var hit2Ds = Physics2D.RaycastAll(rb.position, 
                    direction.normalized, direction.magnitude, whatIsObstacle);

                if (hit2Ds.Length > 0) OnRelease(grabber, transform);

                current = Mathf.MoveTowards(current, 1f, moveSpeed * Time.fixedDeltaTime);
                rb.MovePosition(Vector2.Lerp(startPosition, finalMoveTarget.position, smoothCurve.Evaluate(current)));
                yield return _waitForFixedUpdate;
            }
            
            onMovementEnd?.Invoke();
        }
    }
}