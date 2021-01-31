using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Diagnostics;
using System.IO;

/// <summary>
/// Methods to integrate IML files into the final build
/// </summary>
public class IMLBuildManager
{
    [InitializeOnLoadMethod]
    private static void Initialize()
    {
        BuildPlayerWindow.RegisterBuildPlayerHandler(BuildPlayerHandler);
    }

    /// <summary>
    /// Additional logic for the building process to include IML files post-build
    /// </summary>
    /// <param name="options"></param>
    private static void BuildPlayerHandler(BuildPlayerOptions options)
    {
        // Build the player as set by the unity buildplayer window
        BuildPlayerWindow.DefaultBuildMethods.BuildPlayer(options);

        // Get filename.
        string buildPath = options.locationPathName;        
        // Cut name of executable from path
        string executableName = Path.GetFileNameWithoutExtension(buildPath);
        buildPath = Path.GetDirectoryName(buildPath);
        string buildDataPath = Path.Combine(buildPath, executableName + "_Data");
        UnityEngine.Debug.Log("=========CUSTOM BUILD CODE==========");
        UnityEngine.Debug.Log("The app will be built at: " + buildPath);

        // Copy models and training data from the project folder to the build folder, alongside the built game.
        // Create IML directory
        if (!Directory.Exists(buildDataPath + "/InteractML/Data"))
        {
            Directory.CreateDirectory(buildDataPath + "/InteractML/Data");
            Directory.CreateDirectory(buildDataPath + "/InteractML/Data/Models");
            Directory.CreateDirectory(buildDataPath + "/InteractML/Data/Training_Examples");

        }
        // Calculate target paths
        string targetPathModels = buildDataPath + "/InteractML/Data/Models";
        string targetPathTrainingExamples = buildDataPath + "/InteractML/Data/Training_Examples";
        // Get all modelPaths
        string[] modelNames = Directory.GetFiles("Assets/InteractML/Data/Models");
        // Copy the models and overwrite destination files if they already exist.
        foreach (string modelName in modelNames)
        {
            // Use static Path methods to extract only the file name from the path.
            string fileName = Path.GetFileName(modelName);
            string destFile = Path.Combine(targetPathModels, fileName);
            File.Copy(modelName, destFile, true);
        }
        // Get all trainingExamplesPaths
        string[] trainingExamplesNames = Directory.GetFiles("Assets/InteractML/Data/Training_Examples");
        // Copy the models and overwrite destination files if they already exist.
        foreach (string tExamplesName in trainingExamplesNames)
        {
            // Use static Path methods to extract only the file name from the path.
            string fileName = Path.GetFileName(tExamplesName);
            string destFile = Path.Combine(targetPathTrainingExamples, fileName);
            try
            {               
                File.Copy(tExamplesName, destFile, true);
            }
            catch (System.Exception e)
            {

                UnityEngine.Debug.LogError(e.Message);
                
            }
            
        }


        //FileUtil.CopyFileOrDirectory("Assets/InteractML/Data/Models", buildDataPath + "/InteractML/Data");
        //FileUtil.CopyFileOrDirectory("Assets/InteractML/Data/Training_Examples", buildDataPath + "/InteractML/Data");

        UnityEngine.Debug.Log("Models in build: ");
        modelNames = Directory.GetFiles(buildDataPath + "/InteractML/Data/Models");
        foreach (string modelName in modelNames)
        {
            UnityEngine.Debug.Log(modelName);
        }
        UnityEngine.Debug.Log("Training Examples in build: ");
        trainingExamplesNames = Directory.GetFiles(buildDataPath + "/InteractML/Data/Training_Examples");
        foreach (string name in trainingExamplesNames)
        {
            UnityEngine.Debug.Log(name);
        }

        UnityEngine.Debug.Log("=========END OF CUSTOM BUILD CODE==========");

        // Run the game (Process class from System.Diagnostics).
        Process proc = new Process();
        proc.StartInfo.FileName = options.locationPathName;
        proc.Start();
    }

    [MenuItem("InteractML/Windows Build With Postprocess")]
    public static void BuildGame()
    {
        // Get filename.
        string path = EditorUtility.SaveFolderPanel("Choose Location of Built Game", "", "");
        string[] levels = new string[] { "Assets/Scene1.unity", "Assets/Scene2.unity" };

        // Build player.
        BuildPipeline.BuildPlayer(levels, path + "/BuiltGame.exe", BuildTarget.StandaloneWindows, BuildOptions.None);

        // Copy a models and training data from the project folder to the build folder, alongside the built game.
        FileUtil.CopyFileOrDirectory("Assets/InteractML/Data/Models", path + "InteractML/Data/Models");
        FileUtil.CopyFileOrDirectory("Assets/InteractML/Data/Training_Examples", path + "InteractML/Data/Training_Examples");

        // Run the game (Process class from System.Diagnostics).
        Process proc = new Process();
        proc.StartInfo.FileName = path + "/BuiltGame.exe";
        proc.Start();
    }

 
}
