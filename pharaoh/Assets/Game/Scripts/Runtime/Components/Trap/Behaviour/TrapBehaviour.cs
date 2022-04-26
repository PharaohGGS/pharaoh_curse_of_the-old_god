using System;
using Pharaoh.Gameplay.Components;
using UnityEngine;

namespace Pharaoh.Gameplay
{
    public abstract class TrapBehaviour : MonoBehaviour
    {
        protected bool _isStarted;
        
        public abstract void Enable();
        public abstract void Disable();
        public abstract void Reset();
    }

    public abstract class TrapBehaviour<T> : TrapBehaviour
        where T : TrapData
    {
        [SerializeField] protected T data;

    }
}