using System.Collections.Generic;
using System.Linq;
using BehaviourTree.Tools;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace BehaviourTree.Editor
{
    public class BehaviourTreeView : GraphView
    {
        public System.Action<NodeView> OnNodeSelected;
        private BTree _tree;

        public EditorWindow window;

        public new class UxmlFactory : UxmlFactory<BehaviourTreeView, GraphView.UxmlTraits>
        {
        }

        public BehaviourTreeView()
        {
            Insert(0, new GridBackground());

            this.AddManipulator(new ContentZoomer());
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            var styleSheet =
                AssetDatabase.LoadAssetAtPath<StyleSheet>(
                    "Assets/Plugins/BehaviourTree/UiBuilder/BehaviourTreeEditor.uss");
            styleSheets.Add(styleSheet);

            Undo.undoRedoPerformed += OnUndoRedo;
        }

        private void OnUndoRedo()
        {
            PopulateView(_tree);
            AssetDatabase.SaveAssets();
        }

        private NodeView FindNodeView(BNode node)
        {
            return GetNodeByGuid(node.guid) as NodeView;
        }

        public void PopulateView(BTree tree)
        {
            _tree = tree;

            graphViewChanged -= OnGraphViewChanged;
            DeleteElements(graphElements);
            graphViewChanged += OnGraphViewChanged;

            if (_tree.root == null)
            {
                _tree.root = _tree.CreateNode(typeof(RootNode)) as RootNode;
                EditorUtility.SetDirty(_tree);
                AssetDatabase.SaveAssets();
            }

            // Create node views
            _tree.nodes.ForEach((n) => CreateNodeView(n, Vector2.zero));

            // Create edges
            _tree.nodes.ForEach(node =>
            {
                var children = _tree.GetChildren(node);
                
                children?.ForEach(child =>
                {
                    if (child == null) return;

                    var parentView = FindNodeView(node);
                    var childView = FindNodeView(child);

                    if (parentView == null || childView == null) return;

                    var edge = parentView.output.ConnectTo(childView.input);
                    AddElement(edge);
                });
                
            });
        }

        private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
        {
            if (graphViewChange.elementsToRemove != null)
            {
                graphViewChange.elementsToRemove.ForEach(element =>
                {
                    switch (element)
                    {
                        case NodeView nodeView:
                            _tree?.DeleteNode(nodeView.node);
                            break;
                        case Edge edge:
                        {
                            var parentView = edge.output.node as NodeView;
                            var childView = edge.input.node as NodeView;

                            //if (parentView != null && childView != null)
                            {
                                _tree?.RemoveChild(parentView.node, childView.node);
                            }
                            break;
                        }
                    }
                });
            }

            graphViewChange.edgesToCreate?.ForEach(edge =>
            {
                var parentView = edge.output.node as NodeView;
                var childView = edge.input.node as NodeView;
                //if (parentView != null && childView != null)
                {
                    _tree?.AddChild(parentView.node, childView.node);
                }
            });

            if (graphViewChange.movedElements != null)
            {
                nodes.ForEach(n =>
                {
                    var view = n as NodeView;
                    view?.SortChildren();
                });
            }

            return graphViewChange;
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            return ports.ToList().Where(endPort =>
                    endPort.direction != startPort.direction && endPort.node != startPort.node)
                .ToList();
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            var actionTypes = TypeCache.GetTypesDerivedFrom<ActionNode>();
            foreach (var type in actionTypes)
            {
                evt.menu.AppendAction($"[{type.BaseType.Name}] {type.Name}", (a) => CreateNode(type));
            }

            var compositeTypes = TypeCache.GetTypesDerivedFrom<CompositeNode>();
            foreach (var type in compositeTypes)
            {
                evt.menu.AppendAction($"[{type.BaseType.Name}] {type.Name}", (a) => CreateNode(type));
            }

            var decoratorTypes = TypeCache.GetTypesDerivedFrom<DecoratorNode>();
            foreach (var type in decoratorTypes)
            {
                evt.menu.AppendAction($"[{type.BaseType.Name}] {type.Name}", (a) => CreateNode(type));
            }
        }

        private void CreateNode(System.Type type)
        {
            var node = _tree.CreateNode(type);

            var worldMousePosition = ((BehaviourTreeEditor)window).mousePositionInEditorWindow;
            var localMousePosition = contentViewContainer.WorldToLocal(worldMousePosition);
            
            CreateNodeView(node, localMousePosition);
        }

        private void CreateNodeView(BNode node, Vector2 position)
        {
            var nodeView = new NodeView(node);
            nodeView.OnNodeSelected = OnNodeSelected;
            AddElement(nodeView);
            
            //var worldMousePosition = window.rootVisualElement.ChangeCoordinatesTo(window.rootVisualElement.parent,
            //    nodeView.GetPosition().position - window.position.position);
            
            if (node is RootNode || position == Vector2.zero) return;

            nodeView.SetPosition(new Rect(position,
                new Vector2(nodeView.style.width.value.value, nodeView.style.height.value.value)));
        }
        
        public void UpdateNodeStates()
        {
            nodes.ForEach(n =>
            {
                var view = n as NodeView;
                view?.UpdateState();
            });
        }
    }
}
