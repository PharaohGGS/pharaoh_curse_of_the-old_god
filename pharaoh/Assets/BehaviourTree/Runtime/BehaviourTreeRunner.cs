
using UnityEngine;
using Tree = BehaviourTree.Tools.Tree;

namespace BehaviourTree.Runtime
{
    public class BehaviourTreeRunner : MonoBehaviour
    {
        [SerializeField] private Tree tree;

        // Start is called before the first frame update
        void Start()
        {
            tree = tree.Clone();
        }

        // Update is called once per frame
        void Update()
        {
            tree.Evaluate();
        }
    }
}
