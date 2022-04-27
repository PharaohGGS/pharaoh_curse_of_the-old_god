using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.VFX;

namespace Pharaoh.Gameplay.Components
{
    [RequireComponent(typeof(Gear), typeof(VisualEffect))]
    public class VFXGear : MonoBehaviour
    {
        [SerializeField] private float timeToAppear = 1.0f;
        [SerializeField] private float timeToDisappear = 0.25f;
        [SerializeField] private UnityEvent onVfxSpawned;
        [SerializeField] private UnityEvent onVfxKilled;

        private Gear _gear;
        private VisualEffect _vfx;
        
        private void Awake()
        {
            _gear = GetComponent<Gear>();
            _vfx = GetComponent<VisualEffect>();
            _vfx?.SetFloat("TimeToAppear", timeToAppear);
        }

        public void Spawn()
        {
            StartCoroutine(Spawning());
        }

        private IEnumerator Spawning()
        {
            if (!_vfx) yield break;
            _vfx.SendEvent("Spawn");
            yield return new WaitForSeconds(timeToAppear);
            onVfxSpawned?.Invoke();
        }

        public void Kill()
        {
            StartCoroutine(Killing());
        }
        
        private IEnumerator Killing()
        {
            if (!_vfx) yield break;
            _vfx.SendEvent("Kill");
            yield return new WaitForSeconds(timeToDisappear);
            onVfxKilled?.Invoke();
        }
    }
}