using UnityEngine;

namespace Pharaoh.Gameplay.Components
{
    public interface IWeapon
    {
        public void Attack(Transform target);
    }
}