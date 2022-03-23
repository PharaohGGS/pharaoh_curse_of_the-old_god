using UnityEngine;
using UnityEngine.Events;

namespace Pharaoh.Gameplay
{
    [CreateAssetMenu(fileName = "HookBehaviourEvents", menuName = "HookData/Events", order = 0)]
    public class HookBehaviourEvents : ScriptableObject
    {
        [Tooltip("Event when the behaviour start")]
        public event UnityAction<HookBehaviour> started;
        [Tooltip("Event when the behaviour is performing an action")]
        public event UnityAction<HookBehaviour> performed;
        [Tooltip("Event when the behaviour ended")]
        public event UnityAction<HookBehaviour> ended;
        [Tooltip("Event when the behaviour is released")]
        public event UnityAction<HookBehaviour> released;
        
        public void Started(HookBehaviour behaviour) => started?.Invoke(behaviour);
        public void Performed(HookBehaviour behaviour) => performed?.Invoke(behaviour);
        public void Ended(HookBehaviour behaviour) => ended?.Invoke(behaviour);
        public void Released(HookBehaviour behaviour) => released?.Invoke(behaviour);
    }
}