using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AFramework.Ads
{
    [CreateAssetMenu(menuName = "ScriptableObject/AFramework/Ads/BaseAdapterConfig")]
    public class BaseAdapterConfig : ScriptableObject
    {
        [SerializeField]
        protected float AdsErrorRetryInterval = 3f;
        [SerializeField]
        protected float AdsDownloadTimeout = 30f;

        [System.Serializable]
        public class PlatformConfig
        {
            public string AppId;
            public string BannerlId;
            public string InterstitialId;
            public string RewardedVideoId;
            public string OfferWallId;
            public string IronsourceId;
            public string ApplovingSDKKey;

            [Header("Placement")]
            public string BannerAdPlacementConfig;
            public string InterstitialAdPlacementConfig;
            public string RewardAdPlacementConfig;
            public string OfferWallPlacementConfig;
        }

        [SerializeField]
        protected PlatformConfig Android;
        [SerializeField]
        protected PlatformConfig iOS;

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

        public float GetErrorRetryInterval() { return AdsErrorRetryInterval; }
        public float GetDownloadTimeout() { return AdsDownloadTimeout; }
    }

}