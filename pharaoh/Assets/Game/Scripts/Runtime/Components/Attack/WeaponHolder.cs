using Pharaoh.Tools.Debug;
using UnityEngine;

namespace Pharaoh.Gameplay.Components
{
    public class WeaponHolder : MonoBehaviour
    {
        [SerializeField] private Gear gearPrefab;

        public Gear Gear { get; private set; }
        public DamagerData data { get; private set; }

        private void Awake()
        {
            if (data == null) return;

            if (transform.childCount <= 0)
            {
                Gear = GameObject.Instantiate(gearPrefab, transform.position, Quaternion.identity, transform);
                data = Gear.data;
                return;
            }

            foreach (Transform child in transform)
            {
                if (!child.TryGetComponent(out Gear w)) continue;

                Gear = w;
                data = Gear.TryGetComponent(out DamagerData d) ? d : null;
                break;
            }
        }
    }
}