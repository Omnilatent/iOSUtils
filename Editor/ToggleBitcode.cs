#if UNITY_IOS
using UnityEditor;
using UnityEngine;
using UnityEditor.iOS.Xcode;

namespace Omnilatent.iOSUtils.Editor
{
    public class ToggleBitcode
    {
        public static void DisableBitcode(string pathToXcode)
        {
            string projectPath = pathToXcode + "/Unity-iPhone.xcodeproj/project.pbxproj";
            PBXProject pbxProject = new PBXProject();
            pbxProject.ReadFromFile(projectPath);

            //Disabling Bitcode on all targets
            //Main
            string target = pbxProject.GetUnityMainTargetGuid();
            pbxProject.SetBuildProperty(target, "ENABLE_BITCODE", "NO");

            //Unity Tests
            target = pbxProject.TargetGuidByName(PBXProject.GetUnityTestTargetName());
            pbxProject.SetBuildProperty(target, "ENABLE_BITCODE", "NO");

            //Unity Framework
            target = pbxProject.GetUnityFrameworkTargetGuid();
            pbxProject.SetBuildProperty(target, "ENABLE_BITCODE", "NO");

            pbxProject.WriteToFile(projectPath);
        }
    }
}
#endif