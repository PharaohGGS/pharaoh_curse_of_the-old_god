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

        public Blackboard blackboard = new Blackboard();

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

            Undo.RecordObject(this, "Behaviour Tree (Create node)");
            nodes.Add(node);

            if (!Application.isPlaying)
            {
                AssetDatabase.AddObjectToAsset(node, this);
            }
            
            Undo.RegisterCreatedObjectUndo(node, "Behaviour Tree (Create node)");
            AssetDatabase.SaveAssets();
            return node;
        }

        public void DeleteNode(Node node)
        {
            Undo.RecordObject(this, "Behaviour Tree (Delete node)");
            nodes.Remove(node);
            //AssetDatabase.RemoveObjectFromAsset(node);
            Undo.DestroyObjectImmediate(node);
            AssetDatabase.SaveAssets();
        }

        public void AddChild(Node parent, Node child)
        {
            if (parent is DecoratorNode decorator)
            {
                Undo.RecordObject(decorator, "Behaviour Tree (Add Child)");
                decorator.child = child;
                EditorUtility.SetDirty(decorator);
            }

            if (parent is RootNode rootNode)
            {
                Undo.RecordObject(rootNode, "Behaviour Tree (Add Child)");
                rootNode.child = child;
                EditorUtility.SetDirty(rootNode);
            }

            if (parent is CompositeNode composite)
            {
                Undo.RecordObject(composite, "Behaviour Tree (Add Child)");
                composite.children.Add(child);
                EditorUtility.SetDirty(composite);
            }

        }
        public void RemoveChild(Node parent, Node child)
        {
            if (parent is DecoratorNode decorator)
            {
                Undo.RecordObject(decorator, "Behaviour Tree (Remove Child)");
                decorator.child = null;
                EditorUtility.SetDirty(decorator);
            }

            if (parent is RootNode rootNode)
            {
                Undo.RecordObject(rootNode, "Behaviour Tree (Remove Child)");
                rootNode.child = null;
                EditorUtility.SetDirty(rootNode);
            }

            if (parent is CompositeNode composite)
            {
                Undo.RecordObject(composite, "Behaviour Tree (Remove Child)");
                composite.children.Remove(child);
                EditorUtility.SetDirty(composite);
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

        public void Traverse(Node node, System.Action<Node> visiter)
        {
            if (node)
            {
                visiter?.Invoke(node);
                var children = GetChildren(node);
                children.ForEach((n) => Traverse(n, visiter));
            }
        }

        public Tree Clone()
        {
            Tree tree = Instantiate(this);
            tree.root = tree.root.Clone();
            tree.nodes = new List<Node>();
            Traverse(tree.root, (n) =>
            {
                tree.nodes.Add(n);
            });
            return tree;
        }

        public void Bind()
        {
            Traverse(root, node =>
            {
                node.blackboard = blackboard;
            });
        }
    }
}