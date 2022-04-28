using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BehaviourTree.Tools;
using Pharaoh.AI.Actions;
using Pharaoh.Gameplay.Components;
using Pharaoh.Sets;
using Pharaoh.Tools;
using Pharaoh.Tools.Debug;
using UnityEngine;

namespace Pharaoh.AI
{
    [Serializable]
    public enum WaitType : int
    {
        Null = 0,
        Movement = 1,
        Attack = 2,
    }

    [Serializable]
    public struct WaitStruct
    {
        public float start;
        public float seconds;

        public WaitStruct(float start, float seconds)
        {
            this.start = start;
            this.seconds = seconds;
        }
    }

    public class EnemyAgent : AiAgent
    {
        private readonly List<WaitType> _waitTypes = new List<WaitType>();
        private readonly List<WaitType> _endWaitings = new List<WaitType>();
        private readonly Dictionary<WaitType, WaitStruct> _waitDico = new Dictionary<WaitType, WaitStruct>();
        
        public bool IsWaiting(WaitType type) => _waitTypes.Contains(type);

        public void StartWait(WaitType type, float waitTime)
        {
            if (_waitTypes.Contains(type))
            {
                //Debug.LogWarning($"Can't wait more than once for {type}");
                _waitDico[type] = new WaitStruct(Time.time, waitTime);
                return;
            }

            _waitTypes.Add(type);
            if (!_waitDico.TryAdd(type, new WaitStruct(Time.time, waitTime)))
            {
                Debug.LogWarning($"{type} already exist in dictionnay");
            }
        }

        private void Update()
        {
            foreach (var type in _waitTypes)
            {
                if (!_waitDico.TryGetValue(type, out var wait)) continue;
                float timeSince = Time.time - wait.start;

                if (timeSince < wait.seconds) continue;
                _endWaitings.Add(type);
                _waitDico.Remove(type);
            }

            foreach (var type in _endWaitings.Where(type => _waitTypes.Contains(type)))
            {
                _waitTypes.Remove(type);
            }

            _endWaitings.Clear();
        }
    }
}