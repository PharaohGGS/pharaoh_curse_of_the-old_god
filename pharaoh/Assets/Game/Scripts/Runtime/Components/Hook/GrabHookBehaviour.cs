using System;

namespace Pharaoh.Gameplay
{
    public class GrabHookBehaviour : HookBehaviour<GrabHookCapacity>
    {
        public override void Release(HookCapacity capacity)
        {
            throw new NotImplementedException();
        }

        public override void Begin(GrabHookCapacity capacity, HookBehaviour<GrabHookCapacity> behaviour)
        {
            throw new NotImplementedException();
        }
    }
}