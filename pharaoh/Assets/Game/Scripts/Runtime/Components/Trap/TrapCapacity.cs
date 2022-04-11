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
            _behaviour.Activate(_detection.GetByIndex(0));
        }
    }
}