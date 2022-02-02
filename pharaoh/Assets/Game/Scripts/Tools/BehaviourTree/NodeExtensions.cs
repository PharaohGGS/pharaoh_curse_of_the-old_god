namespace Pharaoh.Tools.BehaviourTree
{
    public static class NodeExtensions
    {
        public static Node GetRootNode(this Node current)
        {
            Node root = current;
            while (true)
            {
                root = root.parent;
                if (root.parent == null) break;
            }

            return root;
        }
    }
}