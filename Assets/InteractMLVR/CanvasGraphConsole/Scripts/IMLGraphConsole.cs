using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ReusableMethods;

namespace InteractML
{
    /// <summary>
    /// Canvas console that displays information about a graph status
    /// </summary>
    public class IMLGraphConsole : MonoBehaviour
    {
        #region Variables

        /// <summary>
        /// MLComponents to pull data from
        /// </summary>
        [SerializeField]
        private List<IMLComponent> m_MLComponents;

        /// <summary>
        /// Where the output is written to
        /// </summary>
        [SerializeField]
        private Text m_ConsoleUIText;
        /// <summary>
        /// Log to draw on console
        /// </summary>
        private string m_ConsoleLog;
        /// <summary>
        /// Wait n frames to redraw Console
        /// </summary>
        private int m_NumFramesToWait = 10;

        /// <summary>
        /// What is the console logging?
        /// </summary>
        private enum LogOptionsEnum { all, trainingExamples, MLsystem }
        [SerializeField]
        private LogOptionsEnum m_LogOptions;

        #endregion

        #region Unity Messages

        // Start is called before the first frame update
        void Start()
        {
            // Find all components if none are added in editor
            if (m_MLComponents == null || m_MLComponents.Count == 0)
            {
                m_MLComponents = new List<IMLComponent>();
                var MLComponentsFound = FindObjectsOfType<IMLComponent>();
                if (MLComponentsFound != null) 
                {
                    foreach (var MLComponent in MLComponentsFound)
                    {
                        m_MLComponents.Add(MLComponent);
                    }
                }                
            }
            if (m_ConsoleUIText == null)
            {
                m_ConsoleUIText = GetComponentInChildren<Text>();
            }

            // Launch coroutine if there is anything to debug
            if (m_ConsoleUIText != null && m_MLComponents != null && m_MLComponents.Count > 0)
            {
                StartCoroutine(ConsoleCoroutine());
            }
        }


        #endregion

        #region Coroutines

        /// <summary>
        /// Coroutine that draws to consle
        /// </summary>
        /// <returns></returns>
        public IEnumerator ConsoleCoroutine()
        {
            while (m_ConsoleUIText != null && m_MLComponents != null)
            {
                // Empty console log
                m_ConsoleLog = "";

                // Pull info from each ml component
                foreach (var MLComponent in m_MLComponents)
                {
                    m_ConsoleLog += PullStatus(MLComponent);
                }

                // Draw on console
                m_ConsoleUIText.text = m_ConsoleLog;

                // wait n frames
                for (int i = 0; i < m_NumFramesToWait; i++) yield return null;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Pull status from an ML component into a string 
        /// </summary>
        /// <param name="MLComponent"></param>
        /// <returns></returns>
        private string PullStatus(IMLComponent MLComponent)
        {
            string status = "";
            bool logTENodes = m_LogOptions == LogOptionsEnum.all || m_LogOptions == LogOptionsEnum.trainingExamples ? true : false;
            bool logMLSNodes = m_LogOptions == LogOptionsEnum.all || m_LogOptions == LogOptionsEnum.MLsystem ? true : false;

            // Training Examples
            if (logTENodes && MLComponent.TrainingExamplesNodesList != null)
            {
                foreach (var TENode in MLComponent.TrainingExamplesNodesList)
                {
                    status += $"Teach Node {Strings.Truncate(TENode.id, 8)}{System.Environment.NewLine}"; // show only first 8 digits of id
                    status += TENode.GetStatus(TENode.id);
                    status += System.Environment.NewLine;
                    status += System.Environment.NewLine;
                }
            }

            // MLSystems
            if (logMLSNodes && MLComponent.MLSystemNodeList != null)
            {
                foreach (var MLSNode in MLComponent.MLSystemNodeList)
                {
                    status += $"Model Node {Strings.Truncate(MLSNode.id, 8)}{System.Environment.NewLine}"; // show only first 8 digits of id
                    status += MLSNode.GetStatus(MLSNode.id);
                    status += System.Environment.NewLine;
                    status += System.Environment.NewLine;
                }
            }

            return status;
        }

        #endregion

    }

}
