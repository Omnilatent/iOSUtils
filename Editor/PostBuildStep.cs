

using UnityEditor;
using UnityEditor.Callbacks;
#if UNITY_IOS
using UnityEditor.iOS.Xcode;
#endif
using System.IO;
using UnityEngine;

namespace Omnilatent.iOSUtils.Editor
{
    public class PostBuildStep
    {
#if UNITY_IOS
        /// <summary>
        /// Description for IDFA request notification 
        /// [sets NSUserTrackingUsageDescription]
        /// </summary>
        //const string TrackingDescription =
        //    "Your data will be used to provide you a better and personalized ad experience.";

        [PostProcessBuild(0)]
        public static void OnPostprocessBuild(BuildTarget buildTarget, string pathToXcode)
        {
            if (buildTarget == BuildTarget.iOS)
            {
                AddPListValues(pathToXcode);
                UpdatePBXProject(buildTarget, pathToXcode);
                if (!UtilsSetting.LoadInstance().EnableBitcode)
                {
                    ToggleBitcode.DisableBitcode(pathToXcode);
                }
            }
        }

        static void AddPListValues(string pathToXcode)
        {
            // Get Plist from Xcode project 
            string plistPath = pathToXcode + "/Info.plist";

            // Read in Plist 
            PlistDocument plistObj = new PlistDocument();
            plistObj.ReadFromString(File.ReadAllText(plistPath));

            // set values from the root obj
            PlistElementDict plistRoot = plistObj.root;

            // Set Tracking Usage Description
            plistRoot.SetString("NSUserTrackingUsageDescription", UtilsSetting.LoadInstance().AppTrackingConsentDescription);

            // Set ITSAppUsesNonExemptEncryption
            plistRoot.SetBoolean("ITSAppUsesNonExemptEncryption", UtilsSetting.LoadInstance().UseNonExemptEncryption);

            if (!string.IsNullOrEmpty(UtilsSetting.LoadInstance().FacebookClientToken))
            {
                // Change value of FacebookClientToken in Xcode plist
                var buildKey = "FacebookClientToken";
                plistRoot.SetString(buildKey, UtilsSetting.LoadInstance().FacebookClientToken);
            }
            
            // Add SKAdNetworkIdentifiers
            if (UtilsSetting.LoadInstance().SKAdNetworkIdentifiers != null && UtilsSetting.LoadInstance().SKAdNetworkIdentifiers.Count > 0)
            {
                PListProcessor.AddSKAdNetworkIdentifier(plistObj, UtilsSetting.LoadInstance().SKAdNetworkIdentifiers);
            } 

            // save
            File.WriteAllText(plistPath, plistObj.WriteToString());
        }

        public static void UpdatePBXProject(BuildTarget buildTarget, string buildPath)
        {
            if (buildTarget != BuildTarget.iOS) return;

            // get project info
            string pbxPath = PBXProject.GetPBXProjectPath(buildPath);
            var project = new PBXProject();
            project.ReadFromFile(pbxPath);
            var mainTargetGuid = project.GetUnityMainTargetGuid();
            
            if (UtilsSetting.LoadInstance().UsePushNotification)
            {
                // get entitlements path
                string[] idArray = Application.identifier.Split('.');
                var entitlementsPath = $"Unity-iPhone/{idArray[idArray.Length - 1]}.entitlements";

                // create capabilities manager
                var capManager = new ProjectCapabilityManager(pbxPath, entitlementsPath, null, mainTargetGuid);

                // Add necessary capabilities
                capManager.AddPushNotifications(true);

                // Write to file
                capManager.WriteToFile();
            }

            if (!UtilsSetting.LoadInstance().EnableAlwaysEmbedSwiftStandardLibraries)
            {
                foreach (var targetGuid in new[] { mainTargetGuid, project.GetUnityFrameworkTargetGuid() })
                {
                    project.SetBuildProperty(targetGuid, "ALWAYS_EMBED_SWIFT_STANDARD_LIBRARIES", "NO");
                }

                project.SetBuildProperty(mainTargetGuid, "ALWAYS_EMBED_SWIFT_STANDARD_LIBRARIES", "NO");
                project.WriteToFile(pbxPath);
            }
        }
#endif

        [MenuItem("Tools/Omnilatent/iOS Utils Setting")]
        public static void OpenInspectorSetting()
        {
            Selection.activeObject = UtilsSetting.LoadInstance();
        }
    }
}