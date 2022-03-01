using UnityEngine;

namespace Pharaoh.Gameplay.Components
{
    public class Actor : MonoBehaviour
    {
        public Collider[] colliders { get; protected set; }
        public Rigidbody rigidBody { get; protected set; }

        protected virtual void Awake()
        {
            colliders = GetComponents<Collider>();
            rigidBody = GetComponent<Rigidbody>();
        }
    }
}