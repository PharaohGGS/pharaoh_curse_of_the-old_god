using System;
using Pharaoh.Tools;
using UnityEngine;
using UnityEngine.Events;

namespace Pharaoh.Gameplay.Components
{
    public abstract class Weapon : Damager
    {
        public bool isOnGround { get; private set; }
        
        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.IsInLayerMask(collidingLayers))
            {
                isOnGround = true;
                rigidBody.velocity = rigidBody.angularVelocity = Vector3.zero;
                rigidBody.isKinematic = true;
                rigidBody.useGravity = false;
            }
        }
    }
}