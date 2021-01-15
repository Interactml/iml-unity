using System.Collections.Generic;

using UnityEngine;
using UnityEditor;
using UnityEditor.XR.Management.Metadata;

namespace Samples
{
    class SamplePackage : IXRPackage
    {
        class SampleLoaderMetadata : IXRLoaderMetadata 
        {
            public string loaderName { get; set; }
            public string loaderType { get; set; }
            public List<BuildTargetGroup> supportedBuildTargets { get; set; }
        }

        class SamplePackageMetadata : IXRPackageMetadata
        {
            public string packageName { get; set; }
            public string packageId { get; set; }
            public string settingsType { get; set; }
            public List<IXRLoaderMetadata> loaderMetadata { get; set; } 
        }

        private static IXRPackageMetadata s_Metadata = new SamplePackageMetadata() {
                packageName = "Sample Package <SAMPLE ONLY YOU MUST REIMPLEMENT>",
                packageId = "com.unity.xr.samplespackage",
                settingsType = typeof(SampleSettings).FullName,

                loaderMetadata = new List<IXRLoaderMetadata>() {
                    new SampleLoaderMetadata() {
                        loaderName = "Sample Loader One  <SAMPLE ONLY YOU MUST REIMPLEMENT>",
                        loaderType = typeof(SampleLoader).FullName,
                        supportedBuildTargets = new List<BuildTargetGroup>() {
                            BuildTargetGroup.Standalone,
                            BuildTargetGroup.WSA
                        }
                    },
                    new SampleLoaderMetadata() {
                        loaderName = "Sample Loader Two <SAMPLE ONLY YOU MUST REIMPLEMENT>",
                        loaderType = typeof(SampleLoader).FullName,
                        supportedBuildTargets = new List<BuildTargetGroup>() {
                            BuildTargetGroup.Android,
                            BuildTargetGroup.iOS,
                            BuildTargetGroup.Lumin
                        }
                    }
                }
        };

        public IXRPackageMetadata metadata => s_Metadata;

        public bool PopulateNewSettingsInstance(ScriptableObject obj)
        {
            return true;
        }

    }
}
