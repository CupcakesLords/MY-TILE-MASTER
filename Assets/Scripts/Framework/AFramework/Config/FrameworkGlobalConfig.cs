using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AFramework
{
    public class FrameworkGlobalConfig : ScriptableObject
    {
        [SerializeField] AFramework.IAP.IAPPackages _IAPConfig;
        [SerializeField] AFramework.IAP.IAPPackages _IAPConfigVIP;
        public AFramework.IAP.IAPPackages IAPConfig
        {
            get
            {
#if PREMIUM
                return _IAPConfigVIP;
#else
                return _IAPConfig;
#endif
            }
        }

        [SerializeField] AFramework.Ads.BaseAdapterConfig _AdsConfig;
        [SerializeField] AFramework.Ads.BaseAdapterConfig _AdsConfigVIP;
        public AFramework.Ads.BaseAdapterConfig AdsConfig
        {
            get
            {
#if PREMIUM
                return _AdsConfigVIP;
#else
                return _AdsConfig;
#endif
            }
        }

        [SerializeField] string[] _iOSAdditionalFramework;
        public string[] iOSAdditionalFramework { get { return _iOSAdditionalFramework; } }

#if UNITY_EDITOR
        private const string SettingsFile = "Assets/FrameworkGlobalConfig.asset";
        private static FrameworkGlobalConfig instance;
        public static FrameworkGlobalConfig Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = (FrameworkGlobalConfig)UnityEditor.AssetDatabase.LoadAssetAtPath(
                        SettingsFile, typeof(FrameworkGlobalConfig));

                    if (instance == null)
                    {
                        instance = ScriptableObject.CreateInstance<FrameworkGlobalConfig>();
                        UnityEditor.AssetDatabase.CreateAsset(instance, SettingsFile);
                    }
                }
                return instance;
            }
        }
#endif
    }
}