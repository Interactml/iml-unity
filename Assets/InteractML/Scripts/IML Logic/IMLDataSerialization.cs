using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using System.Linq;
using System.IO;
using Newtonsoft.Json;
using System;

namespace InteractML
{
    /// <summary>
    /// Offers a collection of functions to serialize/deserialize IML data from disk
    /// </summary>
    public static class IMLDataSerialization
    {
        #region Variables

        private static string m_AppDataPath;
        private static string m_DefaultFolderDataPathName = "InteractML/Data";
        private static string m_FolderDataPathName = "InteractML/Data";
        private static string m_SubFolderTrainingSetPathName;
        private static string m_SubFolderModelPathName;
        private static string m_DataPathModel;
        private static string m_DataPathTrainingSet;
        private static string m_FileTrainingSetName;
        private static string m_FileExtension;
        private static string m_FileModelName;
        private static bool m_SerializeWithJSONDotNet = true;

        #endregion

        #region Public Methods

        /// <summary>
        /// Parses an IML Feature into JSON
        /// </summary>
        /// <param name="features"></param>
        /// <returns></returns>
        public static string ParseIMLFeatureToJSON(List<IMLBaseDataType> features)
        {
            return JsonConvert.SerializeObject(features, Formatting.Indented);
        }

        /// <summary>
        /// Parses a JSON Feature into an IML Feature
        /// </summary>
        /// <param name="jsonFeatures"></param>
        /// <returns></returns>
        public static List<IMLBaseDataType> ParseJSONToIMLFeature(string jsonFeatures)
        {
            return JsonConvert.DeserializeObject<List<IMLBaseDataType>>(jsonFeatures);
        }

        /// <summary>
        /// Overrides the folder data path
        /// </summary>
        /// <param name="newFolderDataPath"></param>
        public static void OverrideFolderDataPath (string newFolderDataPath)
        {
            m_FolderDataPathName = newFolderDataPath;
        }

        /// <summary>
        /// Sets the folder data path value to default under "InteractML/Data"
        /// </summary>
        public static void SetFolderDataPathToDefault()
        {
            m_FolderDataPathName = m_DefaultFolderDataPathName;
        }

        /// <summary>
        /// Instantiates an abstract IML base data into a specific one
        /// </summary>
        /// <param name="dataToInstantiate"></param>
        /// <param name="dataToReadFrom"></param>
        /// <param name="IMLType"></param>
        public static void InstantiateIMLData(ref IMLBaseDataType dataToInstantiate,  IMLBaseDataType dataToReadFrom)
        {
            if (dataToReadFrom == null)
            {
                Debug.LogError("Can't instantiate a null IML data type!");
                return;
            }

            switch (dataToReadFrom.DataType)
            {
                case IMLSpecifications.DataTypes.Float:
                    dataToInstantiate = new IMLFloat(dataToReadFrom);
                    break;
                case IMLSpecifications.DataTypes.Integer:
                    dataToInstantiate = new IMLInteger(dataToReadFrom);
                    break;
                case IMLSpecifications.DataTypes.Vector2:
                    dataToInstantiate = new IMLVector2(dataToReadFrom);
                    break;
                case IMLSpecifications.DataTypes.Vector3:
                    dataToInstantiate = new IMLVector3(dataToReadFrom);
                    break;
                case IMLSpecifications.DataTypes.Vector4:
                    dataToInstantiate = new IMLVector4(dataToReadFrom);
                    break;
                case IMLSpecifications.DataTypes.Array:
                    dataToInstantiate = new IMLArray(dataToReadFrom);
                    break;
                case IMLSpecifications.DataTypes.Boolean:
                    dataToInstantiate = new IMLBoolean(dataToReadFrom);
                    break;
                default:
                    break;
            }

        }

