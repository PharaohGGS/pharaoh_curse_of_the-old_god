using UnityEngine;

namespace Pharaoh.Gameplay
{
    public abstract class TrapData : ScriptableObject
    {
        public bool isTimed;
        public bool oneTimeDelay;
        public float delay;
        public float lifeTime;
        public float timeOut;
    }
}