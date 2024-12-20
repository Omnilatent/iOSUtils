using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Reflection;
using System.Text;
using AppIconChanger.Editor;
using UnityEditor.Android;
using UnityEditor.Build;

public class AlternateIconSetup : EditorWindow
{
    private AlternateIcon _alternateIcon;
    private Texture2D gameIcon;
    private const string imageTail = ".png";

    [MenuItem("Tools/Alternate icon setup")]
    static void Init()
    {
        var window = (AlternateIconSetup)GetWindow(typeof(AlternateIconSetup));
        window.Show();
    }

    private void OnGUI()
    {
        gameIcon = (Texture2D)EditorGUILayout.ObjectField("Game Icon", gameIcon, typeof(Texture2D), false);

        _alternateIcon = (AlternateIcon)EditorGUILayout.ObjectField("Alternate Icon",
            _alternateIcon, typeof(AlternateIcon), false);

        if (GUILayout.Button("Generate & Setup"))
        {
            if (gameIcon == null)
            {
                Debug.LogError("Please assign a game icon.");
                return;
            }

            try
            {
                string gameIconPath = AssetDatabase.GetAssetPath(gameIcon);
                string savePath = Path.GetDirectoryName(gameIconPath);
                if (_alternateIcon == null)
                {
                    var fileName = Path.GetFileNameWithoutExtension(gameIconPath);
                    var basePath = Path.Combine(savePath, $"{fileName}.asset");

                    var overwrite = true;
                    if (File.Exists(basePath))
                    {
                        var result = EditorUtility.DisplayDialogComplex("Warning",
                            $"A file already exists at\n\n{basePath}\n\nWhat do you want to do?", "Overwrite",
                            "Generate Unique Name", "Skip");
                        if (result == 2)
                        {
                            return;
                        }

                        overwrite = (result == 0);
                    }

                    if (!overwrite)
                    {
                        basePath = AssetDatabase.GenerateUniqueAssetPath(basePath);
                    }

                    var icon = CreateInstance<AlternateIcon>();
                    icon.iconName = fileName;
                    icon.type = AlternateIconType.Manual;
                    if (overwrite)
                    {
                        AssetDatabase.DeleteAsset(basePath);
                    }

                    AssetDatabase.CreateAsset(icon, basePath);
                    _alternateIcon = (AlternateIcon)AssetDatabase.LoadAssetAtPath(basePath, typeof(AlternateIcon));
                }

                FieldInfo[] fields = typeof(AlternateIcon).GetFields(BindingFlags.Public | BindingFlags.Instance);
                int count = 0;
                foreach (var field in fields)
                {
                    if (field.FieldType == typeof(Texture2D))
                    {
                        float progress = (float)count / fields.Length;
                        EditorUtility.DisplayProgressBar("Generating Textures", "Creating Texture", progress);
                        count++;
                        var iconName = field.Name + imageTail;
                        var iconSize = GetIconSize(field.Name);
                        var path = Path.Combine(savePath, iconName);
                        if (iconSize > 0)
                            SaveIcon(gameIcon, iconSize, path);
                    }
                }
            }
            finally
            {
                // Clear the progress bar
                EditorUtility.ClearProgressBar();
                AssetDatabase.Refresh();
            }
        }

        if (GUILayout.Button("Apply Icons"))
        {
            if (gameIcon == null)
            {
                Debug.LogError("Please assign a game icon.");
                return;
            }

            ApplyIcon();
        }

        return;

        void ApplyIcon()
        {
            try
            {
                string gameIconPath = AssetDatabase.GetAssetPath(gameIcon);
                string savePath = Path.GetDirectoryName(gameIconPath);
                _alternateIcon.appStore1024px =
                    (Texture2D)AssetDatabase.LoadAssetAtPath(Path.Combine(savePath, "appStore1024px" + imageTail),
                        typeof(Texture2D));
                _alternateIcon.iPhoneNotification40px =
                    (Texture2D)AssetDatabase.LoadAssetAtPath(
                        Path.Combine(savePath, "iPhoneNotification40px" + imageTail),
                        typeof(Texture2D));
                _alternateIcon.iPhoneNotification60px =
                    (Texture2D)AssetDatabase.LoadAssetAtPath(
                        Path.Combine(savePath, "iPhoneNotification60px" + imageTail),
                        typeof(Texture2D));
                _alternateIcon.iPhoneSettings58px =
                    (Texture2D)AssetDatabase.LoadAssetAtPath(Path.Combine(savePath, "iPhoneSettings58px" + imageTail),
                        typeof(Texture2D));
                _alternateIcon.iPhoneSettings87px =
                    (Texture2D)AssetDatabase.LoadAssetAtPath(Path.Combine(savePath, "iPhoneSettings87px" + imageTail),
                        typeof(Texture2D));
                _alternateIcon.iPhoneSpotlight80px =
                    (Texture2D)AssetDatabase.LoadAssetAtPath(Path.Combine(savePath, "iPhoneSpotlight80px" + imageTail),
                        typeof(Texture2D));
                _alternateIcon.iPhoneSpotlight120px =
                    (Texture2D)AssetDatabase.LoadAssetAtPath(Path.Combine(savePath, "iPhoneSpotlight120px" + imageTail),
                        typeof(Texture2D));
                _alternateIcon.iPhoneApp120px =
                    (Texture2D)AssetDatabase.LoadAssetAtPath(Path.Combine(savePath, "iPhoneApp120px" + imageTail),
                        typeof(Texture2D));
                _alternateIcon.iPhoneApp180px =
                    (Texture2D)AssetDatabase.LoadAssetAtPath(Path.Combine(savePath, "iPhoneApp180px" + imageTail),
                        typeof(Texture2D));
                _alternateIcon.iPadNotifications20px =
                    (Texture2D)AssetDatabase.LoadAssetAtPath(
                        Path.Combine(savePath, "iPadNotifications20px" + imageTail),
                        typeof(Texture2D));
                _alternateIcon.iPadNotifications40px =
                    (Texture2D)AssetDatabase.LoadAssetAtPath(
                        Path.Combine(savePath, "iPadNotifications40px" + imageTail),
                        typeof(Texture2D));
                _alternateIcon.iPadSettings29px =
                    (Texture2D)AssetDatabase.LoadAssetAtPath(Path.Combine(savePath, "iPadSettings29px" + imageTail),
                        typeof(Texture2D));
                _alternateIcon.iPadSettings58px =
                    (Texture2D)AssetDatabase.LoadAssetAtPath(Path.Combine(savePath, "iPadSettings58px" + imageTail),
                        typeof(Texture2D));
                _alternateIcon.iPadSpotlight40px =
                    (Texture2D)AssetDatabase.LoadAssetAtPath(Path.Combine(savePath, "iPadSpotlight40px" + imageTail),
                        typeof(Texture2D));
                _alternateIcon.iPadSpotlight80px =
                    (Texture2D)AssetDatabase.LoadAssetAtPath(Path.Combine(savePath, "iPadSpotlight80px" + imageTail),
                        typeof(Texture2D));
                _alternateIcon.iPadApp76px =
                    (Texture2D)AssetDatabase.LoadAssetAtPath(Path.Combine(savePath, "iPadApp76px" + imageTail),
                        typeof(Texture2D));
                _alternateIcon.iPadApp152px =
                    (Texture2D)AssetDatabase.LoadAssetAtPath(Path.Combine(savePath, "iPadApp152px" + imageTail),
                        typeof(Texture2D));
                _alternateIcon.iPadProApp167px =
                    (Texture2D)AssetDatabase.LoadAssetAtPath(Path.Combine(savePath, "iPadProApp167px" + imageTail),
                        typeof(Texture2D));
                EditorUtility.SetDirty(_alternateIcon);
            }
            finally
            {
                Selection.activeObject = _alternateIcon;
            }
        }
    }