        /// <summary>
        /// Instantiates an abstract IML base data into a specific one
        /// </summary>
        /// <param name="dataToInstantiate"></param>
        /// <param name="dataToReadFrom"></param>
        /// <param name="IMLType"></param>
        public static void InstantiateIMLData(ref IMLBaseDataType dataToInstantiate, IMLSpecifications.DataTypes IMLType)
        {
            switch (IMLType)
            {
                case IMLSpecifications.DataTypes.Float:
                    dataToInstantiate = new IMLFloat();
                    break;
                case IMLSpecifications.DataTypes.Integer:
                    dataToInstantiate = new IMLInteger();
                    break;
                case IMLSpecifications.DataTypes.Vector2:
                    dataToInstantiate = new IMLVector2();
                    break;
                case IMLSpecifications.DataTypes.Vector3:
                    dataToInstantiate = new IMLVector3();
                    break;
                case IMLSpecifications.DataTypes.Vector4:
                    dataToInstantiate = new IMLVector4();
                    break;
                case IMLSpecifications.DataTypes.Array:
                    dataToInstantiate = new IMLArray();
                    break;
                default:
                    break;
            }

        }

        /// <summary>
        /// Saves Training Data Set to disk
        /// </summary>
        /// <param name="listToSave">The list of training examples</param>
        /// <param name="filePath">File path without file extension</param>
        public static void SaveTrainingSetToDisk(List<IMLBaseDataType> listToSave, string fileName)
        {
            SaveTrainingSetToDisk<IMLBaseDataType>(listToSave, fileName);
        }

        /// <summary>
        /// Saves Training Data Set to disk
        /// </summary>
        /// <param name="listToSave">The list of training examples</param>
        /// <param name="filePath">File path without file extension</param>
        public static void SaveTrainingSetToDisk(List<IMLTrainingExample> listToSave, string fileName)
        {
            SaveTrainingSetToDisk<IMLTrainingExample>(listToSave, fileName);
        }

        public static void SaveTrainingSeriesCollectionToDisk(List<IMLTrainingSeries> listToSave, string fileName)
        {
            SaveTrainingSetToDisk<IMLTrainingSeries>(listToSave, fileName);
        }

        /// <summary>
        /// Saves Training Data Set (from Rapidlib) to disk
        /// </summary>
        /// <param name="listToSave"></param>
        /// <param name="fileName"></param>
        public static void SaveTrainingSetToDiskRapidlib(List<RapidlibTrainingExample> listToSave, string fileName)
        {
            SaveTrainingSetToDisk<RapidlibTrainingExample>(listToSave, fileName);
        }

        /// <summary>
        /// Saves a training series set (from Rapidlib) to disk
        /// </summary>
        /// <param name="listToSave"></param>
        /// <param name="fileName"></param>
        public static void SaveTrainingSeriesSetsToDiskRapidlib(List<RapidlibTrainingSerie> listToSave, string fileName)
        {
            SaveTrainingSetToDisk<RapidlibTrainingSerie>(listToSave, fileName);
        }

        /// <summary>
        /// Loads Training Data Set from Disk assuming it is stored solely as BaseDataType (not using IMLTrainingExamples)
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns>Returns a list with training set</returns>
        public static List<IMLBaseDataType> LoadTrainingSetFromDiskAsBaseDataType (string fileName)
        {
            List<IMLBaseDataType> auxList = LoadTrainingSetFromDisk<IMLBaseDataType>(fileName);

            if (auxList != null)
            {
                return auxList;
            }
            else
            {
                Debug.LogError("Training set to load from disk is null!");
                return null;
            }
        }

        /// <summary>
        /// Loads Training Data Set from Disk
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns>Returns a list with training set</returns>
        public static List<IMLTrainingExample> LoadTrainingSetFromDisk(string fileName, bool ignoreDefaultLocation = false)
        {
            List<IMLTrainingExample> auxList = LoadTrainingSetFromDisk<IMLTrainingExample>(fileName, ignoreDefaultLocation);

            if (auxList != null)
            {
                return auxList;
            }
            else
            {
                Debug.LogError("Training set to load from disk is null!");
                return null;
            }
        }

        /// <summary>
        /// Loads Training Data Set from Disk Asynchronously
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns>Returns a list with training set</returns>

        public static async Task<List<IMLTrainingExample>> LoadTrainingSetFromDiskAsync(string fileName, bool ignoreDefaultLocation = false)
        {
            List<IMLTrainingExample> auxList = await LoadTrainingSetFromDiskAsync<IMLTrainingExample>(fileName, ignoreDefaultLocation);

            if (auxList != null)
            {
                return auxList;
            }
            else
            {
                Debug.LogError("Training set to load from disk is null!");
                return null;
            }
        }


