
using UnityEngine;

namespace BehaviourTree.Tools
{
    public class BehaviourTreeRunner : MonoBehaviour
    {
        public Tree tree;

        // Start is called before the first frame update
        void Start()
        {
            tree = tree.Clone();
        }

        // Update is called once per frame
        void Update()
        {
            tree?.Evaluate();
        }
    }
}
