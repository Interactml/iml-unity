using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;
using Newtonsoft.Json;
using System;

namespace InteractML
{
    public static class IMLTooltipsSerialization
    {
        private static string m_AppDataPath; // defines if we are using application.datapath or application.persistentdatapath (for cross-platform code)
        private static string m_FolderPath = "InteractML/Data/Tooltips/";
        private static string m_DataPath;
        private static string m_FileExtension = ".json";

        /// <summary>
        /// Loads Tooltip JSON 
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns>Returns a IMLNodeTooltips object </returns>
        public static IMLNodeTooltips LoadTooltip(string fileName)
        {
            IMLNodeTooltips toolTips = new IMLNodeTooltips();
            if (File.Exists(SetUpFileNamesAndPaths(fileName)))
            {
                try
                {
                    string jsonFile = File.ReadAllText(SetUpFileNamesAndPaths(fileName));
                    toolTips = JsonConvert.DeserializeObject<IMLNodeTooltips>(jsonFile);
                }
                catch (FileNotFoundException e)
                {
                    Debug.Log(e.Message);
                }
                catch (IOException e)
                {
                    Debug.LogError(e.Message);
                }
            }
            
            return toolTips;
        }

        /// <summary>
        /// Sets up file path for tooltip
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns>Returns file path for tooltip json</returns>
        private static string SetUpFileNamesAndPaths(string fileName)
        {
            if (String.IsNullOrEmpty(m_AppDataPath))
            {
#if UNITY_EDITOR || UNITY_STANDALONE
                m_AppDataPath = Application.dataPath;
#elif UNITY_ANDROID
                m_AppDataPath = Application.persistentDataPath;
#endif
            }

            // Set up data path (Application.dataPath + FolderName + FileName + FileExtension)
            m_DataPath = Path.Combine(m_AppDataPath, m_FolderPath + fileName + m_FileExtension);

            return m_DataPath;

        }
    }
}