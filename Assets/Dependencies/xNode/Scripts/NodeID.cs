using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;
using System;


    /// <summary>
    /// Offers a collection of functions to serialize/deserialize IML data from disk
    /// </summary>
    public static class NodeID
    { 
        public static string CheckNodeID(string id, Node node)
        {
            Debug.Log(id);
            if (id == null)
            {
                id = Guid.NewGuid().ToString();
            }
            else
            {
                var match = node.graph.nodes.Find(n => {
                    if (n == node) return false;
                    Node skillNode = n;
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

