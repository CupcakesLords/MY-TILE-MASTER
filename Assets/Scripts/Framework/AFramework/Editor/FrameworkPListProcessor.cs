using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_IPHONE || UNITY_IOS
using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
#endif

namespace AFramework
{
    public static class FrameworkPListProcessor
    {
#if UNITY_IPHONE || UNITY_IOS
        [PostProcessBuild(100)]
        public static void OnPostProcessBuild(BuildTarget buildTarget, string path)
        {
            string plistPath = Path.Combine(path, "Info.plist");
            PlistDocument plist = new PlistDocument();
            plist.ReadFromFile(plistPath);

            if (FrameworkGlobalConfig.Instance != null)
            {
                if (FrameworkGlobalConfig.Instance.AdsConfig != null && !string.IsNullOrEmpty(FrameworkGlobalConfig.Instance.AdsConfig.Platform.AppId))
                {
                    string appId = FrameworkGlobalConfig.Instance.AdsConfig.Platform.AppId;
                    if (appId.Length == 0)
                    {
                        Debug.LogError("iOS AdMob app ID is empty. Please enter a valid app ID to run ads properly.");
                    }
                    else
                    {
                        plist.root.SetString("GADApplicationIdentifier", appId);
                    }
                }

#if USE_ADMOB
                if (FrameworkGlobalConfig.Instance.AdsConfig != null && !string.IsNullOrEmpty(FrameworkGlobalConfig.Instance.AdsConfig.Platform.ApplovingSDKKey))
                {
                    string applovinSDKKey = FrameworkGlobalConfig.Instance.AdsConfig.Platform.ApplovingSDKKey;
                    if (applovinSDKKey.Length == 0)
                    {
                        Debug.LogError("iOS Applovin SDK Key is empty. Please enter a valid SDK Key to run ads properly.");
                    }
                    else
                    {
                        plist.root.SetString("AppLovinSdkKey", applovinSDKKey);
                    }
                }
#endif

                if (FrameworkGlobalConfig.Instance.iOSAdditionalFramework != null && FrameworkGlobalConfig.Instance.iOSAdditionalFramework.Length > 0)
                {
                    string projectPath = PBXProject.GetPBXProjectPath(path);
                    PBXProject project = new PBXProject();
                    project.ReadFromString(File.ReadAllText(projectPath));
                    string targetName = PBXProject.GetUnityTargetName(); // note, not "project." ...
                    string targetGUID = project.TargetGuidByName(targetName);

                    bool hasAdditionalFramework = false;
                    for (int x = 0; x < FrameworkGlobalConfig.Instance.iOSAdditionalFramework.Length; ++x)
                    {
                        if (!string.IsNullOrEmpty(FrameworkGlobalConfig.Instance.iOSAdditionalFramework[x]))
                        {
                            project.AddFrameworkToProject(targetGUID, FrameworkGlobalConfig.Instance.iOSAdditionalFramework[x], false);
                            hasAdditionalFramework = true;
                        }
                    }

                    if (hasAdditionalFramework)
                    {
                        File.WriteAllText(projectPath, project.WriteToString());
                    }
                }
            }

            if (plist.root.values.ContainsKey("NSAppTransportSecurity"))
            {
                plist.root.values.Remove("NSAppTransportSecurity");
            }
            plist.root.CreateDict("NSAppTransportSecurity").values.Add("NSAllowsArbitraryLoads", new PlistElementBoolean(true));

#if USE_APPSFLYER_ANALYTICS && APPSFLYER_UNINSTALL_EVENT
            var buildKey = "UIBackgroundModes";
            plist.root.CreateArray(buildKey).AddString("remote-notification");
#endif

            File.WriteAllText(plistPath, plist.WriteToString());
        }
#endif
    }
}