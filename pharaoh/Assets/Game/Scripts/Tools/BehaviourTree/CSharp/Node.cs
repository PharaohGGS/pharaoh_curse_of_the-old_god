using System;
using System.Collections.Generic;
using UnityEngine;

namespace Pharaoh.Tools.BehaviourTree.CSharp
{
    public enum NodeState
    {
        RUNNING,
        SUCCESS,
        FAILURE,
    }

    public class Node<T> : Node where T : Tree
    {
        protected T tree;

        public Node(T tree) : base()
        {
            this.tree = tree;
        }

        public Node(T tree, List<Node> children) : base(children)
        {
            this.tree = tree;
        }
    }

    [Serializable]
    public class Node
    {
        protected NodeState state;

        public Node parent;
        protected List<Node> children = new List<Node>();

        private GenericDictionary<string, object> _dataContext = new GenericDictionary<string, object>();
        
        public Node()
        {
            parent = null;
        }

        public Node(List<Node> children)
        {
            foreach (var child in children)
            {
                Attach(child);
            }
        }

        private void Attach(Node node)
        {
            node.parent = this;
            children.Add(node);
        }

        public virtual NodeState Evaluate() => NodeState.FAILURE;

        public void SetData(string key, object value)
        {
            if (_dataContext.ContainsKey(key))
            {
                _dataContext[key] = value;
            }
            else
            {
                _dataContext.Add(key, value);
            }
        }

        public object GetData(string key)
        {
            object value = null;

            if (_dataContext.TryGetValue(key, out value)) return value;

            Node node = parent;
            while (node != null)
            {
                value = node.GetData(key);
                if (value != null) return value;

                node = node.parent;
            }

            return null;
        }

        public bool ClearData(string key)
        {
            if (_dataContext.ContainsKey(key))
            {
                _dataContext.Remove(key);
                return true;
            }

            Node node = parent;
            while (node != null)
            {
                bool cleared = node.ClearData(key);
                if (cleared) return true;

                node = node.parent;
            }

            return false;
        }
    }
}