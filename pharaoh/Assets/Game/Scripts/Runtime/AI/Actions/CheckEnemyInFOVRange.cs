using System;
using BehaviourTree.Tools;
using UnityEngine;

namespace Pharaoh.AI.Actions
{
    public class CheckEnemyInFOVRange : ActionNode
    {
        private Transform _transform;
        private Collider[] _colliders;

        private void Awake()
        {

            _colliders = new Collider[8];
        }

        protected override NodeState OnUpdate()
        {
            //var t = GetData("target");

            //    if (t != null)
            //    {
            //        state = NodeState.SUCCESS;
            //        return state;
            //    }

            //    int size = Physics.OverlapSphereNonAlloc(
            //        _transform.position, tree.fovRange, 
            //        _colliders, tree.enemyLayerMask);

            //    if (size > 0)
            //    {
            //        var root = this.GetRootNode();
            //        root.SetData("target", _colliders[0].transform);
            //        state = NodeState.SUCCESS;
            //        return state;
            //    }

            //    state = NodeState.FAILURE;
            return state;
        }
    }
}