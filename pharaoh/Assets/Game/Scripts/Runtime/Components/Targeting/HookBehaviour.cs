using System;
using UnityEngine;

namespace Pharaoh.Gameplay
{
    public abstract class HookBehaviour : MonoBehaviour
    {
        public abstract void Fix(HookTargeting grabber, GameObject target);
        public abstract void Release(HookTargeting grabber, GameObject target);

        public virtual void BestTargetSelect(HookTargeting grabber, GameObject target)
        {
            if (target != gameObject) return;
        }
    }
}