using InteractML.DataTypeNodes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace InteractML.FeatureExtractors
{
    /// <summary>
    /// Base Class for Feature Extractor Nodes
    /// </summary>
    public class BaseExtractorNode : IMLNode
    {
        // Use this for initialization
        protected override void Init()
        {
            base.Init();
        }

        // Return the correct value of an output port when requested
        public override object GetValue(NodePort port)
        {
            return null; // Replace this
        }

        //// Check that we are only connecting to a GameObject
        //public override void OnCreateConnection(NodePort from, NodePort to)
        //{
        //    this.DisconnectIfNotType<BaseExtractorNode, GameObjectNode>(from, to);
        //}
    }
}

