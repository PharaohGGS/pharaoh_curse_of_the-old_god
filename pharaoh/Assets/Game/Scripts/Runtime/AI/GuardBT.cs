using System.Collections.Generic;
using UnityEngine;

namespace Pharaoh.AI
{
    public class GuardBT : ScriptableObject // Tree
    {
        public Transform[] waypoints;

        public float moveSpeed = 2f;
        public float fovRange = 6f;
        public LayerMask enemyLayerMask;

        //protected override Node SetupTree()
        //{
        //    Node root = new Selector(new List<Node>
        //    {
        //        new Sequence(new List<Node>
        //        {
        //            new CheckEnemyInFOVRange(this, transform),
        //            new TaskGoToTarget(this, transform),
        //        }),
        //        new TaskPatrol(this, transform, waypoints),
        //    });
        //    return root;
        //}
    }
}