        /// <summary>
        /// Loads a training series collection from disk (for InteractML)
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static List<IMLTrainingSeries> LoadTrainingSeriesCollectionFromDisk(string fileName)
        {
            List<IMLTrainingSeries> auxList = LoadTrainingSetFromDisk<IMLTrainingSeries>(fileName);
            if (auxList != null)
            {
                return auxList;
            }
            else
            {
                Debug.LogError("IML Training series set to load from disk is null!");
                return null;
            }
        }

        /// <summary>
        /// Loads a training data set from disk (for Rapidlib)
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static List<RapidlibTrainingExample> LoadTrainingSetFromDiskRapidlib(string fileName)
        {
            List<RapidlibTrainingExample> auxList = LoadTrainingSetFromDisk<RapidlibTrainingExample>(fileName);

            if (auxList != null)
            {
                return auxList;
            }
            else
            {
                Debug.LogError("Training set to load from disk is null!");
                return null;
            }

        }

        /// <summary>
        /// Loads a training series set from disk (for Rapidlib)
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static List<RapidlibTrainingSerie> LoadTrainingSeriesCollectionFromDiskRapidlib(string fileName)
        {
            List<RapidlibTrainingSerie> auxList = LoadTrainingSetFromDisk<RapidlibTrainingSerie>(fileName);

            if (auxList != null)
            {
                return auxList;
            }
            else
            {
                Debug.LogError("Training series set to load from disk is null!");
                return null;
            }

        }

        public static void SaveRapidlibModelToDisk(string modelToSave, string fileName)
        {
            SetUpFileNamesAndPaths(fileName);
            Debug.Log("save model");
            string subFolderPath = CheckOrCreateFoldersAndSubfoldersModel();

            // We save the entire input/output list as a JSON
            string auxFilePath = subFolderPath + "/" + m_FileModelName + m_FileExtension;
            // Check if there is already a JSON file created for this training example
            if (File.Exists(auxFilePath))
            {
                // We delete it to make sure we override it
                File.Delete(auxFilePath);
            }
            // Write on the path
            File.WriteAllText(auxFilePath, modelToSave);

        }

        public static void DeleteRapidlibModelFromDisk(string fileName)
        {
            SetUpFileNamesAndPaths(fileName);

            string subFolderPath = CheckOrCreateFoldersAndSubfoldersModel();

            // We save the entire input/output list as a JSON
            string auxFilePath = subFolderPath + "/" + m_FileModelName + m_FileExtension;
            // Check if there is already a JSON file created for this training example
            if (File.Exists(auxFilePath))
            {
                // We delete it to make sure we override it
                File.Delete(auxFilePath);
            }
        }

        public static string LoadRapidlibModelFromDisk(string fileName)
        {
            SetUpFileNamesAndPaths(fileName);

            string subFolderPath = CheckOrCreateFoldersAndSubfoldersModel();

            // We save the entire input/output list as a JSON
            string auxFilePath = subFolderPath + "/" + m_FileModelName + m_FileExtension;

            // Check if there is NOT already a JSON file created
            if (File.Exists(auxFilePath))
            {
                Debug.Log("file found");
                var file = File.ReadAllText(auxFilePath);
                if (file != null && file != "")
                {
                    // We read from the file
                    return file;
                }
            }

            return "";

        }

