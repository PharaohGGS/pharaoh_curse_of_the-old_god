using Pharaoh.Tools.Debug;
using UnityEngine;

namespace Pharaoh.Gameplay.Components
{
    public class GearHolder : MonoBehaviour
    {
        [SerializeField] private Gear gearPrefab;

        public Gear gear { get; private set; }

        private void Awake()
        {
            if (transform.childCount <= 0 && gearPrefab)
            {
                gear = GameObject.Instantiate(gearPrefab, transform.position, Quaternion.identity, transform);
                return;
            }

            foreach (Transform child in transform)
            {
                if (!child.TryGetComponent(out Gear w)) continue;

                gear = w;
                break;
            }
        }
    }
}