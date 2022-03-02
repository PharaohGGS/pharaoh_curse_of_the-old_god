using System;
using Pharaoh.Tools;
using Pharaoh.Tools.Debug;
using UnityEngine;
using UnityEngine.Events;

namespace Pharaoh.Gameplay.Components
{
    public abstract class Gear<T> : Gear 
        where T : GearData
    {
        public new T GetData() => _data is T tData ? tData : null;
    }

    [RequireComponent(typeof(Rigidbody2D))]
    public abstract class Gear : MonoBehaviour
    {
        public LayerMask collidingLayers;

        [SerializeField] protected GearData _data;
        public Rigidbody2D rb2D { get; protected set; }
        public Collider2D coll2D { get; protected set; }

        public bool isThrown { get; protected set; }
        public bool isGrounded { get; protected set; }

        public UnityEvent onGroundHit = new UnityEvent();
        [HideInInspector] public UnityEvent<Transform> onSocketAttach = new UnityEvent<Transform>();

        private Transform _parent = null;
        
        protected virtual void Awake()
        {
            rb2D = GetComponent<Rigidbody2D>();
            coll2D = TryGetComponent(out Collider2D coll) ? coll : null;
        }

        protected virtual void OnEnable()
        {
            onSocketAttach?.AddListener(SocketAttach);
        }

        protected virtual void OnDisable()
        {
            onSocketAttach?.RemoveListener(SocketAttach);
        }

        protected virtual void Update()
        {
            if (_parent != transform.parent)
            {
                onSocketAttach?.Invoke(transform.parent);
            }
        }
        
        protected virtual void OnTriggerEnter2D(Collider2D other)
        {
            if (!isThrown || !other.gameObject.IsInLayerMask(collidingLayers)) return;

            coll2D.isTrigger = false;
        }

        protected virtual void OnCollisionEnter2D(Collision2D collision)
        {
            if (!collision.gameObject.IsInLayerMask(collidingLayers)) return;

            isGrounded = true;
            onGroundHit?.Invoke();
            rb2D.angularVelocity = 0f;
            rb2D.velocity = Vector2.zero;
            rb2D.bodyType = RigidbodyType2D.Kinematic;
        }

        protected void SocketAttach(Transform socket)
        {
            if (!rb2D)
            {
                LogHandler.SendMessage($"Can't socket damager.", MessageType.Warning);
                return;
            }

            _parent = socket;
            rb2D.bodyType = socket ? RigidbodyType2D.Dynamic : RigidbodyType2D.Kinematic;

            isGrounded = false;
            isThrown = !socket;
            if (!isThrown && !isGrounded)
            {
                coll2D.isTrigger = true;
            }
        }

        public GearData GetData() => _data;
    }
}