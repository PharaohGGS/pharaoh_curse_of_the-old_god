using System;
using BehaviourTree.Tools;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;


namespace BehaviourTree.Editor
{
    public class NodeView : UnityEditor.Experimental.GraphView.Node
    {
        public System.Action<NodeView> OnNodeSelected;
        public BNode node;

        public Port input;
        public Port output;

        public NodeView(BNode node) : base($"Assets/Plugins/BehaviourTree/UiBuilder/NodeView.uxml")
        {
            this.node = node;
            title = ObjectNames.NicifyVariableName(node.name.Replace("(Clone)", "").Replace("Node", "")); 
            viewDataKey = node.guid;
            
            style.left = node.position.x;
            style.top = node.position.y;

            CreateInputPorts();
            CreateOutputPorts();

            SetupClasses();

            Label descriptionLabel = this.Q<Label>("description");
            descriptionLabel.bindingPath = "description";
            descriptionLabel.Bind(new SerializedObject(node));
        }

        private void SetupClasses()
        {
            switch (node)
            {
                case RootNode rootNode:
                    AddToClassList("root");
                    break;
                case ActionNode actionNode:
                    AddToClassList("action");
                    break;
                case CompositeNode compositeNode:
                    AddToClassList("composite");
                    break;
                case DecoratorNode decoratorNode:
                    AddToClassList("decorator");
                    break;
            }
        }

        private void CreateInputPorts()
        {
            switch (node)
            {
                case RootNode rootNode:
                    break;
                case ActionNode actionNode:
                case CompositeNode compositeNode:
                case DecoratorNode decoratorNode:
                    input = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(bool));
                    break;
            }

            if (input == null) return;

            input.portName = "";
            input.style.flexDirection = FlexDirection.Column;
            inputContainer.Add(input);
        }

        private void CreateOutputPorts()
        {
            switch (node)
            {
                case ActionNode actionNode:
                    break;
                case CompositeNode compositeNode:
                    output = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Multi, typeof(bool));
                    break;
                case DecoratorNode decoratorNode:
                case RootNode rootNode:
                    output = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Single, typeof(bool));
                    break;
            }

            if (output == null) return;

            output.portName = "";
            output.style.flexDirection = FlexDirection.Column;
            outputContainer.Add(output);
        }
        
        public override void SetPosition(Rect newPos)
        {
            base.SetPosition(newPos);
            Undo.RecordObject(node, "Behaviour BTree (Set Position)");
            node.position.x = newPos.xMin;
            node.position.y = newPos.yMin;
            EditorUtility.SetDirty(node);
        }

        public override void OnSelected()
        {
            base.OnSelected();
            OnNodeSelected?.Invoke(this);
        }

        public void SortChildren()
        {
            if (node is CompositeNode compositeNode)
            {
                compositeNode.children.Sort(SortByHorizontalPosition);
            }
        }

        private int SortByHorizontalPosition(BNode left, BNode right)
        {
            return (left.position.x < right.position.x) ? -1 : 1;
        }

        public void UpdateState()
        {
            RemoveFromClassList("running");
            RemoveFromClassList("success");
            RemoveFromClassList("failure");

            if (!Application.isPlaying) return;

            switch (node.state)
            {
                case NodeState.Running:
                    if (node.hasStart)
                    {
                        AddToClassList("running");
                    }
                    break;
                case NodeState.Success:
                    AddToClassList("success");
                    break;
                case NodeState.Failure:
                    AddToClassList("failure");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
