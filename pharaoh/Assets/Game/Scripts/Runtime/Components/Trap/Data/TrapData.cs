using UnityEngine;

namespace Pharaoh.Gameplay
{
    public abstract class TrapData : ScriptableObject
    {
        public float delay;
        public float lifeTime;
        public float timeOut;
    }
}