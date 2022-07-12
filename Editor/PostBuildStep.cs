

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
                if (UtilsSetting.LoadInstance().UsePushNotification)
                    AddToEntitlements(buildTarget, pathToXcode);
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

            // Set value in plist
            plistRoot.SetString("NSUserTrackingUsageDescription", UtilsSetting.LoadInstance().AppTrackingConsentDescription);

            // save
            File.WriteAllText(plistPath, plistObj.WriteToString());
        }

        public static void AddToEntitlements(BuildTarget buildTarget, string buildPath)
        {
            if (buildTarget != BuildTarget.iOS) return;

            // get project info
            string pbxPath = PBXProject.GetPBXProjectPath(buildPath);
            var proj = new PBXProject();
            proj.ReadFromFile(pbxPath);
            var guid = proj.GetUnityMainTargetGuid();

            // get entitlements path
            string[] idArray = Application.identifier.Split('.');
            var entitlementsPath = $"Unity-iPhone/{idArray[idArray.Length - 1]}.entitlements";

            // create capabilities manager
            var capManager = new ProjectCapabilityManager(pbxPath, entitlementsPath, null, guid);

            // Add necessary capabilities
            capManager.AddPushNotifications(true);

            // Write to file
            capManager.WriteToFile();
        }
#endif

        [MenuItem("Tools/Omnilatent/iOS Utils Setting")]
        public static void OpenInspectorSetting()
        {
            Selection.activeObject = UtilsSetting.LoadInstance();
        }
    }
}