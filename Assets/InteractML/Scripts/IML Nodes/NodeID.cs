using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;
using System;


namespace InteractML
{  /// <summary>
   /// Gets or sets the id from a node
   /// </summary>
    public static class NodeID
    {
        public static string CheckNodeID(string id, Node node)
        {
            if (id == null)
            {
                id = Guid.NewGuid().ToString();
            }
            else
            {
                var match = node.graph.nodes.Find(n =>
                {
                    if (n == node) return false;
                    IMLNode skillNode = n as IMLNode;
                    return skillNode != null && skillNode.id == id;
                });

                if (match != null)
                {
                    id = Guid.NewGuid().ToString();
                }
            }
            return id;
        }
    }
}