        /// <summary>
        /// Saves a generic object to disk
        /// </summary>
        /// <param name="ObjToSave"></param>
        public static void SaveObjectToDisk(object ObjToSave)
        {
            string objName = "Obj_" + ObjToSave.GetType().Name;
            SetUpFileNamesAndPaths(objName);

            // Check if there is NOT a folder with the folder name
            if (!Directory.Exists(Path.Combine(m_AppDataPath, m_FolderDataPathName)))
            {
                // If there is not, we create it
                Directory.CreateDirectory(Path.Combine(m_AppDataPath, m_FolderDataPathName));
            }

            string subFolderPath = Path.Combine(m_AppDataPath, m_FolderDataPathName + objName);
            //Debug.Log("SUBFOLDER PATH IS: " + subFolderPath);

            // Check if there is NOT a subfolder with the component name
            if (!Directory.Exists(subFolderPath))
            {
                // If there is not, we create it
                Directory.CreateDirectory(subFolderPath);
            }

            // If the option to serialize witht JSON dot net is active...
            if (m_SerializeWithJSONDotNet)
            {
                // We save the object passed in as a JSON
                string auxFilePath = subFolderPath + "/" + objName + m_FileExtension;
                // Check if there is already a JSON file created for this training example
                if (File.Exists(auxFilePath))
                {
                    // We delete it to make sure we override it
                    File.Delete(auxFilePath);
                }
                // Generate JSON string from the entire list
                var jsonTrainingeExamplesList = JsonConvert.SerializeObject(ObjToSave);
                // Write on the path
                File.WriteAllText(auxFilePath, jsonTrainingeExamplesList);
            }

        }

        /// <summary>
        /// Loads a generic object from disk
        /// </summary>
        /// <param name="ObjToLoad"></param>
        /// <returns></returns>
        public static object LoadObjectFromDisk(object ObjToLoad)
        {
            //Debug.Log("Load object from disk called! " + filePath);

            string objName = "Obj_" + ObjToLoad.GetType().Name;
            SetUpFileNamesAndPaths(objName);

            // Check if there is NOT a folder with the folder name
            if (!Directory.Exists(Path.Combine(m_AppDataPath, m_FolderDataPathName)))
            {
                // If there is not, we create it
                Directory.CreateDirectory(Path.Combine(m_AppDataPath, m_FolderDataPathName));
            }

            string subFolderPath = Path.Combine(m_AppDataPath, m_FolderDataPathName + objName);
            //Debug.Log("SUBFOLDER PATH IS: " + subFolderPath);

            // Check if there is NOT a subfolder with the component name
            if (!Directory.Exists(subFolderPath))
            {
                // If there is not, we create it
                Directory.CreateDirectory(subFolderPath);
            }

            var auxObj = new object();

            // If the option to serialize witht JSON dot net is active...
            if (m_SerializeWithJSONDotNet)
            {
                // We calculate the entire input/output list file name
                string auxFilePath = subFolderPath + "/" + objName + m_FileExtension;
                //Debug.Log("File name to read is: " + auxFilePath);
                // We check if the file is there before reading from it
                if (File.Exists(auxFilePath))
                {
                    //Debug.Log("The file exists and we read from it!");
                    string objJSONData = File.ReadAllText(auxFilePath);
                    if (objJSONData != null)
                        auxObj = JsonConvert.DeserializeObject<object>(objJSONData);

                    //Debug.Log("What we read is: " + jsonTrainingExamplesList);
                }
            }

            return auxObj;

        }

        public static string GetValueFromJSON(string valueName, string jsonFile)
        {
            int index = jsonFile.IndexOf(valueName);
            string valueToReturn = "";
            // If that property is there...
            if (index != -1)
            {
                // We get the enough values after the string (counting the chars of the word numOutputs + the spaces for the numbers)
                string dataFromJson = jsonFile.Substring(index, 20);
                // Get only the digits
                valueToReturn = new string(dataFromJson.Where(char.IsDigit).ToArray());
            }

            //Debug.Log("Value read from JSON for property " + valueName + " is: " + valueToReturn);

            return valueToReturn;
        }
        #endregion

        #region Private Methods

