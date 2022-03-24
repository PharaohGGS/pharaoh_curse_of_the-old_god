using BehaviourTree.Tools;
using Pharaoh.Sets;

namespace Pharaoh.AI
{
    public class TrapAgent : AiAgent
    {
        public TrapAgentRuntimeSet trapAgentRuntimeSet;
        
        private void OnEnable()
        {
            trapAgentRuntimeSet?.Add(this);
        }

        private void OnDisable()
        {
            trapAgentRuntimeSet?.Remove(this);
        }
    }
}