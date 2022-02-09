using System.Collections.Generic;
using UnityEngine;

namespace Pharaoh.AI
{
    public class GuardBT : ScriptableObject // BTree
    {
        public Transform[] waypoints;

        public float moveSpeed = 2f;
        public float fovRange = 6f;
        public LayerMask enemyLayerMask;

        //protected override BNode SetupTree()
        //{
        //    BNode root = new Selector(new List<BNode>
        //    {
        //        new Sequence(new List<BNode>
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