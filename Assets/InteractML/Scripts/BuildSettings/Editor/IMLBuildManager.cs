using UnityEditor;
using System.IO;
#if UNITY_EDITOR
using UnityEngine;
#endif

/// <summary>
/// Methods to integrate IML files into the final build
/// </summary>
public class IMLBuildManager
{
    /// <summary>
    /// Delegate type storing different methods to call when the app builds
    /// </summary>
    public delegate void OnBuildEvent(BuildPlayerOptions options);
    /// <summary>
    /// Gets called when a build is produced
    /// </summary>
    public static OnBuildEvent OnBuildCallback;


    [InitializeOnLoadMethod]
    private static void Initialize()
    {
#if UNITY_EDITOR && UNITY_STANDALONE
        // Subscribe to event that fires when a build is produced
        BuildPlayerWindow.RegisterBuildPlayerHandler(BuildPlayerHandler);
#endif
    }

    /// <summary>
    /// Additional logic for the building process to include IML files post-build
    /// </summary>
    /// <param name="options"></param>
    private static void BuildPlayerHandler(BuildPlayerOptions options)
    {
#if UNITY_EDITOR
        // Build the player as set by the unity buildplayer window
        BuildPlayerWindow.DefaultBuildMethods.BuildPlayer(options);

        // Copy iml files to build
        CopyIMLFilesToBuild(options);

        // Call any additional methods specified by the user
        OnBuildCallback(options);

        //UnityEngine.Debug.Log("=========END OF CUSTOM BUILD CODE==========");

        // Run the game (Process class from System.Diagnostics).
        //Process proc = new Process();
        //proc.StartInfo.FileName = options.locationPathName;
        //proc.Start();
#endif
    }
 
    /// <summary>
    /// Copies all training examples and models present in the editor to the final build
    /// </summary>
    /// <param name="options"></param>
    private static void CopyIMLFilesToBuild(BuildPlayerOptions options)
    {
#if UNITY_STANDALONE
        // Calculate the datapath for the build
        string buildDataPath = GetBuildDataPath(options);

        // Copy models and training data from the project folder to the build folder, alongside the built game.
        // Create IML directory
        if (!Directory.Exists(buildDataPath + "/InteractML/Data"))
        {
            Directory.CreateDirectory(buildDataPath + "/InteractML/Data");
            Directory.CreateDirectory(buildDataPath + "/InteractML/Data/Models");
            Directory.CreateDirectory(buildDataPath + "/InteractML/Data/Training_Examples");
            Directory.CreateDirectory(buildDataPath + "/InteractML/Data/InputSetUp");

        }
        // Calculate target paths
        string targetPathModels = buildDataPath + "/InteractML/Data/Models";
        string targetPathTrainingExamples = buildDataPath + "/InteractML/Data/Training_Examples";
        string targetPathInputSettings = buildDataPath + "InteractML/Data/InputSetUp";

        // If the Models folder exists in current project...
        if (Directory.Exists("Assets/InteractML/Data/Models") && Directory.Exists(targetPathModels))
        {
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
        }

        // If the Training Examples folder exists in current project...
        if (Directory.Exists("Assets/InteractML/Data/Training_Examples") && Directory.Exists(targetPathTrainingExamples))
        {
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
        }

        // If the IML Input Settings folder exists in current project...
        if (Directory.Exists("Assets/InteractML/Data/InputSetUp") && Directory.Exists(targetPathInputSettings))
        {
            // Get all inputSettings files
            string[] inputSettingsFiles = Directory.GetFiles("Assets/InteractML/Data/InputSetUp");
            // Copy the models and overwrite destination files if they already exist.
            foreach (string inputSettingsFile in inputSettingsFiles)
            {
                // Use static Path methods to extract only the file name from the path.
                string fileName = Path.GetFileName(inputSettingsFile);
                string destFile = Path.Combine(targetPathInputSettings, fileName);
                File.Copy(inputSettingsFile, destFile, true);
            }

        }
#endif


    }

    /// <summary>
    /// Returns the data path inside a build
    /// </summary>
    /// <returns></returns>
    public static string GetBuildDataPath(BuildPlayerOptions options)
    {
        // Calculate the datapath for the build
        string buildDataPath = "";
#if UNITY_STANDALONE
        // Get filename.
        string buildPath = options.locationPathName;
        // Cut name of executable from path
        string executableName = Path.GetFileNameWithoutExtension(buildPath);
        buildPath = Path.GetDirectoryName(buildPath);
        buildDataPath = Path.Combine(buildPath, executableName + "_Data");
        //UnityEngine.Debug.Log("=========CUSTOM BUILD CODE==========");
        //UnityEngine.Debug.Log("The app will be built at: " + buildPath);
#elif UNITY_ANDROID
        // The datapath for the build will be the persistent one
        buildDataPath = Application.persistentDataPath;
# endif

        return buildDataPath;

    }
}
