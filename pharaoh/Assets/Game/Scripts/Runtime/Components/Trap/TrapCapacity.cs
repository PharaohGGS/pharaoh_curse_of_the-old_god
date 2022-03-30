using Pharaoh.Gameplay.Components;
using UnityEngine;
using UnityEngine.Events;

namespace Pharaoh.Gameplay
{
    [RequireComponent(typeof(DetectionComponent))]
    public class TrapCapacity : MonoBehaviour
    {
        [SerializeField] protected UnityEvent<TrapCapacity, TrapBehaviour> onStart;

        private TrapBehaviour _behaviour;
        private DetectionComponent _detection;
        private GameObject _currentTarget;
        public bool isStarted { get; protected set; }

        protected virtual void Awake()
        {
            _detection = GetComponent<DetectionComponent>();
            _behaviour = GetComponentInChildren<TrapBehaviour>();
            isStarted = false;
        }

        protected virtual void Update()
        {
            // check if the current target is different (I mean null here)
            var currentTarget = _detection.GetGameObjectAtIndex(0);
            if (_currentTarget != currentTarget)
            {
                if (currentTarget == null) isStarted = false;
                _currentTarget = currentTarget;
            }

            // don't start trap when there isn't any target
            if (!_currentTarget || isStarted) return;
            isStarted = true;
            onStart?.Invoke(this, _behaviour);
        }
    }
}