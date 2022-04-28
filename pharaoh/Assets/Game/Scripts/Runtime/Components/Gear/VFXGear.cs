using System;
using System.Collections;
using Pharaoh.Tools;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.VFX;

namespace Pharaoh.Gameplay.Components
{
    [Serializable]
    public class VFXEvent
    {
        public string delay, lifetime;
        public UnityEvent onEventSpawn, onEventAfterDelay, onEventAfterLifetime;
    } 
    
    [RequireComponent(typeof(Gear), typeof(VisualEffect))]
    public class VFXGear : MonoBehaviour
    {
        public float waitForFixedUpdateCount;
        
        [SerializeField]
        private GenericDictionary<string, VFXEvent> vfxEvents = new GenericDictionary<string, VFXEvent>();

        private Gear _gear;
        private VisualEffect _vfx;
        
        private void Awake()
        {
            _gear = GetComponent<Gear>();
            _vfx = GetComponent<VisualEffect>();
        }

        public void CallEvent(string name)
        {
            StartCoroutine(LaunchEvent(vfxEvents[name]));
        }

        private IEnumerator LaunchEvent(VFXEvent vfxEvent)
        {
            if (!_vfx || vfxEvent == null) yield break;
            vfxEvent.onEventSpawn?.Invoke();

            int index = 0;
            while (index < waitForFixedUpdateCount)
            {
                index++;
                yield return new WaitForFixedUpdate();
            }

            var delay = _vfx.HasFloat(vfxEvent.delay) ? _vfx.GetFloat(vfxEvent.delay) : 0.0f;
            yield return new WaitForSeconds(delay);
            vfxEvent.onEventAfterDelay?.Invoke();
            
            var lifetime = _vfx.HasFloat(vfxEvent.lifetime) ? _vfx.GetFloat(vfxEvent.lifetime) : 0.0f;
            yield return new WaitForSeconds(lifetime);
            vfxEvent.onEventAfterLifetime?.Invoke();
        }

        public void ResetCollisionAABox()
        {
            if (!_vfx) return;
            _vfx.SetVector3("CollisionCenter", Vector3.zero);
            _vfx.SetVector3("CollisionSize", Vector3.zero);
        }
        
        public void SetCollisionAABox(Collider2D other)
        {
            if (!_vfx || !other) return;
            _vfx.SetVector3("CollisionCenter", other.bounds.center);
            _vfx.SetVector3("CollisionSize", other.bounds.size);
        }
    }
}