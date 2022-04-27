using System;
using System.Collections;
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
        [SerializeField] protected Transform socket;
        [SerializeField] protected Vector3 overrideRotation = Vector3.zero;
        [SerializeField] protected UnityEvent onSocketAttach;

        public bool isThrown { get; protected set; }
        public bool isGrounded { get; protected set; }
        
        protected Rigidbody2D _rigidbody2D = null;

        protected virtual void Awake()
        {
            if (!TryGetComponent(out _rigidbody2D))
            {
                LogHandler.SendMessage($"{name} can't use physics engine", MessageType.Warning);
            }

            SocketAttach(true);
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

        public virtual void SocketAttach(bool attach)
        {
            isGrounded = false;
            isThrown = !attach;
            
            if (!_rigidbody2D) return;
            _rigidbody2D.isKinematic = attach;

            if (!attach || !socket)
            {
                transform.parent = null;
            }
            else
            {
                transform.parent = socket;
                transform.localPosition = Vector3.zero;
                transform.localRotation = Quaternion.Euler(overrideRotation);
                if (TryGetComponent(out Collider2D col))
                {
                    col.enabled = false;
                }
                _rigidbody2D.WakeUp();
                onSocketAttach?.Invoke();
            }
        }

        public abstract GearData GetBaseData();

        public T GetData<T>() where T : GearData => GetBaseData() as T;

        public bool TryGetData<T>(out T data)
        {
            data = (T)(object)null;
            if (GetBaseData() is not T tData) return false;

            data = tData;
            return true;
        }

        public virtual float GetRate()
        {
            var data = GetBaseData();
            return !data ? 0f : data.rate;
        }
    }
}