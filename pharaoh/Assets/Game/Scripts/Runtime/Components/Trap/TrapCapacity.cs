using Pharaoh.Gameplay.Components;
using UnityEngine;
using UnityEngine.Events;

namespace Pharaoh.Gameplay
{
    [RequireComponent(typeof(TrapDetection))]
    public class TrapCapacity : MonoBehaviour
    {
        [SerializeField] private TrapBehaviour _behaviour;

        private TrapDetection _trapDetection;

        protected virtual void Awake()
        {
            _trapDetection = GetComponent<TrapDetection>();
            if (!_behaviour) _behaviour = GetComponentInChildren<TrapBehaviour>();
        }

        protected virtual void Update()
        {
            if (!_behaviour || _behaviour.isStarted) return;
            _behaviour.Activate(_trapDetection.GetByIndex(0));
        }
    }
}