    private int GetIconSize(string iconName)
    {
        StringBuilder numberBuilder = new StringBuilder();

        foreach (char c in iconName)
        {
            if (char.IsDigit(c))
            {
                numberBuilder.Append(c);
            }
        }

        if (numberBuilder.Length > 0)
        {
            return int.Parse(numberBuilder.ToString());
        }

        return -1; // Return -1 if no number is found
    }

    private static void SaveIcon(Texture2D sourceTexture, int size, string savePath)
    {
        var iconTexture = new Texture2D(0, 0);
        iconTexture.LoadImage(File.ReadAllBytes(AssetDatabase.GetAssetPath(sourceTexture)));

        if (iconTexture.width != size || iconTexture.height != size)
        {
            var renderTexture = new RenderTexture(size, size, 24);
            var tmpRenderTexture = RenderTexture.active;
            RenderTexture.active = renderTexture;
            Graphics.Blit(iconTexture, renderTexture);
            var resizedTexture = new Texture2D(size, size);
            resizedTexture.ReadPixels(new Rect(0, 0, size, size), 0, 0);
            resizedTexture.Apply();
            RenderTexture.active = tmpRenderTexture;
            renderTexture.Release();
            iconTexture = resizedTexture;
        }

        var pngBytes = iconTexture.EncodeToPNG();
        File.WriteAllBytes(savePath, pngBytes);
    }
}