        /// <summary>
        /// Sets up file names and paths based on the fileName passed in
        /// </summary>
        /// <param name="fileName"></param>
        private static void SetUpFileNamesAndPaths(string fileName)
        {
            // First, set up data path and immediate subfolders
            SetUpIMLDataPath();

            string folderInFileName = "";
            // If the file name contains any folders...
            if (fileName.Contains("/"))
            {
                // Get folder(s) names
                folderInFileName = Path.GetDirectoryName(fileName);
                // Remove folder(s) from fileName
                fileName = Path.GetFileName(fileName);
            }

            // Set up file extension type
            m_FileExtension = ".json";

            // Set up file names
            m_FileModelName = fileName + "_Model";
            m_FileTrainingSetName = fileName + "_TrainingSet";

            // If the fileName included desired subfolders...
            if (!String.IsNullOrEmpty(folderInFileName)) 
            {
                // ONLY AFFECTING LOCATION OF TRAINING SET FOR THE MOMENT
                // Add the folders to the m_SubFolderTrainingSetPathName
                m_SubFolderTrainingSetPathName = string.Concat(m_SubFolderTrainingSetPathName, "/", folderInFileName);
            }

            // Set up data path (m_AppDataPath + FolderName + FileName + FileExtension)
            m_DataPathModel = Path.Combine(m_AppDataPath, m_FolderDataPathName + m_FileModelName + m_FileExtension);
            // Training set is not having the extension added yet
            m_DataPathTrainingSet = Path.Combine(m_AppDataPath, m_FolderDataPathName + m_FileTrainingSetName);
            //Debug.Log("datapath TRAINING SET IS: " + m_DataPathTrainingSet);

            // Mark the class to use json serialization
            m_SerializeWithJSONDotNet = true;

        }

        /// <summary>
        /// Only sets up the IML data path, but not specific training example files
        /// </summary>
        private static void SetUpIMLDataPath() 
        {
            // Set up training Examples subfolder 
            m_SubFolderTrainingSetPathName = m_FolderDataPathName + "/Training_Examples";
            m_SubFolderModelPathName = m_FolderDataPathName + "/Models";

            //m_AppDataPath = "";
#if UNITY_STANDALONE || UNITY_EDITOR
            // in a standalone build or editor, we go to local assets folder
            if (string.IsNullOrEmpty(m_AppDataPath)) m_AppDataPath = Application.dataPath;
#elif UNITY_ANDROID
            // on Android it is better to use persistent datapath           
            if (string.IsNullOrEmpty(m_AppDataPath)) m_AppDataPath = Application.persistentDataPath;
#endif

        }

        /// <summary>
        /// Makes sure that the directory structure is consistent and returns the subfolder path of Models (call after SetUpFileNamesAndPaths)
        /// </summary>
        /// <returns></returns>
        private static string CheckOrCreateFoldersAndSubfoldersModel()
        {
            // Check if there is NOT a folder with the folder name
            if (!Directory.Exists(Path.Combine(m_AppDataPath, m_FolderDataPathName)))
            {
                // If there is not, we create it
                Directory.CreateDirectory(Path.Combine(m_AppDataPath, m_FolderDataPathName));
            }

            string subFolderPath = Path.Combine(m_AppDataPath, m_SubFolderModelPathName);
            //Debug.Log("SUBFOLDER PATH IS: " + subFolderPath);

            // Check if there is NOT a subfolder with the component name
            if (!Directory.Exists(subFolderPath))
            {
                // If there is not, we create it
                Directory.CreateDirectory(subFolderPath);
            }

            // Make sure the default folderDataPath name is reset to default (in case there has been any changes)
            m_FolderDataPathName = m_DefaultFolderDataPathName;

            return subFolderPath;
        }

        /// <summary>
        /// Makes sure that the directory structure is consistent and returns the subfolder path of Training Sets (call after SetUpFileNamesAndPaths)
        /// </summary>
        /// <returns></returns>
        private static string CheckOrCreateFoldersAndSubfoldersTrainingSet()
        {
            // Check if there is NOT a folder with the folder name
            if (!Directory.Exists(Path.Combine(m_AppDataPath, m_FolderDataPathName)))
            {
                // If there is not, we create it
                Directory.CreateDirectory(Path.Combine(m_AppDataPath, m_FolderDataPathName));
            }

            string subFolderPath = Path.Combine(m_AppDataPath, m_SubFolderTrainingSetPathName);
            //Debug.Log("SUBFOLDER PATH IS: " + subFolderPath);

            // Check if there is NOT a subfolder with the component name
            if (!Directory.Exists(subFolderPath))
            {
                // If there is not, we create it
                Directory.CreateDirectory(subFolderPath);
            }

            // Make sure the default folderDataPath name is reset to default (in case there has been any changes)
            m_FolderDataPathName = m_DefaultFolderDataPathName;

            return subFolderPath;
        }


