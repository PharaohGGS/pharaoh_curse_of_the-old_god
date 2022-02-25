using System;
using System.Collections.Generic;
using BehaviourTree.Tools;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace BehaviourTree.Editor
{
    public class NodeSearchWindow : ScriptableObject, ISearchWindowProvider
    {
        private EditorWindow window;
        private BehaviourTreeView graphView;

        private Texture2D indentationIcon;

        public void Configure(EditorWindow window, BehaviourTreeView graphView)
        {
            this.window = window;
            this.graphView = graphView;
            
            indentationIcon = new Texture2D(1, 1);
            indentationIcon.SetPixel(0, 0, new Color(0, 0, 0, 0));
            indentationIcon.Apply();
        }
        
        private void CreateTypeNodes(ref List<SearchTreeEntry> tree,TypeCache.TypeCollection collection)
        {
            foreach (var type in collection)
            {
                if (type.IsAbstract) continue;

                tree.Add(new SearchTreeEntry(new GUIContent(ObjectNames.NicifyVariableName(type.Name), indentationIcon))
                {
                    level = 2,
                    userData = type
                });
            }
        }

        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            var tree = new List<SearchTreeEntry>();
            
            tree.Add(new SearchTreeGroupEntry(new GUIContent("Add Node"), 0));

            tree.Add(new SearchTreeGroupEntry(new GUIContent("Action"), 1));
            CreateTypeNodes(ref tree, TypeCache.GetTypesDerivedFrom<ActionNode>());

            tree.Add(new SearchTreeGroupEntry(new GUIContent("Composite"), 1));
            CreateTypeNodes(ref tree, TypeCache.GetTypesDerivedFrom<CompositeNode>());

            tree.Add(new SearchTreeGroupEntry(new GUIContent("Decorator"), 1));
            CreateTypeNodes(ref tree, TypeCache.GetTypesDerivedFrom<DecoratorNode>());

            tree.AddRange(new List<SearchTreeEntry>
            {
                new SearchTreeGroupEntry(new GUIContent("Create Script..."), 1),
                new SearchTreeEntry(new GUIContent("New Action Node", indentationIcon))
                {
                    level = 2
                },
                new SearchTreeEntry(new GUIContent("New Composite Node", indentationIcon))
                {
                    level = 2
                },
                new SearchTreeEntry(new GUIContent("New Decorator Node", indentationIcon))
                {
                    level = 2
                }
            });

            return tree;
        }

        public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
        {
            var mousePosition = window.rootVisualElement.ChangeCoordinatesTo(window.rootVisualElement.parent,
                context.screenMousePosition - window.position.position);
            var graphMousePosition = graphView.contentViewContainer.WorldToLocal(mousePosition);
            
            graphView.CreateNode(SearchTreeEntry.userData.GetType());
            return true;
        }
    }
}
