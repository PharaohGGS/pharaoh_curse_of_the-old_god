
using Pharaoh.AI.Actions;
using Pharaoh.AI.Decorators;
using Pharaoh.Tools.BehaviourTree.ScriptableObjects;
using Pharaoh.Tools.Debug;
using UnityEngine;

namespace Pharaoh.AI
{
    public class BehaviourTreeRunner : MonoBehaviour
    {
        private BehaviourTree tree;

        // Start is called before the first frame update
        void Start()
        {
            tree = ScriptableObject.CreateInstance<BehaviourTree>();
            var log = ScriptableObject.CreateInstance<LogNode>();
            log.message = "Hello World";
            log.type = MessageType.Log;

            var loop = ScriptableObject.CreateInstance<RepeatNode>();
            loop.child = log;

            tree.root = loop;
        }

        // Update is called once per frame
        void Update()
        {
            tree.Evaluate();
        }
    }
}
