using System;
using UnityEngine;

namespace Pharaoh.Gameplay
{
    public class GrappleHookBehaviour : HookBehaviour<GrappleHookCapacity>
    {
        [SerializeField, Tooltip("target position the player is going to be at the end")]
        private Transform finalMoveTarget;
        
        private readonly WaitForFixedUpdate _waitForFixedUpdate = new WaitForFixedUpdate();
        private Coroutine _moveToCoroutine;
        
        public override void Release(HookCapacity capacity)
        {
            if (_moveToCoroutine != null) StopCoroutine(_moveToCoroutine);
        }

        public override void Begin(GrappleHookCapacity capacity, HookBehaviour<GrappleHookCapacity> behaviour)
        {
            if (behaviour != this) return;
            _moveToCoroutine = StartCoroutine(Grapple(capacity));
        }

        private System.Collections.IEnumerator Grapple(GrappleHookCapacity capacity)
        {
            if (!capacity) yield break;
            
            hookIndicator?.SetActive(false);

            float speed = capacity.grappleSpeed;
            AnimationCurve curve = capacity.smoothCurve;
            Vector2 startPosition = capacity.transform.position;

            float maxDistance = Vector2.Distance(startPosition, finalMoveTarget.position);
            float timeToTravel = maxDistance / speed;
            float currentTime = 0f;

            while (currentTime <= timeToTravel)
            {
                capacity.transform.position =  Vector2.Lerp(startPosition, finalMoveTarget.position, curve.Evaluate(currentTime / timeToTravel));
                currentTime = Mathf.MoveTowards(currentTime, timeToTravel, Time.fixedDeltaTime * speed);
                yield return _waitForFixedUpdate;
            }
            
            capacity.onGrappleEnd?.Invoke(capacity, this);
        }
    }
}