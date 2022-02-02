using System.Collections.Generic;
using Pharaoh.Tools.BehaviourTree;
using UnityEngine;
using Tree = Pharaoh.Tools.BehaviourTree.Tree;

namespace Pharaoh.Gameplay.AI
{
    public class GuardBT : Tree
    {
        public Transform[] waypoints;

        public float moveSpeed = 2f;
        public float fovRange = 6f;
        public LayerMask enemyLayerMask;

        protected override Node SetupTree()
        {
            Node root = new Selector<GuardBT>(this, new List<Node>
            {
                new Sequence<GuardBT>(this, new List<Node>
                {
                    new CheckEnemyInFOVRange(this, transform),
                    new TaskGoToTarget(this, transform),
                }),
                new TaskPatrol(this, transform, waypoints),
            });
            return root;
        }
    }
}