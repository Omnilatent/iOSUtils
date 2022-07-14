using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Omnilatent.iOSUtils.Editor
{
    public class UtilsSetting : ScriptableObject
    {
        [Header("Capabilities")]
        [SerializeField] bool usePushNotification = true;
        public bool UsePushNotification { get => usePushNotification; }

        [Header("App Tracking Consent Flow")]
        [SerializeField] string appTrackingConsentDescription = "Your data will be used to provide you a better and personalized ad experience.";
        public string AppTrackingConsentDescription { get => appTrackingConsentDescription; }

        [Header("Other")]
        [SerializeField] bool useNonExemptEncryption = false;
        public bool UseNonExemptEncryption { get => useNonExemptEncryption; }

        private const string settingsResDir = "Assets/Omnilatent/Resources";

        private const string settingsFile = "iOSUtilsSetting";

        private const string settingsFileExtension = ".asset";
        internal static UtilsSetting LoadInstance()
        {
            //Read from resources.
            var instance = Resources.Load<UtilsSetting>(settingsFile);

            //Create instance if null.
            if (instance == null)
            {
                Directory.CreateDirectory(settingsResDir);
                instance = ScriptableObject.CreateInstance<UtilsSetting>();
                string assetPath = Path.Combine(
                    settingsResDir,
                    settingsFile + settingsFileExtension);
                AssetDatabase.CreateAsset(instance, assetPath);
                AssetDatabase.SaveAssets();
            }

            return instance;
        }
    }
}