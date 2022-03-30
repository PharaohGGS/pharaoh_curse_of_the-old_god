using System;
using Pharaoh.Gameplay.Components.Movement;
using Pharaoh.Tools.Debug;
using UnityEngine;

namespace Pharaoh.Gameplay
{
    public class GrappleHookBehaviour : HookBehaviour
    {
        [SerializeField, Tooltip("target position the player is going to be at the end")]
        private Transform finalMoveTarget;
        
        private readonly WaitForFixedUpdate _waitForFixedUpdate = new WaitForFixedUpdate();
        private Coroutine _moveToCoroutine;
        private bool _triggerSet = false;
        
        protected void OnEnable()
        {
            inputs.hookGrappleStartedEvent += OnHookGrappleStart;
            inputs.movePerformedEvent += OnMove;
            inputs.jumpStartedEvent += OnJump;
            inputs.dashStartedEvent += OnDash;
        }
        
        protected void OnDisable()
        {
            inputs.hookGrappleStartedEvent -= OnHookGrappleStart;
            inputs.movePerformedEvent -= OnMove;
            inputs.jumpStartedEvent -= OnJump;
            inputs.dashStartedEvent -= OnDash;
        }

        private void FixedUpdate()
        {
            if (!isCurrentTarget || !_hook) return;

            // Cancel grapple when encounter an obstacle 
            Vector2 hookPosition = _hook.transform.position;
            Vector2 direction = (Vector2)transform.position - hookPosition;
            int size = Physics2D.RaycastNonAlloc(hookPosition, direction.normalized, 
                _hits, direction.magnitude, _hook.whatIsObstacle);
            if (size > 0)
            {
                Release();
            }
        }

        private void OnHookGrappleStart()
        {
            if (isCurrentTarget) Release();
        }

        private void OnMove(Vector2 axis)
        {
            if (!isCurrentTarget) return;
            if (axis.y >= -0.8f) return;
            Release();
        }

        private void OnJump()
        {
            if (!isCurrentTarget) return;
            Release();
        }

        private void OnDash()
        {
            if (!isCurrentTarget) return;
            Release();
        }

        public override void Release()
        {
            base.Release();
            if (_moveToCoroutine != null) StopCoroutine(_moveToCoroutine);
        }

        public override void Interact(HookCapacity hook, GameObject target)
        {
            base.Interact(hook, target);
            if (!isCurrentTarget) return;

            if (hookIndicator) hookIndicator.SetActive(false);
            _moveToCoroutine = StartCoroutine(Grapple());
        }

        //private bool dd = false;

        private System.Collections.IEnumerator Grapple()
        {
            if (!_hook) yield break;

            float speed = _hook.grappleData.speed;
            AnimationCurve curve = _hook.grappleData.curve;
            Vector2 startPosition = _hook.transform.position;

            float maxDistance = Vector2.Distance(startPosition, finalMoveTarget.position);
            float timeToTravel = maxDistance / speed;
            float currentTime = 0f;
            
            while (currentTime < timeToTravel)
            {
                nextPosition =  Vector2.Lerp(startPosition, finalMoveTarget.position, curve.Evaluate(currentTime / timeToTravel));
                currentTime = Mathf.MoveTowards(currentTime, timeToTravel, Time.fixedDeltaTime * speed);
                //FindObjectOfType<PlayerMovement>().animator.SetTrigger("Will Be Hooked");

                if ((currentTime / timeToTravel) >= 0.9f && !_triggerSet)
                {
                    Debug.Log((currentTime / timeToTravel));
                    FindObjectOfType<PlayerMovement>().animator.SetTrigger("Will Be Hooked");
                    _triggerSet = true;
                }
                Perform();

                yield return _waitForFixedUpdate;
            }
            _triggerSet = false;
            End();
        }
    }
}