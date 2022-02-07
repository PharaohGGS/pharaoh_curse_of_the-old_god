using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace BehaviourTree.Tools
{
    [CreateAssetMenu(menuName = "BehaviourTree")]
    public class Tree : ScriptableObject
    {
        public Node root;

        public NodeState treeState = NodeState.Running;

        public List<Node> nodes = new List<Node>();

        public NodeState Evaluate()
        {
            if (root.state == NodeState.Running)
            {
                treeState = root.Evaluate();
            }

            return treeState;
        }

        public Node CreateNode(System.Type type)
        {
            var node = ScriptableObject.CreateInstance(type) as Node;
            node.name = type.Name;
            node.guid = GUID.Generate().ToString();
            nodes.Add(node);

            AssetDatabase.AddObjectToAsset(node, this);
            AssetDatabase.SaveAssets();
            return node;
        }

        public void DeleteNode(Node node)
        {
            nodes.Remove(node);
            AssetDatabase.RemoveObjectFromAsset(node);
            AssetDatabase.SaveAssets();
        }

        public void AddChild(Node parent, Node child)
        {
            if (parent is DecoratorNode decorator)
            {
                decorator.child = child;
            }

            if (parent is RootNode rootNode)
            {
                rootNode.child = child;
            }

            if (parent is CompositeNode composite)
            {
                composite.children.Add(child);
            }

        }
        public void RemoveChild(Node parent, Node child)
        {
            if (parent is DecoratorNode decorator)
            {
                decorator.child = null;
            }

            if (parent is RootNode rootNode)
            {
                rootNode.child = null;
            }

            if (parent is CompositeNode composite)
            {
                composite.children.Remove(child);
            }
        }
        public List<Node> GetChildren(Node parent)
        {
            var children = new List<Node>();
            if (parent is DecoratorNode decorator && decorator.child != null)
            {
                children.Add(decorator.child);
            }

            if (parent is RootNode rootNode)
            {
                children.Add(rootNode.child);
            }

            if (parent is CompositeNode composite)
            {
                return composite.children;
            }

            return children;
        }

        public Tree Clone()
        {
            Tree tree = Instantiate(this);
            tree.root = tree.root.Clone();
            return tree;
        }
    }
}