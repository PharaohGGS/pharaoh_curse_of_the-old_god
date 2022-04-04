
using UnityEngine;

namespace BehaviourTree.Tools
{
    public class BehaviourTreeRunner : MonoBehaviour
    {
        public BTree tree;

        // Start is called before the first frame update
        void Start()
        {
            tree = tree.Clone();
            if (TryGetComponent(out AiAgent agent))
            {
                tree.Bind(agent);
            }
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            tree?.Evaluate();
        }
    }
}
