using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;
using Newtonsoft.Json;
using System;

namespace InteractML
{
    public class IMLTooltipsSerialization : MonoBehaviour
    {
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
            string jsonFile = File.ReadAllText(SetUpFileNamesAndPaths(fileName));
            IMLNodeTooltips toolTips = JsonConvert.DeserializeObject<IMLNodeTooltips>(jsonFile);
            return toolTips;
        }

        /// <summary>
        /// Sets up file path for tooltip
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns>Returns file path for tooltip json</returns>
        private static string SetUpFileNamesAndPaths(string fileName)
        {
            // Set up data path (Application.dataPath + FolderName + FileName + FileExtension)
            m_DataPath = Path.Combine(Application.dataPath, m_FolderPath + fileName + m_FileExtension);

            return m_DataPath;

        }
    }
}