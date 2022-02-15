using System.Collections;
using UnityEngine;

namespace BehaviourTree.Tools
{
    [RequireComponent(typeof(BehaviourTreeRunner))]
    public class AiAgent : MonoBehaviour
    {
        private BehaviourTreeRunner _runner;

        private void Awake()
        {
            _runner = GetComponent<BehaviourTreeRunner>();
        }
    }
}