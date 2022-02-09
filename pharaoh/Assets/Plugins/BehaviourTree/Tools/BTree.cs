using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace BehaviourTree.Tools
{
    [CreateAssetMenu(menuName = "BehaviourTree")]
    public class BTree : ScriptableObject
    {
        public BNode root;
        public NodeState treeState;
        public List<BNode> nodes;
        public Blackboard blackboard;

        private void Awake()
        {
            treeState = NodeState.Running;
            nodes = new List<BNode>();
            blackboard = new Blackboard();
        }

        public NodeState Evaluate()
        {
            if (root.state == NodeState.Running)
            {
                treeState = root.Evaluate();
            }

            return treeState;
        }

        public BNode CreateNode(System.Type type)
        {
            var node = ScriptableObject.CreateInstance(type) as BNode;
            node.name = type.Name;
            node.guid = GUID.Generate().ToString();

            Undo.RecordObject(this, "Behaviour BTree (Create node)");
            nodes.Add(node);

            if (!Application.isPlaying)
            {
                AssetDatabase.AddObjectToAsset(node, this);
            }
            
            Undo.RegisterCreatedObjectUndo(node, "Behaviour BTree (Create node)");
            AssetDatabase.SaveAssets();
            return node;
        }

        public void DeleteNode(BNode node)
        {
            Undo.RecordObject(this, "Behaviour BTree (Delete node)");
            nodes.Remove(node);
            //AssetDatabase.RemoveObjectFromAsset(node);
            Undo.DestroyObjectImmediate(node);
            AssetDatabase.SaveAssets();
        }

        public void AddChild(BNode parent, BNode child)
        {
            switch (parent)
            {
                case DecoratorNode decorator:
                    Undo.RecordObject(decorator, "Behaviour BTree (Add Child)");
                    decorator.child = child;
                    EditorUtility.SetDirty(decorator);
                    break;
                case RootNode rootNode:
                    Undo.RecordObject(rootNode, "Behaviour BTree (Add Child)");
                    rootNode.child = child;
                    EditorUtility.SetDirty(rootNode);
                    break;
                case CompositeNode composite:
                    Undo.RecordObject(composite, "Behaviour BTree (Add Child)");
                    composite.children.Add(child);
                    EditorUtility.SetDirty(composite);
                    break;
            }
        }
        public void RemoveChild(BNode parent, BNode child)
        {
            switch (parent)
            {
                case DecoratorNode decorator:
                    Undo.RecordObject(decorator, "Behaviour BTree (Remove Child)");
                    decorator.child = null;
                    EditorUtility.SetDirty(decorator);
                    break;
                case RootNode rootNode:
                    Undo.RecordObject(rootNode, "Behaviour BTree (Remove Child)");
                    rootNode.child = null;
                    EditorUtility.SetDirty(rootNode);
                    break;
                case CompositeNode composite:
                    Undo.RecordObject(composite, "Behaviour BTree (Remove Child)");
                    composite.children.Remove(child);
                    EditorUtility.SetDirty(composite);
                    break;
            }
        }

        public List<BNode> GetChildren(BNode parent)
        {
            var children = new List<BNode>();
            switch (parent)
            {
                case DecoratorNode decorator when decorator.child != null:
                    children.Add(decorator.child);
                    break;
                case RootNode rootNode:
                    children.Add(rootNode.child);
                    break;
                case CompositeNode composite:
                    return composite.children;
            }

            return children;
        }

        public void Traverse(BNode bNode, System.Action<BNode> visiter)
        {
            if (bNode)
            {
                visiter?.Invoke(bNode);
                var children = GetChildren(bNode);
                children.ForEach((n) => Traverse(n, visiter));
            }
        }

        public BTree Clone()
        {
            BTree bTree = Instantiate(this);
            bTree.root = bTree.root.Clone();
            bTree.nodes = new List<BNode>();
            Traverse(bTree.root, (n) =>
            {
                bTree.nodes.Add(n);
            });
            return bTree;
        }

        public void Bind(AiAgent agent)
        {
            Traverse(root, node =>
            {
                node.Blackboard = blackboard;
                node.Agent = agent;
            });
        }
    }
}