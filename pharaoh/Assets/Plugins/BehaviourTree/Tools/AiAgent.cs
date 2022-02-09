using System.Collections;
using UnityEngine;

namespace BehaviourTree.Tools
{
    [RequireComponent(typeof(BehaviourTreeRunner))]
    public class AiAgent : MonoBehaviour
    {
        private BehaviourTreeRunner _runner;

        public float moveSpeed = 5;
        public float fovRange = 6;
        public LayerMask enemyLayerMask;
        public Transform[] waypoints;

        private void Awake()
        {
            _runner = GetComponent<BehaviourTreeRunner>();
        }
    }
}