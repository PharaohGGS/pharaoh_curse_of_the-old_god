using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

public class BehaviourTreeView : GraphView
{
    public new class UxmlFactory : UxmlFactory<BehaviourTreeView, GraphView.UxmlTraits> {}
    public BehaviourTreeView()
    {
        Insert(0, new GridBackground());

        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Game/Scripts/Editor/BehaviourTree/BehaviourTreeEditor.uss");
        styleSheets.Add(styleSheet);
    }
}
