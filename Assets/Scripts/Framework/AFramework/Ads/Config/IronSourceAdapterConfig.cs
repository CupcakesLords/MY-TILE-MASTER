#if false//USE_IRONSOURCE_ADS
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AFramework.Ads
{
    [CreateAssetMenu(menuName = "ScriptableObject/AFramework/Ads/IronSourceAdapterConfig")]
    public class IronSourceAdapterConfig : ScriptableObject, IBaseAdapterConfig
    {
        public float AdsFirstDelayTime = 10f;
        public float AdsRequestInterval = 30f;

        [System.Serializable]
        public class PlatformConfig
        {
            public string AppId;
        }

	    //[SerializeField]
	    public PlatformConfig Android;
	    //[SerializeField]
	    public PlatformConfig iOS;
        public PlatformConfig Platform
        {
            get
            {
#if UNITY_IOS
            return iOS;
#elif UNITY_ANDROID
            return Android;
#endif
            }
        }

        public float GetAdsFirstDelayTime() { return AdsFirstDelayTime; }
        public float GetRequestInterval() { return AdsRequestInterval; }
    }
}
#endif