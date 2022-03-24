using BehaviourTree.Tools;
using Pharaoh.Gameplay.Components;
using Pharaoh.Sets;
using Pharaoh.Tools.Debug;
using UnityEngine;

namespace Pharaoh.AI
{
    public class EnemyAgent : AiAgent
    {
        public EnemyAgentRuntimeSet enemyAgentRuntimeSet;
        
        private void OnEnable()
        {
            enemyAgentRuntimeSet?.Add(this);
        }

        private void OnDisable()
        {
            enemyAgentRuntimeSet?.Remove(this);
        }
    }
}