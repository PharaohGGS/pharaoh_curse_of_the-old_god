using Pharaoh.Gameplay;

namespace Pharaoh.GameEvents
{
    public class HookGameEvent<T0, T1> : AbstractGameEvent<T0, T1> 
        where T0 : HookCapacity 
        where T1 : HookBehaviour<T0>
    {
    }
}