        /// <summary>
        /// Private loading data method with generic type that loads the training set. We control the types in the public overloads
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private static List<T> LoadTrainingSetFromDisk<T>(string fileName, bool ignoreDefaultLocation = false)
        {
            bool canLoad = false;
            string auxFilePath = "";

            List<T> auxList = new List<T>();

            // If the option to serialize witht JSON dot net is active...
            if (m_SerializeWithJSONDotNet)
            {
                if (ignoreDefaultLocation)
                {
                    auxFilePath = fileName;
                    if (File.Exists(auxFilePath)) 
                        canLoad = true;
                }
                else 
                {
                    SetUpFileNamesAndPaths(fileName);

                    //Debug.Log("Load training set from disk called! FolderDataPath: " + m_FolderDataPathName);

                    string subFolderPath = CheckOrCreateFoldersAndSubfoldersTrainingSet();

                    // We calculate the entire input/output list file name
                    auxFilePath = subFolderPath + "/" + m_FileTrainingSetName + "_Inputs_Outputs" + m_FileExtension;
                    //Debug.Log("File to load is: >>> " + auxFilePath);
                    //Debug.Log("File name to read is: " + auxFilePath);
                    // We check if the file is there before reading from it
                    if (File.Exists(auxFilePath))
                        canLoad = true;
                    //else
                    //    Debug.LogError($"Error when loading file: {fileName}. It doesn't exist!");
                }

                // Load file if possible
                if (canLoad)
                {
                    try
                    {
                        string jsonTrainingExamplesList = File.ReadAllText(auxFilePath);
                        if (jsonTrainingExamplesList != null)
                        {
                            //Debug.Log("Examples are not null, loading the text");
                            //Debug.Log(jsonTrainingExamplesList);
                            auxList = JsonConvert.DeserializeObject<List<T>>(jsonTrainingExamplesList);
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
            }

            return auxList;


        }

        private static async Task<List<T>> LoadTrainingSetFromDiskAsync<T>(string fileName, bool ignoreDefaultLocation = false)
        {
            bool canLoad = false;
            string auxFilePath = "";

            List<T> auxList = new List<T>();

            // If the option to serialize witht JSON dot net is active...
            if (m_SerializeWithJSONDotNet)
            {
                if (ignoreDefaultLocation)
                {
                    auxFilePath = fileName;
                    if (File.Exists(auxFilePath))
                        canLoad = true;
                }
                else
                {
                    SetUpFileNamesAndPaths(fileName);

                    //Debug.Log("Load training set from disk called! FolderDataPath: " + m_FolderDataPathName);

                    string subFolderPath = CheckOrCreateFoldersAndSubfoldersTrainingSet();

                    // We calculate the entire input/output list file name
                    auxFilePath = subFolderPath + "/" + m_FileTrainingSetName + "_Inputs_Outputs" + m_FileExtension;
                    //Debug.Log("File to load is: >>> " + auxFilePath);
                    //Debug.Log("File name to read is: " + auxFilePath);
                    // We check if the file is there before reading from it
                    if (File.Exists(auxFilePath))
                        canLoad = true;
                    else
                        Debug.LogError($"Error when loading file: {fileName}. It doesn't exist!");
                }

                // Load file if possible
                if (canLoad)
                {
                    try
                    {
                        using (var reader = File.OpenText(auxFilePath))
                        {
                            string jsonTrainingExamplesList = await reader.ReadToEndAsync();
                            if (jsonTrainingExamplesList != null)
                            {
                                //Debug.Log("Examples are not null, loading the text");
                                //Debug.Log(jsonTrainingExamplesList);
                                auxList = JsonConvert.DeserializeObject<List<T>>(jsonTrainingExamplesList);
                            }

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
            }

            return auxList;



        }


        /// <summary>
        /// Private method that saves Training Data Set to disk
        /// </summary>
        /// <param name="listToSave">The list of training examples</param>
        /// <param name="filePath">File path without file extension</param>
        private static void SaveTrainingSetToDisk<T>(List<T> listToSave, string gameObjectName)
        {
            // We make sure paths and filenames are set properly
            SetUpFileNamesAndPaths(gameObjectName);

            string subFolderPath = CheckOrCreateFoldersAndSubfoldersTrainingSet();

            // If the option to serialize witht JSON dot net is active...
            if (m_SerializeWithJSONDotNet)
            {
                // We save the entire input/output list as a JSON
                string auxFilePath = subFolderPath + "/" + m_FileTrainingSetName + "_Inputs_Outputs" + m_FileExtension;
                // Check if there is already a JSON file created for this training example
                if (File.Exists(auxFilePath))
                {
                    bool fileDeleted = false;
                    while (!fileDeleted)
                    {
                        try
                        {
                            // We delete it to make sure we override it
                            File.Delete(auxFilePath);
                            // If there was no exception, update flag
                            fileDeleted = true;
                        }
                        catch (Exception)
                        {
                            // The file might be in use, in that case, let's wait until next iteration to try again 
                            fileDeleted = false;
                        }

                    }
                }
                // Generate JSON string from the entire list
                //COMEBACK
                var jsonTrainingeExamplesList = JsonConvert.SerializeObject(listToSave, Formatting.Indented);
                //Debug.Log(jsonTrainingeExamplesList);
                // Write on the path
                File.WriteAllText(auxFilePath, jsonTrainingeExamplesList);
                Debug.Log("written to disk");
            }


        }

        /// <summary>
        /// Private method that saves Training Data Set to disk using a thread
        /// </summary>
        /// <param name="listToSave">The list of training examples</param>
        /// <param name="filePath">File path without file extension</param>
        private static async void SaveTrainingSetToDiskAsync<T>(List<T> listToSave, string gameObjectName)
        {
            // Launch the task in a thread
            await Task.Run(async () => 
            {
            // We make sure paths and filenames are set properly
            SetUpFileNamesAndPaths(gameObjectName);

            string subFolderPath = CheckOrCreateFoldersAndSubfoldersTrainingSet();

                // If the option to serialize witht JSON dot net is active...
                if (m_SerializeWithJSONDotNet)
                {
                    // We save the entire input/output list as a JSON
                    string auxFilePath = subFolderPath + "/" + m_FileTrainingSetName + "_Inputs_Outputs" + m_FileExtension;
                    // Check if there is already a JSON file created for this training example
                    if (File.Exists(auxFilePath))
                    {
                        bool fileDeleted = false;
                        while (!fileDeleted)
                        {
                            try
                            {
                                // We delete it to make sure we override it
                                File.Delete(auxFilePath);
                                // If there was no exception, update flag
                                fileDeleted = true;
                            }
                            catch (Exception)
                            {
                                // The file might be in use, in that case, let's wait until next iteration to try again 
                                fileDeleted = false;
                            }

                        }
                    }
                    // Generate JSON string from the entire list
                    //COMEBACK
                    var jsonTrainingeExamplesList = JsonConvert.SerializeObject(listToSave, Formatting.Indented);
                    //Debug.Log(jsonTrainingeExamplesList);
                    // Write on the path
                    using (var sw = new StreamWriter(auxFilePath))
                    {
                        await sw.WriteAsync(jsonTrainingeExamplesList);
                    }
                }
            });

        }

        /// <summary>
        /// Returns path for the Assets folder 
        /// </summary>
        /// <returns></returns>
        public static string GetAssetsPath()
        {
            SetUpIMLDataPath();
            return m_AppDataPath;
        }

        /// <summary>
        /// Returns path for InteractML/Data
        /// </summary>
        /// <returns></returns>
        public static string GetDataPath()
        {
            SetUpIMLDataPath();
            return Path.Combine(m_AppDataPath, m_FolderDataPathName);
        }

        /// <summary>
        /// Returns path for InteractML/Data/Training_Examples 
        /// </summary>
        /// <returns></returns>
        public static string GetTrainingExamplesDataPath()
        {
            SetUpIMLDataPath();
            return Path.Combine(m_AppDataPath, m_FolderDataPathName, "Training_Examples");
        }

        /// <summary>
        /// Returns path for InteractML/Data/Models 
        /// </summary>
        /// <returns></returns>
        public static string GetModelsDataPath()
        {
            SetUpIMLDataPath();
            return Path.Combine(GetDataPath(), "Models");
        }


        #endregion

    }

}
