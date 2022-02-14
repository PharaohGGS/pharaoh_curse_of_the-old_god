
using UnityEngine;
using UnityEngine.Events;

namespace Pharaoh.Gameplay.Components.Events
{
    public class GameEventListener : MonoBehaviour
    {
        [SerializeField] private GameEvent gameEvent; 
        [SerializeField] private UnityEvent response; 

        private void OnEnable() 
        {
            gameEvent?.RegisterListener(this);
        }

        private void OnDisable() 
        {
            gameEvent?.UnregisterListener(this);
        }

        public void OnEventRaised() 
        {
            response?.Invoke();
        }
    }

}