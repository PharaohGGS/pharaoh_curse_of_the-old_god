using Pharaoh.Gameplay;
using UnityEngine.Events;

namespace Pharaoh.GameEvents
{
    public class HookGameEventListener<T0, T1> : AbstractGameEventListener<T0, T1, HookGameEvent<T0, T1>, UnityEvent<T0, T1>>
        where T0 : HookCapacity
        where T1 : HookBehaviour<T0>
    {
    }
}