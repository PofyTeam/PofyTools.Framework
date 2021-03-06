﻿using UnityEngine;
using XNodeEditor;

namespace PofyTools
{
    [CustomNodeGraphEditor(typeof(CategoryGraph))]
    public class CategoryGraphEditor : NodeGraphEditor
    {
        private CategoryGraph _catGraph;

        /// <summary> 
        /// Overriding GetNodeMenuName lets you control if and how nodes are categorized.
        /// In this example we are sorting out all node types that are not in the XNode.Examples namespace.
        /// </summary>
        public override string GetNodeMenuName(System.Type type)
        {
            if (this._catGraph == null)
                this._catGraph = this.target as CategoryGraph;

            if (type.Namespace == "PofyTools")
            {
                //if (type == typeof(CategorySetNode) && this._catGraph.HasSetNode)
                //    return null;
                return base.GetNodeMenuName(type).Replace("Pofy Tools/", "");
            }
            else
                return null;
        }

        public override void OnGUI()
        {
            
            if (this._catGraph == null)
                this._catGraph = this.target as CategoryGraph;

            //if (this._catGraph.HasSetNode)
            //{
            //    this._catGraph.SetNode.position = NodeEditorWindow.current.WindowToGridPosition(Vector2.zero);
            //}

            base.OnGUI();
        }
    }
}