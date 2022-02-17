using System;
using BehaviourTree.Tools;
using Game.Scripts.Runtime.Components;
using Pharaoh.Gameplay.Components;
using Pharaoh.Gameplay.Sets;
using UnityEngine;

namespace Pharaoh.AI
{
    public class EnemyPawn : Pawn
    {
        public EnemyPawnRuntimeSet enemyPawnRuntimeSet;
        public WeaponHolder holder;

        public DetectionComponent detection { get; private set; }

        protected override void OnEnable()
        {
            base.OnEnable();
            enemyPawnRuntimeSet?.Add(this);
            
            if (detection == null && TryGetComponent(out DetectionComponent dtct)) detection = dtct;

            holder = GetComponentInChildren<WeaponHolder>();
        }

        private void OnDisable()
        {
            enemyPawnRuntimeSet?.Remove(this);
        }

        public void Attack()
        {
            // play animation of attack or throw weapon
        }
    }
}