using BehaviourTree.Tools;
using Pharaoh.Gameplay.Components;
using Pharaoh.Sets;
using Pharaoh.Tools.Debug;
using UnityEngine;

namespace Pharaoh.AI
{
    public class EnemyAgent : AiAgent
    {
        public EnemyPawnRuntimeSet enemyPawnRuntimeSet;
        
        private void OnEnable()
        {
            enemyPawnRuntimeSet?.Add(this);
        }

        private void OnDisable()
        {
            enemyPawnRuntimeSet?.Remove(this);
        }
    }
}