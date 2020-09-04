#if USE_APPLOVIN_ADS
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AFramework.Ads
{
    public class AppLovinAdapter : BaseAdsAdapter
    {
        protected BaseAdapterConfig Config { get; private set; }

        float mRVLoadTimer = 0f;
        int mInterstitialAdRequestIndex = 0;
        int mRewardAdRequestIndex = 0;

        public override void Init(object[] parameters)
        {
            mConfig = ((BaseAdapterConfig)parameters[0]);
            Config = mConfig;// (AppLovinAdapterConfig)mConfig;

            MaxSdkCallbacks.OnBannerAdLoadedEvent += HandleOnBannerAdLoadedEvent;
            MaxSdkCallbacks.OnBannerAdLoadFailedEvent += HandleOnBannerAdLoadFailedEvent;
            MaxSdkCallbacks.OnBannerAdClickedEvent += HandleOnBannerAdClickedEvent;

            MaxSdkCallbacks.OnInterstitialLoadedEvent += HandleOnInterstitialLoadedEvent;
            MaxSdkCallbacks.OnInterstitialLoadFailedEvent += HandleOnInterstitialLoadFailedEvent;
            MaxSdkCallbacks.OnInterstitialAdFailedToDisplayEvent += HandleOnInterstitialAdFailedToDisplayEvent;
            MaxSdkCallbacks.OnInterstitialHiddenEvent += HandleOnInterstitialHiddenEvent;
            MaxSdkCallbacks.OnInterstitialDisplayedEvent += HandleOnInterstitialDisplayedEvent;
            MaxSdkCallbacks.OnInterstitialClickedEvent += HandleOnInterstitialClickedEvent;

            MaxSdkCallbacks.OnRewardedAdLoadedEvent += HandleOnRewardedAdLoadedEvent;
            MaxSdkCallbacks.OnRewardedAdLoadFailedEvent += HandleOnRewardedAdLoadFailedEvent;
            MaxSdkCallbacks.OnRewardedAdFailedToDisplayEvent += HandleOnRewardedAdFailedToDisplayEvent;
            MaxSdkCallbacks.OnRewardedAdDisplayedEvent += HandleOnRewardedAdDisplayedEvent;
            MaxSdkCallbacks.OnRewardedAdClickedEvent += HandleOnRewardedAdClickedEvent;
            MaxSdkCallbacks.OnRewardedAdHiddenEvent += HandleOnRewardedAdHiddenEvent;
            MaxSdkCallbacks.OnRewardedAdReceivedRewardEvent += HandleOnRewardedAdReceivedRewardEvent;

            var cacheParameters = parameters;
            MaxSdkCallbacks.OnSdkInitializedEvent += (MaxSdkBase.SdkConfiguration sdkConfiguration) => {
                //MaxSdk.ShowMediationDebugger();
                MaxSdk.SetMuted(mIsMuted);
                base.Init(cacheParameters);
            };

            MaxSdk.SetSdkKey(Config.Platform.ApplovingSDKKey);
            MaxSdk.InitializeSdk();
        }

        bool mIsMuted = false;
        public void SetMute(bool mute)
        {
            mIsMuted = mute;
            MaxSdk.SetMuted(mIsMuted);
        }

        protected override AdStatusHandler CreateAdHandler(AdsType type, string id)
        {
            return new MaxAdStatusHandler(type, id);
        }

        protected override void DownloadAd(AdStatusHandler ad)
        {
            if (AdsManager.Debugging) Debug.Log("DownloadAd " + ad._Type.ToString() + " id " + ad._Id);
            base.DownloadAd(ad);
            switch (ad._Type)
            {
                case AdsType.Banner:
                    MaxSdk.CreateBanner(ad._Id, AdapterBannerPosition(mBannerPosition));
                    break;
                case AdsType.Interstitial:
                    MaxSdk.LoadInterstitial(ad._Id);
                    break;
                case AdsType.RewardedVideo:
                    MaxSdk.LoadRewardedAd(ad._Id);
                    break;
            }
        }

        #region Banner
        MaxSdkBase.BannerPosition AdapterBannerPosition(BannerPosition position)
        {
            switch (position)
            {
                case BannerPosition.Top:
                    return MaxSdkBase.BannerPosition.TopCenter;
                case BannerPosition.Bottom:
                    return MaxSdkBase.BannerPosition.BottomCenter;
                case BannerPosition.TopLeft:
                    return MaxSdkBase.BannerPosition.TopLeft;
                case BannerPosition.TopRight:
                    return MaxSdkBase.BannerPosition.TopRight;
                case BannerPosition.BottomLeft:
                    return MaxSdkBase.BannerPosition.BottomLeft;
                case BannerPosition.BottomRight:
                    return MaxSdkBase.BannerPosition.BottomRight;
            }
            return MaxSdkBase.BannerPosition.BottomCenter;
        }

        public override void SetBannerPosition(BannerPosition position)
        {
            //if (mBannerAdVisibility && mBannerPosition != position)
            //{
            //    MaxSdk.HideBanner(mBannerAdUnitId);
            //    MaxSdk.DestroyBanner(mBannerAdUnitId);
            //    MaxSdk.CreateBanner(mBannerAdUnitId, AdapterBannerPosition(position));
            //    mBannerAdLoadState = AdsLoadState.Downloading;
            //}
            base.SetBannerPosition(position);
        }

        public override void ShowAdsBanner()
        {
            if (mDefaultBannerAdList.Count <= 0 || !mDefaultBannerAdList[0]._Available) return;
            base.ShowAdsBanner();
            MaxSdk.ShowBanner(mDefaultBannerAdList[0]._Id);
        }

        public override void HideAdsBanner()
        {
            if (mDefaultBannerAdList.Count <= 0 || !mDefaultBannerAdList[0]._Available) return;
            base.HideAdsBanner();
            MaxSdk.HideBanner(mDefaultBannerAdList[0]._Id);
        }

        private void HandleOnBannerAdLoadedEvent(string obj)
        {
            if (AdsManager.Debugging) Debug.Log("HandleOnBannerAdLoadedEvent");
            AdDownloadCallback(true, null);
            if (mBannerAdVisibility)
            {
                ShowAdsBanner();
            }
            else
            {
                HideAdsBanner();
            }
        }

        private void HandleOnBannerAdClickedEvent(string obj)
        {
            if (AdsManager.Debugging) Debug.Log("HandleOnBannerAdClickedEvent");
            AFramework.Analytics.TrackingManager.I.TrackAdsClick(AdsType.Banner);
        }

        private void HandleOnBannerAdLoadFailedEvent(string arg1, int arg2)
        {
            if (AdsManager.Debugging) Debug.Log("HandleOnBannerAdLoadFailedEvent");
            AdDownloadCallback(false, arg2 + " : " + arg1);
        }
        #endregion

        #region InterstitialAd
        public override bool ShowAdsInterstitial(Action<bool> callback, string adId = null)
        {
            if (string.IsNullOrEmpty(adId))
            {
                for (int i = 0; i < mDefaultInterstitialAdList.Count; ++i)
                {
                    if (mDefaultInterstitialAdList[i]._Available)
                    {
                        adId = mDefaultInterstitialAdList[i]._Id;
                        break;
                    }
                }
            }
            if (string.IsNullOrEmpty(adId)) return false;
            if (!MaxSdk.IsInterstitialReady(adId)) return false;
            base.ShowAdsInterstitial(callback);
            mCurrentFullscreenAd = mAdDownloadHandler[adId];
            MaxSdk.ShowInterstitial(adId);
            return true;
        }

        public override bool IsInterstitialAdAvailable(string adId = null)
        {
            if (!string.IsNullOrEmpty(adId))
            {
                return mAdDownloadHandler[adId]._Available;
            }
            for (int i = 0; i < mDefaultInterstitialAdList.Count; ++i)
            {
                if (mDefaultInterstitialAdList[i]._Available) return true;
            }
            return false;
        }

        private void HandleOnInterstitialHiddenEvent(string obj)
        {
            if (AdsManager.Debugging) Debug.Log("HandleOnInterstitialHiddenEvent");
            AudioListener.volume = 1;
            if (mInterstitialAdCallback != null)
            {
                mInterstitialAdCallback(true);
                mInterstitialAdCallback = null;
            }
            if (mCurrentFullscreenAd != null)
            {
                mCurrentFullscreenAd.OnAdAvailabilityUpdate(false);
                mCurrentFullscreenAd = null;
            }
            FullscreenAdShowing = false;
        }

        private void HandleOnInterstitialDisplayedEvent(string arg)
        {
            if (AdsManager.Debugging) Debug.Log("HandleOnInterstitialDisplayedEvent");
            AudioListener.volume = 0;
            FullscreenAdShowing = true;
        }

        private void HandleOnInterstitialClickedEvent(string arg)
        {
            if (AdsManager.Debugging) Debug.Log("HandleOnInterstitialClickedEvent");
            AFramework.Analytics.TrackingManager.I.TrackAdsClick(AdsType.Interstitial);
        }

        private void HandleOnInterstitialAdFailedToDisplayEvent(string arg1, int arg2)
        {
            if (AdsManager.Debugging) Debug.Log("HandleOnInterstitialAdFailedToDisplayEvent");
            AudioListener.volume = 1;
            if (mInterstitialAdCallback != null)
            {
                mInterstitialAdCallback(false);
                mInterstitialAdCallback = null;
            }
            if (mCurrentFullscreenAd != null)
            {
                mCurrentFullscreenAd.OnAdAvailabilityUpdate(false);
                mCurrentFullscreenAd = null;
            }
            FullscreenAdShowing = false;
        }

        private void HandleOnInterstitialLoadFailedEvent(string arg1, int arg2)
        {
            if (AdsManager.Debugging) Debug.Log("HandleOnInterstitialLoadFailedEvent");
            AdDownloadCallback(false, arg2 + " : " + arg1);
        }

        private void HandleOnInterstitialLoadedEvent(string obj)
        {
            if (AdsManager.Debugging) Debug.Log("HandleOnInterstitialLoadedEvent");
            if (AdsManager.Debugging && mCurrentDownload != null && mCurrentDownload._Id != obj) Debug.LogError("HandleOnInterstitialLoadedEvent conflict id");
            AdDownloadCallback(true, null);
        }
        #endregion

        #region RewardAd
        bool mHaveRewarded = false;

        public override bool ShowAdsReward(Action<bool> callback, string adId = null)
        {
            if (string.IsNullOrEmpty(adId))
            {
                for (int i = 0; i < mDefaultRewardAdList.Count; ++i)
                {
                    if (mDefaultRewardAdList[i]._Available)
                    {
                        adId = mDefaultRewardAdList[i]._Id;
                        break;
                    }
                }
            }
            if (string.IsNullOrEmpty(adId)) return false;
            if (!MaxSdk.IsRewardedAdReady(adId)) return false;
            base.ShowAdsReward(callback);
            mCurrentFullscreenAd = mAdDownloadHandler[adId];
            MaxSdk.ShowRewardedAd(adId);
            return true;
        }

        public override bool IsRewardAdAvailable(string adId = null)
        {
            if (!string.IsNullOrEmpty(adId))
            {
                return mAdDownloadHandler[adId]._Available;
            }
            for (int i = 0; i < mDefaultRewardAdList.Count; ++i)
            {
                if (mDefaultRewardAdList[i]._Available) return true;
            }
            return false;
        }

        IEnumerator CRAdsRewardInsurance()
        {
            float insuranceTime = 35f;
            while (insuranceTime > 0 && FullscreenAdShowing)
            {
                insuranceTime -= Time.fixedUnscaledDeltaTime;
                if (insuranceTime <= 0)
                {
                    mHaveRewarded = true;
                }
                yield return null;
            }
        }

        private void HandleOnRewardedAdReceivedRewardEvent(string arg1, MaxSdkBase.Reward arg2)
        {
            if (AdsManager.Debugging) Debug.Log("HandleOnRewardedAdReceivedRewardEvent");
            mHaveRewarded = true;
        }

        private void HandleOnRewardedAdHiddenEvent(string obj)
        {
            if (AdsManager.Debugging) Debug.Log("HandleOnRewardedAdHiddenEvent");
            AudioListener.volume = 1;
            if (mRewardAdCallback != null)
            {
                mRewardAdCallback(mHaveRewarded);
                mRewardAdCallback = null;
            }

            if (mCurrentFullscreenAd != null)
            {
                mCurrentFullscreenAd.OnAdAvailabilityUpdate(false);
                mCurrentFullscreenAd = null;
            }

            mHaveRewarded = false;
            FullscreenAdShowing = false;
        }

        private void HandleOnRewardedAdClickedEvent(string obj)
        {
            if (AdsManager.Debugging) Debug.Log("HandleOnRewardedAdClickedEvent");
            AFramework.Analytics.TrackingManager.I.TrackAdsClick(AdsType.RewardedVideo);
        }

        private void HandleOnRewardedAdDisplayedEvent(string obj)
        {
            if (AdsManager.Debugging) Debug.Log("HandleOnRewardedAdDisplayedEvent");
            AudioListener.volume = 0;
            FullscreenAdShowing = true;
            mHaveRewarded = false;
            StartCoroutine(CRAdsRewardInsurance());
        }

        private void HandleOnRewardedAdFailedToDisplayEvent(string arg1, int arg2)
        {
            if (AdsManager.Debugging) Debug.Log("HandleOnRewardedAdFailedToDisplayEvent");
            AudioListener.volume = 1;
            if (mRewardAdCallback != null)
            {
                mRewardAdCallback(false);
                mRewardAdCallback = null;
            }
            if (mCurrentFullscreenAd != null)
            {
                mCurrentFullscreenAd.OnAdAvailabilityUpdate(false);
                mCurrentFullscreenAd = null;
            }
        }

        private void HandleOnRewardedAdLoadFailedEvent(string arg1, int arg2)
        {
            if (AdsManager.Debugging) Debug.Log("HandleOnRewardedAdLoadFailedEvent");
            AdDownloadCallback(false, arg2 + " : " + arg1);
        }

        private void HandleOnRewardedAdLoadedEvent(string obj)
        {
            if (AdsManager.Debugging) Debug.Log("HandleOnRewardedAdLoadedEvent");
            if (AdsManager.Debugging && mCurrentDownload != null && mCurrentDownload._Id != obj) Debug.LogError("HandleOnRewardedAdLoadedEvent conflict id");
            AdDownloadCallback(true, null);
        }
        #endregion
    }

    public class MaxAdStatusHandler : AdStatusHandler
    {
        public MaxAdStatusHandler(AdsType type, string id) : base(type, id) { }

        protected override bool BannerAdAvailable()
        {
            return _Available;
        }

        protected override bool InterstitialAdAvailable()
        {
            return MaxSdk.IsInterstitialReady(_Id);
        }

        protected override bool RewardAdAvailable()
        {
            return MaxSdk.IsRewardedAdReady(_Id);
        }
    }
}
#endif