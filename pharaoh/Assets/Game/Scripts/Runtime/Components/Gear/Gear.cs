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
        [field: SerializeField] protected T data;
        public override GearData GetBaseData() => data;
        public T GetData() => data as T;
    }
    
    public abstract class Gear : MonoBehaviour
    {
        public LayerMask collidingLayers;
        
        public bool isThrown { get; protected set; }
        public bool isGrounded { get; protected set; }

        protected Transform _parent = null;
        protected Rigidbody2D _rigidbody2D = null;

        protected virtual void Awake()
        {
            if (!TryGetComponent(out _rigidbody2D))
            {
                LogHandler.SendMessage($"{name} can't use physics engine", MessageType.Warning);
            }
        }

        protected virtual void Update()
        {
            if (_parent != transform.parent) SocketAttach();
        }
        
        protected virtual void OnTriggerEnter2D(Collider2D other)
        {
            if (!isThrown || !other.gameObject.HasLayer(collidingLayers)) return;

            isGrounded = true;

            if (_rigidbody2D)
            {
                _rigidbody2D.isKinematic = true;
                _rigidbody2D.Sleep();
            }
        }

        protected virtual void SocketAttach()
        {
            _parent = transform.parent;
            isGrounded = false;
            isThrown = !_parent;
            
            if (!_rigidbody2D) return;
            _rigidbody2D.isKinematic = transform.parent;
            if (transform.parent) _rigidbody2D.WakeUp();
        }

        public abstract GearData GetBaseData();
        
        public bool TryGetData<T>(out T data)
        {
            data = (T)(object)null;
            if (GetBaseData() is not T tData) return false;

            data = tData;
            return true;
        }
    }
}