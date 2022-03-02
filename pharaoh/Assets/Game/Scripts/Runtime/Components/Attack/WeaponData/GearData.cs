using UnityEngine;

namespace Pharaoh.Gameplay.Components
{
    [CreateAssetMenu(fileName = "New GearData", menuName = "Weapons/", order = 0)]
    public abstract class GearData : ScriptableObject
    {
        public Damager prefab;
        public string description;
    }
}