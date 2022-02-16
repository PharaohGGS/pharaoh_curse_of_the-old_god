using System;
using BehaviourTree.Tools;
using Pharaoh.Gameplay.Components;
using Pharaoh.Gameplay.Sets;
using UnityEngine;

namespace Pharaoh.AI
{
    public class EnemyAgent : AiAgent
    {
        public EnemyAgentRuntimeSet enemyAgentRuntimeSet;
        public Weapon weapon;

        public HealthComponent health { get; private set; }
        public MovementComponent movement { get; private set; }
        public DetectionComponent detection { get; private set; }
        
        private void OnEnable()
        {
            enemyAgentRuntimeSet?.Add(this);
            
            if (health == null && TryGetComponent(out HealthComponent hlth)) health = hlth;
            if (movement == null && TryGetComponent(out MovementComponent move)) movement = move;
            if (detection == null && TryGetComponent(out DetectionComponent dtct)) detection = dtct;
        }

        private void OnDisable()
        {
            enemyAgentRuntimeSet?.Remove(this);
        }
    }
}