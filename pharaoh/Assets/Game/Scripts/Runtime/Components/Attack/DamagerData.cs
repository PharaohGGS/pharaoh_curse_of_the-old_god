using System.Collections;
using UnityEngine;

namespace Pharaoh.Gameplay.Components
{
    [CreateAssetMenu(fileName = "DamagerData", menuName = "Damagers/Damager", order = 53)]
    public class DamagerData : ScriptableObject
    {
        public float damage;
    }
}