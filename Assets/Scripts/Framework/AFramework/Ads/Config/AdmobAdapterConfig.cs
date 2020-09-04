#if false//USE_ADMOB
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;

namespace AFramework.Ads
{
    [CreateAssetMenu(menuName = "ScriptableObject/AFramework/Ads/AdmobAdapterConfig")]
    [System.Serializable]
    public class AdmobAdapterConfig : IBaseAdapterConfig
    {
        public float AdsFirstDelayTime = 10f;
        public float AdsRequestInterval = 30f;

        [System.Serializable]
        public class PlatformConfig
        {
            public string AppId;
            public string BannerlId;
            public string InterstitialId;
            public string RewardedVideoId;
            public AdSize BannerSize = AdSize.Banner;
        }

        [SerializeField]
        PlatformConfig Android;
        [SerializeField]
        PlatformConfig iOS;
        public PlatformConfig Platform
        {
            get
            {
#if UNITY_IOS
            return iOS;
#elif UNITY_ANDROID
            return Android;
#else
            return null;
#endif
            }
        }

        public float GetAdsFirstDelayTime() { return AdsFirstDelayTime; }
        public float GetRequestInterval() { return AdsRequestInterval; }
    }
}
#endif