/************************************************************************************

Copyright   :   Copyright (c) Facebook Technologies, LLC and its affiliates. All rights reserved.

Licensed under the Oculus SDK License Version 3.4.1 (the "License");
you may not use the Oculus SDK except in compliance with the License,
which is provided at the time of installation or download, or which
otherwise accompanies this software in either electronic or hard copy form.

You may obtain a copy of the License at

https://developer.oculus.com/licenses/sdk-3.4.1

Unless required by applicable law or agreed to in writing, the Oculus SDK
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.

************************************************************************************/

//#define BUILDSESSION

#if USING_XR_MANAGEMENT && USING_XR_SDK_OCULUS
#define USING_XR_SDK
#endif

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Diagnostics;
using System.Threading;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEditor.Build;
#if UNITY_2018_1_OR_NEWER
using UnityEditor.Build.Reporting;
#endif
#if UNITY_ANDROID
using UnityEditor.Android;
#endif

[InitializeOnLoad]
public class OVRGradleGeneration
#if UNITY_2018_2_OR_NEWER
	: IPreprocessBuildWithReport, IPostprocessBuildWithReport
#if UNITY_ANDROID
	, IPostGenerateGradleAndroidProject
#endif
{
	public OVRADBTool adbTool;
	public Process adbProcess;

	public int callbackOrder { get { return 3; } }
	static private System.DateTime buildStartTime;
	static private System.Guid buildGuid;

#if UNITY_ANDROID
	private const string prefName = "OVRAutoIncrementVersionCode_Enabled";
	private const string menuItemAutoIncVersion = "Oculus/Tools/Auto Increment Version Code";
	static bool autoIncrementVersion = false;
#endif

	static OVRGradleGeneration()
	{
		EditorApplication.delayCall += OnDelayCall;
	}

	static void OnDelayCall()
	{
#if UNITY_ANDROID
		autoIncrementVersion = PlayerPrefs.GetInt(prefName, 0) != 0;
		Menu.SetChecked(menuItemAutoIncVersion, autoIncrementVersion);
#endif
	}

#if UNITY_ANDROID
	[MenuItem(menuItemAutoIncVersion)]
	static void ToggleUtilities()
	{
		autoIncrementVersion = !autoIncrementVersion;
		Menu.SetChecked(menuItemAutoIncVersion, autoIncrementVersion);

		int newValue = (autoIncrementVersion) ? 1 : 0;
		PlayerPrefs.SetInt(prefName, newValue);
		PlayerPrefs.Save();

		UnityEngine.Debug.Log("Auto Increment Version Code: " + autoIncrementVersion);
	}
#endif

	public void OnPreprocessBuild(BuildReport report)
	{
#if UNITY_ANDROID && !(USING_XR_SDK && UNITY_2019_3_OR_NEWER)
		// Generate error when Vulkan is selected as the perferred graphics API, which is not currently supported in Unity XR
		if (!PlayerSettings.GetUseDefaultGraphicsAPIs(BuildTarget.Android))
		{
			GraphicsDeviceType[] apis = PlayerSettings.GetGraphicsAPIs(BuildTarget.Android);
			if (apis.Length >= 1 && apis[0] == GraphicsDeviceType.Vulkan)
			{
				throw new BuildFailedException("The Vulkan Graphics API does not support XR in your configuration. To use Vulkan, you must use Unity 2019.3 or newer, and the XR Plugin Management.");
			}
		}
#endif

		buildStartTime = System.DateTime.Now;
		buildGuid = System.Guid.NewGuid();

		if (!report.summary.outputPath.Contains("OVRGradleTempExport"))
		{
			OVRPlugin.SetDeveloperMode(OVRPlugin.Bool.True);
			OVRPlugin.AddCustomMetadata("build_type", "standard");
		}

		OVRPlugin.AddCustomMetadata("build_guid", buildGuid.ToString());
		OVRPlugin.AddCustomMetadata("target_platform", report.summary.platform.ToString());
#if !UNITY_2019_3_OR_NEWER
		OVRPlugin.AddCustomMetadata("scripting_runtime_version", UnityEditor.PlayerSettings.scriptingRuntimeVersion.ToString());
#endif
		if (report.summary.platform == UnityEditor.BuildTarget.StandaloneWindows
			|| report.summary.platform == UnityEditor.BuildTarget.StandaloneWindows64)
		{
			OVRPlugin.AddCustomMetadata("target_oculus_platform", "rift");
		}
#if BUILDSESSION
		StreamWriter writer = new StreamWriter("build_session", false);
		UnityEngine.Debug.LogFormat("Build Session: {0}", buildGuid.ToString());
		writer.WriteLine(buildGuid.ToString());
		writer.Close();
#endif
	}

	public void OnPostGenerateGradleAndroidProject(string path)
	{
		UnityEngine.Debug.Log("OVRGradleGeneration triggered.");

		var targetOculusPlatform = new List<string>();
		if (OVRDeviceSelector.isTargetDeviceGearVrOrGo)
		{
			targetOculusPlatform.Add("geargo");
		}
		if (OVRDeviceSelector.isTargetDeviceQuest)
		{
			targetOculusPlatform.Add("quest");
		}
		OVRPlugin.AddCustomMetadata("target_oculus_platform", String.Join("_", targetOculusPlatform.ToArray()));
		UnityEngine.Debug.LogFormat("  GearVR or Go = {0}  Quest = {1}", OVRDeviceSelector.isTargetDeviceGearVrOrGo, OVRDeviceSelector.isTargetDeviceQuest);

#if UNITY_2019_3_OR_NEWER
		string gradleBuildPath = Path.Combine(path, "../launcher/build.gradle");
#else
		string gradleBuildPath = Path.Combine(path, "build.gradle");
#endif
		//Enable v2signing for Quest only
		bool v2SigningEnabled = OVRDeviceSelector.isTargetDeviceQuest && !OVRDeviceSelector.isTargetDeviceGearVrOrGo;

		if (File.Exists(gradleBuildPath))
		{
			try
			{
				string gradle = File.ReadAllText(gradleBuildPath);
				int v2Signingindex = gradle.IndexOf("v2SigningEnabled false");

				if (v2Signingindex != -1)
				{
					//v2 Signing flag found, ensure the correct value is set based on platform.
					if (v2SigningEnabled)
					{
						gradle = gradle.Replace("v2SigningEnabled false", "v2SigningEnabled true");
						System.IO.File.WriteAllText(gradleBuildPath, gradle);
					}
				}
				else
				{
					//v2 Signing flag missing, add it right after the key store password and set the value based on platform.
					int keyPassIndex = gradle.IndexOf("keyPassword");
					if (keyPassIndex != -1)
					{
						int v2Index = gradle.IndexOf("\n", keyPassIndex) + 1;
						if(v2Index != -1)
						{
							gradle = gradle.Insert(v2Index, "v2SigningEnabled " + (v2SigningEnabled ? "true" : "false") + "\n");
							System.IO.File.WriteAllText(gradleBuildPath, gradle);
						}
					}
				}
			}
			catch (System.Exception e)
			{
				UnityEngine.Debug.LogWarningFormat("Unable to overwrite build.gradle, error {0}", e.Message);
			}
		}
		else
		{
			UnityEngine.Debug.LogWarning("Unable to locate build.gradle");
		}

		PatchAndroidManifest(path);
	}

	public void PatchAndroidManifest(string path)
	{
		string manifestFolder = Path.Combine(path, "src/main");
		try
		{
			// Load android manfiest file
			XmlDocument doc = new XmlDocument();
			doc.Load(manifestFolder + "/AndroidManifest.xml");

			string androidNamepsaceURI;
			XmlElement element = (XmlElement)doc.SelectSingleNode("/manifest");
			if (element == null)
			{
				UnityEngine.Debug.LogError("Could not find manifest tag in android manifest.");
				return;
			}

			// Get android namespace URI from the manifest
			androidNamepsaceURI = element.GetAttribute("xmlns:android");
			if (!string.IsNullOrEmpty(androidNamepsaceURI))
			{
				// Look for intent filter category and change LAUNCHER to INFO
				XmlNodeList nodeList = doc.SelectNodes("/manifest/application/activity/intent-filter/category");
				foreach (XmlElement e in nodeList)
				{
					string attr = e.GetAttribute("name", androidNamepsaceURI);
					if (attr == "android.intent.category.LAUNCHER")
					{
						e.SetAttribute("name", androidNamepsaceURI, "android.intent.category.INFO");
					}
				}

				//If Quest is the target device, add the headtracking manifest tag
				if (OVRDeviceSelector.isTargetDeviceQuest)
				{
					XmlNodeList manifestUsesFeatureNodes = doc.SelectNodes("/manifest/uses-feature");
					bool foundHeadtrackingTag = false;
					foreach (XmlElement e in manifestUsesFeatureNodes)
					{
						string attr = e.GetAttribute("name", androidNamepsaceURI);
						if (attr == "android.hardware.vr.headtracking")
							foundHeadtrackingTag = true;
					}
					//If the tag already exists, don't patch with a new one. If it doesn't, we add it.
					if (!foundHeadtrackingTag)
					{
						XmlNode manifestElement = doc.SelectSingleNode("/manifest");
						XmlElement headtrackingTag = doc.CreateElement("uses-feature");
						headtrackingTag.SetAttribute("name", androidNamepsaceURI, "android.hardware.vr.headtracking");
						headtrackingTag.SetAttribute("version", androidNamepsaceURI, "1");
						string tagRequired = OVRDeviceSelector.isTargetDeviceGearVrOrGo ? "false" : "true";
						headtrackingTag.SetAttribute("required", androidNamepsaceURI, tagRequired);
						manifestElement.AppendChild(headtrackingTag);
					}
				}

				// If Quest is the target device, add the handtracking manifest tags if needed
				// Mapping of project setting to manifest setting:
				// OVRProjectConfig.HandTrackingSupport.ControllersOnly => manifest entry not present
				// OVRProjectConfig.HandTrackingSupport.ControllersAndHands => manifest entry present and required=false
				// OVRProjectConfig.HandTrackingSupport.HandsOnly => manifest entry present and required=true
				if (OVRDeviceSelector.isTargetDeviceQuest)
				{
					OVRProjectConfig.HandTrackingSupport targetHandTrackingSupport = OVRProjectConfig.GetProjectConfig().handTrackingSupport;
					bool handTrackingEntryNeeded = (targetHandTrackingSupport != OVRProjectConfig.HandTrackingSupport.ControllersOnly);
					if (handTrackingEntryNeeded)
					{
						// uses-feature: <uses-feature android:name="oculus.software.handtracking" android:required="false" />
						XmlNodeList manifestUsesFeatureNodes = doc.SelectNodes("/manifest/uses-feature");
						bool foundHandTrackingFeature = false;
						foreach (XmlElement e in manifestUsesFeatureNodes)
						{
							string attr = e.GetAttribute("name", androidNamepsaceURI);
							if (attr == "oculus.software.handtracking")
								foundHandTrackingFeature = true;
						}
						//If the tag already exists, don't patch with a new one. If it doesn't, we add it.
						if (!foundHandTrackingFeature)
						{
							XmlNode manifestElement = doc.SelectSingleNode("/manifest");
							XmlElement handTrackingFeature = doc.CreateElement("uses-feature");
							handTrackingFeature.SetAttribute("name", androidNamepsaceURI, "oculus.software.handtracking");
							string tagRequired = (targetHandTrackingSupport == OVRProjectConfig.HandTrackingSupport.HandsOnly) ? "true" : "false";
							handTrackingFeature.SetAttribute("required", androidNamepsaceURI, tagRequired);
							manifestElement.AppendChild(handTrackingFeature);
						}

						// uses-permission: <uses-permission android:name="oculus.permission.handtracking" />
						XmlNodeList manifestUsesPermissionNodes = doc.SelectNodes("/manifest/uses-permission");
						bool foundHandTrackingPermission = false;
						foreach (XmlElement e in manifestUsesPermissionNodes)
						{
							string attr = e.GetAttribute("name", androidNamepsaceURI);
							if (attr == "oculus.permission.handtracking")
								foundHandTrackingPermission = true;
						}
						//If the tag already exists, don't patch with a new one. If it doesn't, we add it.
						if (!foundHandTrackingPermission)
						{
							XmlNode manifestElement = doc.SelectSingleNode("/manifest");
							XmlElement handTrackingPermission = doc.CreateElement("uses-permission");
							handTrackingPermission.SetAttribute("name", androidNamepsaceURI, "oculus.permission.handtracking");
							manifestElement.AppendChild(handTrackingPermission);
						}
					}
				}

				XmlElement applicationNode = (XmlElement)doc.SelectSingleNode("/manifest/application");
				if(applicationNode != null)
				{
					// If android label and icon are missing from the xml, add them
					if (applicationNode.GetAttribute("android:label") == null)
					{
						applicationNode.SetAttribute("label", androidNamepsaceURI, "@string/app_name");
					}
					if (applicationNode.GetAttribute("android:icon") == null)
					{
						applicationNode.SetAttribute("icon", androidNamepsaceURI, "@mipmap/app_icon");
					}

					// Check for VR tag, if missing, append it
					bool vrTagFound = false;
					XmlNodeList appNodeList = applicationNode.ChildNodes;
					foreach (XmlElement e in appNodeList)
					{
						if (e.GetAttribute("android:name") == "com.samsung.android.vr.application.mode")
						{
							vrTagFound = true;
							break;
						}
					}

					if (!vrTagFound)
					{
						XmlElement vrTag = doc.CreateElement("meta-data");
						vrTag.SetAttribute("name", androidNamepsaceURI, "com.samsung.android.vr.application.mode");
						vrTag.SetAttribute("value", androidNamepsaceURI, "vr_only");
						applicationNode.AppendChild(vrTag); ;
					}

					// Disable allowBackup in manifest and add Android NSC XML file
					OVRProjectConfig projectConfig = OVRProjectConfig.GetProjectConfig();
					if (projectConfig != null)
					{
						if (projectConfig.disableBackups)
						{
							applicationNode.SetAttribute("allowBackup", androidNamepsaceURI, "false");
						}

						if (projectConfig.enableNSCConfig)
						{
							applicationNode.SetAttribute("networkSecurityConfig", androidNamepsaceURI, "@xml/network_sec_config");

							string securityConfigFile = GetOculusProjectNetworkSecConfigPath();
							string xmlDirectory = Path.Combine(path, "src/main/res/xml");
							try
							{
								if (!Directory.Exists(xmlDirectory))
								{
									Directory.CreateDirectory(xmlDirectory);
								}
								File.Copy(securityConfigFile, Path.Combine(xmlDirectory, "network_sec_config.xml"), true);
							}
							catch (Exception e)
							{
								UnityEngine.Debug.LogError(e.Message);
							}
						}

						// If only targeting Quest, check for focus aware support
						if (OVRDeviceSelector.isTargetDeviceQuest)
						{
							if (projectConfig.focusAware)
							{
								XmlElement activityNode = (XmlElement)doc.SelectSingleNode("/manifest/application/activity");
								if (activityNode != null)
								{
									XmlElement focusAwareTag = doc.CreateElement("meta-data");
									focusAwareTag.SetAttribute("name", androidNamepsaceURI, "com.oculus.vr.focusaware");
									focusAwareTag.SetAttribute("value", androidNamepsaceURI, "true");
									activityNode.AppendChild(focusAwareTag);
								}
							}
						}
					}
				}
				doc.Save(manifestFolder + "/AndroidManifest.xml");
			}
		}
		catch (Exception e)
		{
			UnityEngine.Debug.LogError(e.Message);
		}
	}

	private static string GetOculusProjectNetworkSecConfigPath()
	{
		var so = ScriptableObject.CreateInstance(typeof(OVRPluginUpdaterStub));
		var script = MonoScript.FromScriptableObject(so);
		string assetPath = AssetDatabase.GetAssetPath(script);
		string editorDir = Directory.GetParent(assetPath).FullName;
		string configAssetPath = Path.GetFullPath(Path.Combine(editorDir, "network_sec_config.xml"));
		Uri configUri = new Uri(configAssetPath);
		Uri projectUri = new Uri(Application.dataPath);
		Uri relativeUri = projectUri.MakeRelativeUri(configUri);

		return relativeUri.ToString();
	}

	public void OnPostprocessBuild(BuildReport report)
	{
#if UNITY_ANDROID
		if(autoIncrementVersion)
		{
			if((report.summary.options & BuildOptions.Development) == 0)
			{
				PlayerSettings.Android.bundleVersionCode++;
				UnityEngine.Debug.Log("Incrementing version code to " + PlayerSettings.Android.bundleVersionCode);
			}
		}

		bool isExporting = true;
		foreach (var step in report.steps)
		{
			if (step.name.Contains("Compile scripts")
				|| step.name.Contains("Building scenes")
				|| step.name.Contains("Writing asset files")
				|| step.name.Contains("Preparing APK resources")
				|| step.name.Contains("Creating Android manifest")
				|| step.name.Contains("Processing plugins")
				|| step.name.Contains("Exporting project")
				|| step.name.Contains("Building Gradle project"))
			{
				OVRPlugin.SendEvent("build_step_" + step.name.ToLower().Replace(' ', '_'),
					step.duration.TotalSeconds.ToString(), "ovrbuild");
#if BUILDSESSION
				UnityEngine.Debug.LogFormat("build_step_" + step.name.ToLower().Replace(' ', '_') + ": {0}", step.duration.TotalSeconds.ToString());
#endif
				if(step.name.Contains("Building Gradle project"))
				{
					isExporting = false;
				}
			}
		}
		OVRPlugin.AddCustomMetadata("build_step_count", report.steps.Length.ToString());
		if (report.summary.outputPath.Contains("apk")) // Exclude Gradle Project Output
		{
			var fileInfo = new System.IO.FileInfo(report.summary.outputPath);
			OVRPlugin.AddCustomMetadata("build_output_size", fileInfo.Length.ToString());
		}
#endif
		if (!report.summary.outputPath.Contains("OVRGradleTempExport"))
		{
			OVRPlugin.SendEvent("build_complete", (System.DateTime.Now - buildStartTime).TotalSeconds.ToString(), "ovrbuild");
#if BUILDSESSION
			UnityEngine.Debug.LogFormat("build_complete: {0}", (System.DateTime.Now - buildStartTime).TotalSeconds.ToString());
#endif
		}

#if UNITY_ANDROID
		if (!isExporting)
		{
			// Get the hosts path to Android SDK
			if (adbTool == null)
			{
				adbTool = new OVRADBTool(OVRConfig.Instance.GetAndroidSDKPath(false));
			}

			if (adbTool.isReady)
			{
				// Check to see if there are any ADB devices connected before continuing.
				List<string> devices = adbTool.GetDevices();
				if(devices.Count == 0)
				{
					return;
				}

				// Clear current logs on device
				Process adbClearProcess;
				adbClearProcess = adbTool.RunCommandAsync(new string[] { "logcat --clear" }, null);

				// Add a timeout if we cannot get a response from adb logcat --clear in time.
				Stopwatch timeout = new Stopwatch();
				timeout.Start();
				while (!adbClearProcess.WaitForExit(100))
				{
					if (timeout.ElapsedMilliseconds > 2000)
					{
						adbClearProcess.Kill();
						return;
					}
				}

				// Check if existing ADB process is still running, kill if needed
				if (adbProcess != null && !adbProcess.HasExited)
				{
					adbProcess.Kill();
				}

				// Begin thread to time upload and install
				var thread = new Thread(delegate ()
				{
					TimeDeploy();
				});
				thread.Start();
			}
		}
#endif
	}

#if UNITY_ANDROID
	public bool WaitForProcess;
	public bool TransferStarted;
	public DateTime UploadStart;
	public DateTime UploadEnd;
	public DateTime InstallEnd;

	public void TimeDeploy()
	{
		if (adbTool != null)
		{
			TransferStarted = false;
			DataReceivedEventHandler outputRecieved = new DataReceivedEventHandler(
				(s, e) =>
				{
					if (e.Data != null && e.Data.Length != 0 && !e.Data.Contains("\u001b"))
					{
						if (e.Data.Contains("free_cache"))
						{
							// Device recieved install command and is starting upload
							UploadStart = System.DateTime.Now;
							TransferStarted = true;
						}
						else if (e.Data.Contains("Running dexopt"))
						{
							// Upload has finished and Package Manager is starting install
							UploadEnd = System.DateTime.Now;
						}
						else if (e.Data.Contains("dex2oat took"))
						{
							// Package Manager finished install
							InstallEnd = System.DateTime.Now;
							WaitForProcess = false;
						}
						else if (e.Data.Contains("W PackageManager"))
						{
							// Warning from Package Manager is a failure in the install process
							WaitForProcess = false;
						}
					}
				}
			);

			WaitForProcess = true;
			adbProcess = adbTool.RunCommandAsync(new string[] { "logcat" }, outputRecieved);

			Stopwatch transferTimeout = new Stopwatch();
			transferTimeout.Start();
			while (adbProcess != null && !adbProcess.WaitForExit(100))
			{
				if (!WaitForProcess)
				{
					adbProcess.Kill();
					float UploadTime = (float)(UploadEnd - UploadStart).TotalMilliseconds / 1000f;
					float InstallTime = (float)(InstallEnd - UploadEnd).TotalMilliseconds / 1000f;

					if (UploadTime > 0f)
					{
						OVRPlugin.SendEvent("deploy_task", UploadTime.ToString(), "ovrbuild");
					}
					if (InstallTime > 0f)
					{
						OVRPlugin.SendEvent("install_task", InstallTime.ToString(), "ovrbuild");
					}
				}

				if (!TransferStarted && transferTimeout.ElapsedMilliseconds > 5000)
				{
					adbProcess.Kill();
				}
			}
		}
	}
#endif
#else
{
#endif
}
