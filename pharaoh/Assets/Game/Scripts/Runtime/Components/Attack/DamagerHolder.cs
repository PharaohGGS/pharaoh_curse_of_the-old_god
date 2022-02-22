using Pharaoh.Tools.Debug;
using UnityEngine;

namespace Pharaoh.Gameplay.Components
{
    public class DamagerHolder : MonoBehaviour
    {
        [SerializeField] private DamagerData data;

        public Damager damager { get; private set; }

        public float timeAfterPickingWeapon = 1f;

        private void Awake()
        {
            if (data == null) return;

            damager = GameObject.Instantiate(data.prefab, transform.position, transform.rotation, transform);
        }
    }
}