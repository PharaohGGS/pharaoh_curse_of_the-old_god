using System;
using BehaviourTree.Tools;
using UnityEngine;

namespace Pharaoh.AI.Actions
{
    public class CheckWait : ActionNode
    {
        [SerializeField] private WaitType _waitType = WaitType.Null;
        
        protected override NodeState OnUpdate()
        {
            bool isWaiting = ((EnemyAgent) agent).IsWaiting(_waitType);
            return isWaiting ? NodeState.Running : NodeState.Failure;
        }
    }
}