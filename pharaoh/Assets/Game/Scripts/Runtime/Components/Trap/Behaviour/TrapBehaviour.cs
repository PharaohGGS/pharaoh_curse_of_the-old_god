using System;
using Pharaoh.Gameplay.Components;
using UnityEngine;

namespace Pharaoh.Gameplay
{
    public abstract class TrapBehaviour : MonoBehaviour
    {
        protected GameObject _currentTarget;

        public bool isStarted { get; protected set; }
        
        public abstract void Activate(GameObject target);
    }

    public abstract class TrapBehaviour<T> : TrapBehaviour
        where T : TrapData
    {
        [SerializeField] protected T data;
    }
}