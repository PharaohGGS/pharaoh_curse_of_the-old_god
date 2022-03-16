using System;
using UnityEngine;

namespace Pharaoh.Gameplay
{
    public abstract class HookBehaviour : MonoBehaviour
    {
        public abstract void OnHook(Transform grabber, Transform target);
        public abstract void OnRelease(Transform grabber, Transform target);
    }
}