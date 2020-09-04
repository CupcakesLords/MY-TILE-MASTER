using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace AFramework
{
    //[InitializeOnLoad]
    //public class FrameworkStartup
    //{
    //    static FrameworkStartup()
    //    {
    //        FrameworkToolEditor.CheckFirebaseTool();
    //    }
    //}

    public static class FrameworkToolEditor
    {
        [MenuItem("AFramework/Delete Save")]
        public static void ClearSave()
        {
            AFramework.SaveGameManager.I.DeleteAll();
        }

        [MenuItem("AFramework/Build CommonLib")]
        public static void CheckFirebaseTool() { CheckFirebaseTool(true); }
        public static void CheckFirebaseTool(bool forceClear = true)
        {
            var libPath = "";
            {
                string fileName = "abcdef_lib_indicator";
                var targets = AssetDatabase.FindAssets(fileName);
                if (targets.Length <= 0)
                {
                    Debug.LogError("Could not found indicator for path");
                    return;
                }
                var temp = AssetDatabase.GUIDToAssetPath(targets[0]);
                libPath = temp.Substring(0, temp.LastIndexOf('/'));
            }
            var currentDirectory = Directory.GetCurrentDirectory();

            //firebase
            {
                if (Directory.Exists(currentDirectory + "/Assets/Firebase") && forceClear) Directory.Delete(currentDirectory + "/Assets/Firebase", true);

                if (!Directory.Exists(currentDirectory + "/Assets/Firebase"))
                {
                    Directory.CreateDirectory(currentDirectory + "/Assets/Firebase");
                    Directory.CreateDirectory(currentDirectory + "/Assets/Firebase/Editor");

                    FileUtil.CopyFileOrDirectory(
                        libPath + "/Firebase/Editor/generate_xml_from_google_services_json.exe",
                        currentDirectory + "/Assets/Firebase/Editor/generate_xml_from_google_services_json.exe"
                    );
                    FileUtil.CopyFileOrDirectory(
                        libPath + "/Firebase/Editor/generate_xml_from_google_services_json.py",
                        currentDirectory + "/Assets/Firebase/Editor/generate_xml_from_google_services_json.py"
                    );

                    FileUtil.CopyFileOrDirectory(
                        libPath + "/Firebase/Editor/network_request.exe",
                        currentDirectory + "/Assets/Firebase/Editor/network_request.exe"
                    );
                    FileUtil.CopyFileOrDirectory(
                        libPath + "/Firebase/Editor/network_request.py",
                        currentDirectory + "/Assets/Firebase/Editor/network_request.py"
                    );

                    var customDependency = currentDirectory + "/Assets/Firebase/Editor/AppDependencies.xml";
                    FileUtil.CopyFileOrDirectory(
                            libPath + "/Firebase/Editor/AppDependencies.xml",
                            customDependency
                            );

                    var fileText = System.IO.File.ReadAllText(customDependency);
                    fileText = fileText.Replace(
                        "Assets/Firebase/m2repository",
                        libPath + "/Firebase/m2repository");
                    System.IO.File.WriteAllText(customDependency, fileText);

                    if (Directory.Exists(currentDirectory + "/Assets/Plugins/Android/Firebase")) Directory.Delete(currentDirectory + "/Assets/Plugins/Android/Firebase", true);
                    Directory.CreateDirectory(currentDirectory + "/Assets/Plugins/Android/Firebase");

                    FileUtil.CopyFileOrDirectory(
                        libPath + "/Plugins/Android/Firebase/AndroidManifest.xml",
                        currentDirectory + "/Assets/Plugins/Android/Firebase/AndroidManifest.xml"
                    );
                    FileUtil.CopyFileOrDirectory(
                        libPath + "/Plugins/Android/Firebase/project.properties",
                        currentDirectory + "/Assets/Plugins/Android/Firebase/project.properties"
                    );
                }
            }

            /*
            //IronSource
            {
                if (Directory.Exists(currentDirectory + "/Assets/Plugins/Android/IronSource/") && forceClear) Directory.Delete(currentDirectory + "/Assets/Plugins/Android/IronSource/", true);

                if (!Directory.Exists(currentDirectory + "/Assets/Plugins/Android/IronSource/"))
                {
                    Directory.CreateDirectory(currentDirectory + "/Assets/Plugins/Android/IronSource/");

                    FileUtil.CopyFileOrDirectory(
                        libPath + "/Plugins/Android/IronSource/AndroidManifest.xml",
                        currentDirectory + "/Assets/Plugins/Android/IronSource/AndroidManifest.xml"
                    );
                    FileUtil.CopyFileOrDirectory(
                        libPath + "/Plugins/Android/IronSource/project.properties",
                        currentDirectory + "/Assets/Plugins/Android/IronSource/project.properties"
                    );

                    Directory.CreateDirectory(currentDirectory + "/Assets/Plugins/Android/IronSource/res/");
                    Directory.CreateDirectory(currentDirectory + "/Assets/Plugins/Android/IronSource/res/xml/");
                    FileUtil.CopyFileOrDirectory(
                        libPath + "/Plugins/Android/IronSource/res/xml/mtg_provider_paths.xml",
                        currentDirectory + "/Assets/Plugins/Android/IronSource/res/xml/mtg_provider_paths.xml"
                    );
                }
            }
            */

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.FocusProjectWindow();
        }
    }
}