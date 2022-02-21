using Pharaoh.Tools;
using Pharaoh.Tools.Debug;
using UnityEngine;

namespace Pharaoh.Gameplay.Components
{
    public class RangeWeapon : Weapon
    {
        [SerializeField] private float gravity = 9.81f;
        [SerializeField] private float height = 2f;
        public bool isThrown { get; private set; }
        private void FixedUpdate()
        {
            if (isThrown)
            {
                rigidBody.AddForce(Vector3.up * (gravity * -2f));
                //_rigidbody.rotation = Quaternion.LookRotation(_rigidbody.velocity.normalized, Vector3.up);
                if (rigidBody.velocity.normalized.magnitude >= Mathf.Epsilon)
                {
                    rigidBody.rotation = Quaternion.RotateTowards(rigidBody.rotation,
                        Quaternion.LookRotation(rigidBody.velocity.normalized,
                            Vector3.up) /* * Quaternion.Euler(90, 0, 0)*/,
                        rigidBody.velocity.magnitude /** Time.fixedDeltaTime*/);
                }
            }
        }

        protected override void OnTriggerEnter(Collider other)
        {
            if (isThrown && other.gameObject.IsInLayerMask(collidingLayers))
            {
                collider.isTrigger = false;
                return;
            }

            base.OnTriggerEnter(other);
        }

        public void Parenting(Transform parent = null)
        {
            transform.parent = parent;
            isThrown = transform.parent == null;
            isOnGround = false;

            if (!isOnGround && !isThrown)
            {
                collider.isTrigger = true;
            }
        }

        public void Throw(Vector3 target)
        {
            if (!data.canThrow || rigidBody == null)
            {
                LogHandler.SendMessage($"Can't throw this weapon!", MessageType.Warning);
                return;
            }

            var launchData = LaunchData.Calculate(gravity, height, target, rigidBody.position);

            Parenting();
            rigidBody.isKinematic = false;
            rigidBody.useGravity = true;
            rigidBody.velocity = launchData.initialVelocity;
        }
    }
}