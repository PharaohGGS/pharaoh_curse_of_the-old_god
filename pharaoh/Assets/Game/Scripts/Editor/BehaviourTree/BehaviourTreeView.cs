using Pharaoh.Tools.BehaviourTree.ScriptableObjects;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using Node = Pharaoh.Tools.BehaviourTree.ScriptableObjects.Node;

public class BehaviourTreeView : GraphView
{
    private BehaviourTree tree;

    public new class UxmlFactory : UxmlFactory<BehaviourTreeView, GraphView.UxmlTraits> {}
    public BehaviourTreeView()
    {
        Insert(0, new GridBackground());

        this.AddManipulator(new ContentZoomer());
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());

        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Game/Scripts/Editor/BehaviourTree/BehaviourTreeEditor.uss");
        styleSheets.Add(styleSheet);
    }

    public void PopulateView(BehaviourTree bt)
    {
        this.tree = bt;

        graphViewChanged -= GraphViewChanged;
        DeleteElements(graphElements);
        graphViewChanged += GraphViewChanged;

        tree.nodes.ForEach(node => CreateNodeView(node));
    }

    private GraphViewChange GraphViewChanged(GraphViewChange graphviewchange)
    {
        if (graphviewchange.elementsToRemove != null)
        {
            graphviewchange.elementsToRemove.ForEach(element =>
            {
                NodeView nodeView = element as NodeView;
                if (nodeView != null)
                {
                    tree.DeleteNode(nodeView.node);
                }
            });
        }
        return graphviewchange;
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
        var node = tree.CreateNode(type);
        CreateNodeView(node);
    }

    private void CreateNodeView(Node node)
    {
        NodeView nodeView = new NodeView(node);
        AddElement(nodeView);
    }
}
