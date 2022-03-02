using System;
using Pharaoh.Tools;
using Pharaoh.Tools.Debug;
using UnityEngine;
using UnityEngine.Events;

namespace Pharaoh.Gameplay.Components
{
    public class Weapon : Damager
    {
        public LayerMask collidingLayers;
        public bool isThrown { get; protected set; }
        public bool isOnGround { get; protected set; }

        public UnityEvent onWeaponThrown = new UnityEvent();
        public UnityEvent onGroundHit = new UnityEvent();
        public UnityEvent<Transform> onSocketAttach = new UnityEvent<Transform>();

        private Transform _parent = null;

        private void OnEnable()
        {
            onSocketAttach?.AddListener(SocketAttach);
        }

        private void OnDisable()
        {
            onSocketAttach?.RemoveListener(SocketAttach);
        }

        public void Update()
        {
            if (_parent != transform.parent)
            {
                onSocketAttach?.Invoke(transform.parent);
            }
        }

        private void SocketAttach(Transform socket)
        {
            if (!rb2D)
            {
                LogHandler.SendMessage($"Can't socket damager.", MessageType.Warning);
                return;
            }

            _parent = socket;
            rb2D.bodyType = socket ? RigidbodyType2D.Kinematic : RigidbodyType2D.Dynamic;

            isOnGround = false;
            isThrown = !socket;
            if (!isThrown && !isOnGround)
            {
                coll2D.isTrigger = true;
            }
        }

        protected override void OnTriggerEnter2D(Collider2D other)
        {
            if (isThrown && other.gameObject.IsInLayerMask(collidingLayers))
            {
                coll2D.isTrigger = false;
                return;
            }

            base.OnTriggerEnter2D(other);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.IsInLayerMask(collidingLayers))
            {
                isOnGround = true;
                onGroundHit?.Invoke();
                rb2D.angularVelocity = 0f;
                rb2D.velocity = Vector2.zero;
                rb2D.bodyType = RigidbodyType2D.Kinematic;
            }
        }

        public void Throw(Damager damager)
        {
            if (!data.throwable || damager != this || !TryGetComponent(out Ballistic ballistic)) return;

            transform.parent = null;
            onWeaponThrown?.Invoke();
        }
    }
}