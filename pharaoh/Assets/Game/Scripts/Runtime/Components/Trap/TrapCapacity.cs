using Pharaoh.Gameplay.Components;
using UnityEngine;
using UnityEngine.Events;

namespace Pharaoh.Gameplay
{
    [RequireComponent(typeof(DetectionComponent))]
    public class TrapCapacity : MonoBehaviour
    {
        [SerializeField] private TrapBehaviour _behaviour;

        private DetectionComponent _detection;

        protected virtual void Awake()
        {
            _detection = GetComponent<DetectionComponent>();
            if (!_behaviour) _behaviour = GetComponentInChildren<TrapBehaviour>();
        }

        protected virtual void Update()
        {
            // check if the current target is different (I mean null here)
            // don't start trap when there isn't any target or already processing
            if (_behaviour.isStarted || !_detection.TryGetByIndex(0, out GameObject target) || !target) return;
            _behaviour.Activate(target);
        }
    }
}