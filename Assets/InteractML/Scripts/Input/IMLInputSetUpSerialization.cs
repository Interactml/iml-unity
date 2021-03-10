using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;
using Newtonsoft.Json;
using System;

namespace InteractML {
    public static class IMLInputSetUpSerialization
    {
        private static string m_AppDataPath; // defines if we are using application.datapath or application.persistentdatapath (for cross-platform code)
        private static string m_FolderDataPathName = "InteractML/Data/InputSetUp";
        private static string m_FileName = "InputSettings.json";

        /// <summary>
        /// Saves the controller set up for interaction
        /// </summary>
        public static void SaveInputSettingToDisk(InputSetUpVRSettings settings)
        {
            if (String.IsNullOrEmpty(m_AppDataPath) ) {
#if UNITY_EDITOR || UNITY_STANDALONE
                m_AppDataPath = Application.dataPath;
#elif UNITY_ANDROID
                m_AppDataPath = Application.persistentDataPath;
#endif
            }
            string json = JsonUtility.ToJson(settings);
            string m_DataPath = Path.Combine(m_AppDataPath, m_FolderDataPathName);
            // Check if there is NOT a folder with the folder name
            if (!Directory.Exists(m_DataPath))
            {
                // If there is not, we create it
                Directory.CreateDirectory(m_DataPath);
            }
            m_DataPath = Path.Combine(m_DataPath, m_FileName);

            // Check if there is already a JSON file created for this training example
            if (File.Exists(m_DataPath))
            {
                // We delete it to make sure we override it
                File.Delete(m_DataPath);
            }
            //Debug.Log(jsonTrainingeExamplesList);
            var jsonSettings = JsonConvert.SerializeObject(settings, Formatting.Indented);
            // Write on the path
            File.WriteAllText(m_DataPath, jsonSettings);
        }

        public static InputSetUpVRSettings LoadInputSettings()
        {
            if (String.IsNullOrEmpty(m_AppDataPath))
            {
#if UNITY_EDITOR || UNITY_STANDALONE
                m_AppDataPath = Application.dataPath;
#elif UNITY_ANDROID
                m_AppDataPath = Application.persistentDataPath;
#endif
            }

            string auxFilePath = Path.Combine(m_AppDataPath, m_FolderDataPathName, m_FileName);
            InputSetUpVRSettings settings = new InputSetUpVRSettings();
            if (File.Exists(auxFilePath))
            {
                try
                {
                    string jsonTrainingExamplesList = File.ReadAllText(auxFilePath);
                    if (jsonTrainingExamplesList != null)
                    {
                        string json = File.ReadAllText(auxFilePath);
                        settings = JsonConvert.DeserializeObject<InputSetUpVRSettings>(json);
                    }
                }
                catch (FileNotFoundException e)
                {
                    Debug.Log(e.Message);
                }
                catch (IOException e)
                {
                    Debug.LogError(e.Message);
                }
                //Debug.Log("The file exists and we read from it!");


                //Debug.Log("What we read is: " + jsonTrainingExamplesList);
            } 
            return settings;
        }
    }
}

