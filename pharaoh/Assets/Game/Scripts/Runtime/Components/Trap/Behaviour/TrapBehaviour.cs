using System;
using Pharaoh.Gameplay.Components;
using UnityEngine;

namespace Pharaoh.Gameplay
{
    public abstract class TrapBehaviour : MonoBehaviour { }

    public abstract class TrapBehaviour<T> : TrapBehaviour
        where T : TrapData
    {
        [SerializeField] protected T data;
        
        public abstract void TrapStart(TrapCapacity capacity, TrapBehaviour behaviour);
    }
}