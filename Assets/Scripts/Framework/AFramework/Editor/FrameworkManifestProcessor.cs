using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace AFramework
{
    public class FrameworkManifestProcessor : IPreprocessBuildWithReport
    {
        private const string META_APPLICATION_ID = "com.google.android.gms.ads.APPLICATION_ID";
        private const string META_APPLOVIN_KEY = "applovin.sdk.key";
        private XNamespace ns = "http://schemas.android.com/apk/res/android";

        public int callbackOrder { get { return 0; } }

        public void OnPreprocessBuild(BuildReport report)
        {
            string manifestPath = Path.Combine(
                    Application.dataPath, "Plugins/Android/AndroidManifest.xml");
            XDocument manifest = null;
            try
            {
                manifest = XDocument.Load(manifestPath);
            }
#pragma warning disable 0168
            catch (IOException e)
#pragma warning restore 0168
            {
                Debug.LogError("AndroidManifest.xml is missing. Try re-importing the plugin.");
            }

            XElement elemManifest = manifest.Element("manifest");
            if (elemManifest == null)
            {
                Debug.LogError("AndroidManifest.xml is not valid. Try re-importing the plugin.");
            }

            XElement elemApplication = elemManifest.Element("application");
            if (elemApplication == null)
            {
                Debug.LogError("AndroidManifest.xml is not valid. Try re-importing the plugin.");
            }

            IEnumerable<XElement> metas = elemApplication.Descendants()
                    .Where(elem => elem.Name.LocalName.Equals("meta-data"));

            if (FrameworkGlobalConfig.Instance != null && FrameworkGlobalConfig.Instance.AdsConfig != null)
            {
                XElement elemAdMobEnabled = GetMetaElement(metas, META_APPLICATION_ID);
                if (!string.IsNullOrEmpty(FrameworkGlobalConfig.Instance.AdsConfig.Platform.AppId))
                {
                    string appId = FrameworkGlobalConfig.Instance.AdsConfig.Platform.AppId;

                    if (appId.Length == 0)
                    {
                        Debug.LogError(
                            "Android AdMob app ID is empty. Please enter a valid app ID to run ads properly.");
                    }

                    if (elemAdMobEnabled == null)
                    {
                        elemApplication.Add(CreateMetaElement(META_APPLICATION_ID, appId));
                    }
                    else
                    {
                        elemAdMobEnabled.SetAttributeValue(ns + "value", appId);
                    }
                }
                else
                {
                    if (elemAdMobEnabled != null)
                    {
                        elemAdMobEnabled.Remove();
                    }
                }

#if USE_ADMOB
                XElement elemApplovinEnabled = GetMetaElement(metas, META_APPLOVIN_KEY);
                if (!string.IsNullOrEmpty(FrameworkGlobalConfig.Instance.AdsConfig.Platform.ApplovingSDKKey))
                {
                    string applovinSDKKey = FrameworkGlobalConfig.Instance.AdsConfig.Platform.ApplovingSDKKey;

                    if (applovinSDKKey.Length == 0)
                    {
                        Debug.LogError(
                            "Android Applovin SDK Key is empty. Please enter a valid SDK Key to run ads properly.");
                    }

                    if (elemApplovinEnabled == null)
                    {
                        elemApplication.Add(CreateMetaElement(META_APPLOVIN_KEY, applovinSDKKey));
                    }
                    else
                    {
                        elemApplovinEnabled.SetAttributeValue(ns + "value", applovinSDKKey);
                    }
                }
                else
                {
                    if (elemApplovinEnabled != null)
                    {
                        elemApplovinEnabled.Remove();
                    }
                }
#endif
            }

            elemManifest.Save(manifestPath);
        }

        private XElement CreateMetaElement(string name, object value)
        {
            return new XElement("meta-data",
                    new XAttribute(ns + "name", name), new XAttribute(ns + "value", value));
        }

        private XElement GetMetaElement(IEnumerable<XElement> metas, string metaName)
        {
            foreach (XElement elem in metas)
            {
                IEnumerable<XAttribute> attrs = elem.Attributes();
                foreach (XAttribute attr in attrs)
                {
                    if (attr.Name.Namespace.Equals(ns)
                            && attr.Name.LocalName.Equals("name") && attr.Value.Equals(metaName))
                    {
                        return elem;
                    }
                }
            }
            return null;
        }
    }
}