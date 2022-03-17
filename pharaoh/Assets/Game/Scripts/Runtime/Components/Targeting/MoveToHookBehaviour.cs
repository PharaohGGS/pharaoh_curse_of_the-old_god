using System;
using UnityEngine;

namespace Pharaoh.Gameplay
{
    public class MoveToHookBehaviour : HookBehaviour
    {
        [SerializeField, Tooltip("FX indicator for the best target selected")] 
        private GameObject hookIndicator;
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
        
        public UnityEngine.Events.UnityEvent<HookTargeting, GameObject> onMovementEnd = new UnityEngine.Events.UnityEvent<HookTargeting, GameObject>();
        public UnityEngine.Events.UnityEvent<HookTargeting, GameObject> onHookReleaseEarly = new UnityEngine.Events.UnityEvent<HookTargeting, GameObject>();

        private readonly WaitForFixedUpdate _waitForFixedUpdate = new WaitForFixedUpdate();
        private Coroutine _moveToCoroutine;

        private void Awake()
        {
            hookIndicator?.SetActive(false);
        }

        public override void Fix(HookTargeting grabber, GameObject target)
        {
            if (target != gameObject) return;
            
            if (grabber.TryGetComponent(out Rigidbody2D rb))
            {
                rb.velocity = Vector2.zero;
                //rb.bodyType = RigidbodyType2D.Kinematic;
            }

            _moveToCoroutine = StartCoroutine(MoveTo(grabber));
        }

        public override void Release(HookTargeting grabber, GameObject target)
        {
            if (!grabber || target != gameObject) return;
            if (_moveToCoroutine != null) StopCoroutine(_moveToCoroutine);

            if (!grabber.TryGetComponent(out Rigidbody2D rb)) return;
            rb.bodyType = RigidbodyType2D.Dynamic;
        }

        public override void BestTargetSelect(HookTargeting grabber, GameObject target)
        {
            if (!hookIndicator) return;
            hookIndicator.SetActive(target == gameObject);
        }

        private System.Collections.IEnumerator MoveTo(HookTargeting grabber)
        {
            if (!grabber || !grabber.TryGetComponent(out Rigidbody2D rb)) yield break;
            
            hookIndicator?.SetActive(false);
            Vector2 startPosition = rb.position;
            float current = 0f;

            while (Vector2.Distance(finalMoveTarget.position, rb.position) > offsetHook)
            {
                Vector2 direction = (Vector2)finalMoveTarget.position - rb.position;
                var hit2Ds = Physics2D.RaycastAll(rb.position, 
                    direction.normalized, direction.magnitude, whatIsObstacle);

                if (hit2Ds.Length > 0) onHookReleaseEarly?.Invoke(grabber, gameObject);

                current = Mathf.MoveTowards(current, 1f, moveSpeed * Time.fixedDeltaTime);
                rb.MovePosition(Vector2.Lerp(startPosition, finalMoveTarget.position, smoothCurve.Evaluate(current)));
                yield return _waitForFixedUpdate;
            }
            
            onMovementEnd?.Invoke(grabber, gameObject);
        }
    }
}