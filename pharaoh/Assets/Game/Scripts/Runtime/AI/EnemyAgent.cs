using System;
using BehaviourTree.Tools;
using Pharaoh.Gameplay.Sets;
using UnityEngine;

namespace Pharaoh.AI
{
    public class EnemyAgent : AiAgent
    {
        public EnemyAgentRuntimeSet enemyAgentRuntimeSet;

        public float moveSpeed = 5;
        public float fovRange = 6;
        public LayerMask enemyLayerMask;
        public Transform[] waypoints;

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