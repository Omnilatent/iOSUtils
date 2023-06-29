using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.iOS.Xcode;
using UnityEngine;

namespace Omnilatent.iOSUtils.Editor
{
    public class PListProcessor : MonoBehaviour
    {
        private const string KEY_SK_ADNETWORK_ITEMS = "SKAdNetworkItems";

        private const string KEY_SK_ADNETWORK_ID = "SKAdNetworkIdentifier";

        private static PlistElementArray GetSKAdNetworkItemsArray(PlistDocument document)
        {
            PlistElementArray array;
            if (document.root.values.ContainsKey(KEY_SK_ADNETWORK_ITEMS))
            {
                try
                {
                    PlistElement element;
                    document.root.values.TryGetValue(KEY_SK_ADNETWORK_ITEMS, out element);
                    array = element.AsArray();
                }
#pragma warning disable 0168
                catch (Exception e)
#pragma warning restore 0168
                {
                    // The element is not an array type.
                    array = null;
                }
            }
            else { array = document.root.CreateArray(KEY_SK_ADNETWORK_ITEMS); }

            return array;
        }

        public static void AddSKAdNetworkIdentifier(PlistDocument document, List<string> skAdNetworkIds)
        {
            PlistElementArray array = GetSKAdNetworkItemsArray(document);
            if (array != null)
            {
                foreach (string id in skAdNetworkIds)
                {
                    if (!ContainsSKAdNetworkIdentifier(array, id))
                    {
                        PlistElementDict added = array.AddDict();
                        added.SetString(KEY_SK_ADNETWORK_ID, id);
                    }
                }
            }
            else { NotifyBuildFailure("SKAdNetworkItems element already exists in Info.plist, but is not an array.", false); }
        }

        private static bool ContainsSKAdNetworkIdentifier(PlistElementArray skAdNetworkItemsArray, string id)
        {
            foreach (PlistElement elem in skAdNetworkItemsArray.values)
            {
                try
                {
                    PlistElementDict elemInDict = elem.AsDict();
                    PlistElement value;
                    bool identifierExists = elemInDict.values.TryGetValue(KEY_SK_ADNETWORK_ID, out value);

                    if (identifierExists && value.AsString().Equals(id)) { return true; }
                }
#pragma warning disable 0168
                catch (Exception e)
#pragma warning restore 0168
                {
                    // Do nothing
                }
            }

            return false;
        }

        private static void NotifyBuildFailure(string message, bool showOpenSettingsButton = true)
        {
            string dialogTitle = "iOS Utils";
            string dialogMessage = "Error: " + message;

            if (showOpenSettingsButton)
            {
                bool openSettings = EditorUtility.DisplayDialog(
                    dialogTitle, dialogMessage, "Open Settings", "Close");
            }
            else { EditorUtility.DisplayDialog(dialogTitle, dialogMessage, "Close"); }

            ThrowBuildException("[iOS Utils] " + message);
        }

        private static void ThrowBuildException(string message)
        {
#if UNITY_2017_1_OR_NEWER
            throw new BuildPlayerWindow.BuildMethodException(message);
#else
        throw new OperationCanceledException(message);
#endif
        }
    }
}