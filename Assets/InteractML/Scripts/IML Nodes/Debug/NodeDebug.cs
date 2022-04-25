using System.Collections.Generic;
using System.Linq;
using XNode;
using UnityEngine;

namespace InteractML
{
    /// <summary>
    /// Shows debug logs on graph nodes
    /// </summary>
    public static class NodeDebug
    {
        public static Dictionary<Node, string> Logs 
        { 
            get 
            {
                if (m_Logs == null)
                    m_Logs = new Dictionary<Node, string>();

                return m_Logs;
            }

        }
        private static Dictionary<Node, string> m_Logs;

        /// <summary>
        /// When there are more than one debug comments in the dictionary per node, queue them for later
        /// </summary>
        private static List<KeyValuePair<Node, string>> m_LogsQueue;

        /// <summary>
        /// Shows a warning on the node
        /// </summary>
        /// <param name="text"></param>
        /// <param name="node"></param>
        public static void LogWarning(string text, Node node, bool debugToConsole = false) 
        {
            if (m_Logs == null)
                m_Logs = new Dictionary<Node, string>();

            if (!m_Logs.ContainsKey(node))
                m_Logs.Add(node, text);
            else
                AddToQueue(text, node);

            if (debugToConsole)
                Debug.LogWarning($"{text} \n In {node}");
        }

        /// <summary>
        /// Deletes the current log warning displayed for a node
        /// </summary>
        /// <param name="node"></param>
        public static void DeleteLogWarning(Node node)
        {
            // Remove
            if (m_Logs.ContainsKey(node))
                m_Logs.Remove(node);

            // Pulls a warning from the queue if there are any
            if (m_LogsQueue != null  )
            {
                // Get all entries from queue
                var queue = m_LogsQueue.Where(x => x.Key.Equals(node));
                KeyValuePair<Node, string> entryToDelete = new KeyValuePair<Node, string>();
                // Show first entry
                foreach (var entry in queue)
                {
                    LogWarning(entry.Value, entry.Key);
                    entryToDelete = entry;
                    // Skip any more 
                    return;
                }
                // Delete entry if we managed to log any more
                if (entryToDelete.Key != null)
                    m_LogsQueue.Remove(entryToDelete);

            }


        }

        /// <summary>
        /// Adds text to queue in case there is already a warning displayed for a specific node
        /// </summary>
        /// <param name="text"></param>
        /// <param name="node"></param>
        private static void AddToQueue(string text, Node node)
        {
            if (m_LogsQueue == null)
                m_LogsQueue = new List<KeyValuePair<Node, string>>();

            var element = new KeyValuePair<Node, string>(node, text);
            m_LogsQueue.Add(element);

        }
    }
}