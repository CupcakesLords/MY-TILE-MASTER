#if USE_ADMOB
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;

namespace AFramework.Ads
{
    public class AdmobAdapter : BaseAdsAdapter
    {
        protected BaseAdapterConfig Config { get; private set; }

        public override void Init(object[] parameters)
        {
            mConfig = ((BaseAdapterConfig)parameters[0]);
            Config = (BaseAdapterConfig)mConfig;
            MobileAds.Initialize(AdModInitCallback);
            //base.Init(parameters);
        }

        void AdModInitCallback(InitializationStatus result)
        {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
            ReflectionHelper._InvokeNamespaceClassStaticMethod("GoogleMobileAds.Api.Mediation.AppLovin", "Initialize", false);
#endif
#if !UNITY_EDITOR
            if (AdsManager.Debugging)
            {
                foreach (var pair in result.getAdapterStatusMap())
                {
                    Debug.Log(pair.Key + " " + pair.Value.InitializationState);
                }
            }
#endif
            base.Init(null);
        }

        protected override AdStatusHandler CreateAdHandler(AdsType type, string id)
        {
            return new AdmodAdStatusHandler(type, id);
        }

        protected override void DownloadAd(AdStatusHandler ad)
        {
            if (AdsManager.Debugging) Debug.Log("DownloadAd " + ad._Type.ToString() + " id " + ad._Id);
            base.DownloadAd(ad);
            switch (ad._Type)
            {
                case AdsType.Banner:
                    {
                        var newBannerAd = new BannerView(ad._Id, AdSize.Banner, AdapterBannerPosition(mBannerPosition));
                        var currentAdhandler = mCurrentDownload as AdmodAdStatusHandler;
                        currentAdhandler.AssignData(newBannerAd);

                        newBannerAd.OnAdOpening += (sender, args) =>
                        {
                            UnityMainThreadDispatcher.instance.Enqueue(() => { BannerAdOpenedEvent(currentAdhandler); });
                        };
                        newBannerAd.OnAdLoaded += (sender, args) =>
                        {
                            var backupSender = sender;
                            UnityMainThreadDispatcher.instance.Enqueue(() => {
                                BannerAdLoadedEvent(currentAdhandler, sender, null, true);
                            });
                        };
                        newBannerAd.OnAdFailedToLoad += (sender, args) =>
                        {
                            var backupSender = sender;
                            var backupMsg = args.Message;
                            UnityMainThreadDispatcher.instance.Enqueue(() => {
                                BannerAdLoadedEvent(currentAdhandler, sender, backupMsg, false);
                            });
                        };

                        // Create an empty ad request.
                        AdRequest request = new AdRequest.Builder().Build();
                        // Load the banner with the request.
                        newBannerAd.LoadAd(request);
                    }
                    break;
                case AdsType.Interstitial:
                    {
                        var newInterstitialAd = new InterstitialAd(ad._Id);
                        var currentAdhandler = mCurrentDownload as AdmodAdStatusHandler;
                        currentAdhandler.AssignData(newInterstitialAd);


                        newInterstitialAd.OnAdOpening += (sender, args) =>
                        {
                            UnityMainThreadDispatcher.instance.Enqueue(() => { InterstitialAdOpenedEvent(currentAdhandler); });
                        };
                        newInterstitialAd.OnAdLoaded += (sender, args) =>
                        {
                            var backupSender = sender;
                            UnityMainThreadDispatcher.instance.Enqueue(() => {
                                InterstitialAdLoadedEvent(currentAdhandler, sender, null, true);
                            });
                        };
                        newInterstitialAd.OnAdFailedToLoad += (sender, args) =>
                        {
                            var backupSender = sender;
                            var backupMsg = args.Message;
                            UnityMainThreadDispatcher.instance.Enqueue(() => {
                                InterstitialAdLoadedEvent(currentAdhandler, sender, backupMsg, false);
                            });
                        };
                        newInterstitialAd.OnAdClosed += (sender, args) =>
                        {
                            UnityMainThreadDispatcher.instance.Enqueue(() => { InterstitialAdClosedEvent(currentAdhandler); });
                        };

                        // Create an empty ad request.
                        AdRequest request = new AdRequest.Builder().Build();
                        // Load the interstitial with the request.
                        newInterstitialAd.LoadAd(request);
                    }
                    break;
                case AdsType.RewardedVideo:
                    {
                        var newRewardedAd = new RewardedAd(ad._Id);
                        var currentAdhandler = mCurrentDownload as AdmodAdStatusHandler;
                        currentAdhandler.AssignData(newRewardedAd);
                        newRewardedAd.OnAdOpening += (sender, args) =>
                        {
                            UnityMainThreadDispatcher.instance.Enqueue(() => { RewardedVideoAdOpenedEvent(currentAdhandler); });
                        };
                        newRewardedAd.OnAdLoaded += (sender, args) =>
                        {
                            var backupSender = sender;
                            UnityMainThreadDispatcher.instance.Enqueue(() => {
                                RewardedVideoAdLoadedEvent(currentAdhandler, sender, null, true);
                            });
                        };
                        newRewardedAd.OnAdFailedToLoad += (sender, args) =>
                        {
                            var backupSender = sender;
                            var backupMsg = args.Message;
                            UnityMainThreadDispatcher.instance.Enqueue(() => {
                                RewardedVideoAdLoadedEvent(currentAdhandler, sender, backupMsg, false);
                            });
                        };
                        newRewardedAd.OnAdClosed += (sender, args) =>
                        {
                            UnityMainThreadDispatcher.instance.Enqueue(() => { RewardedVideoAdClosedEvent(currentAdhandler); });
                        };
                        newRewardedAd.OnUserEarnedReward += (sender, args) =>
                        {
                            UnityMainThreadDispatcher.instance.Enqueue(() => { RewardedVideoAdRewardedEvent(currentAdhandler); });
                        };

                        AdRequest request = new AdRequest.Builder().Build();
                        // Load the rewarded ad with the request.
                        newRewardedAd.LoadAd(request);
                    }
                    break;
            }
        }

        public override void SetBannerPosition(BannerPosition position)
        {
            base.SetBannerPosition(position);
            if (mDefaultBannerAdList.Count <= 0 || !mDefaultBannerAdList[0]._Available) return;
            (mDefaultBannerAdList[0] as AdmodAdStatusHandler).bannerData.SetPosition(AdapterBannerPosition(mBannerPosition));
        }

        public override void HideAdsBanner()
        {
            if (AdsManager.EventOnBannerAdsChanged != null) AdsManager.EventOnBannerAdsChanged(false);
            base.HideAdsBanner();
            if (mDefaultBannerAdList.Count <= 0 || !mDefaultBannerAdList[0]._Available) return;
            (mDefaultBannerAdList[0] as AdmodAdStatusHandler).bannerData.Hide();
        }

        public override void ShowAdsBanner()
        {
            base.ShowAdsBanner();
            if (mDefaultBannerAdList.Count <= 0 || !mDefaultBannerAdList[0]._Available) return;
            (mDefaultBannerAdList[0] as AdmodAdStatusHandler).bannerData.Show();
            if (AdsManager.EventOnBannerAdsChanged != null)
            {
                AdsManager.EventOnBannerAdsChanged(true);
            }
        }

        public override bool ShowAdsInterstitial(System.Action<bool> callback, string adId = null)
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
            if (!IsInterstitialAdAvailable(adId)) return false;
            base.ShowAdsInterstitial(callback, adId);
            mCurrentFullscreenAd = mAdDownloadHandler[adId];

            //if (mAdsInterstitialTimeoutThread != null)
            //{
            //    StopCoroutine(mAdsInterstitialTimeoutThread);
            //    mAdsInterstitialTimeoutThread = null;
            //}
            //mAdsInterstitialTimeoutThread = CRAdsInterstitialTimeoutThread();
            //StartCoroutine(mAdsInterstitialTimeoutThread);

            (mCurrentFullscreenAd as AdmodAdStatusHandler).interstitialData.Show();
            return true;
        }

        public override bool ShowAdsReward(System.Action<bool> callback, string adId = null)
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
            if (!IsRewardAdAvailable(adId)) return false;
            base.ShowAdsReward(callback);
            mCurrentFullscreenAd = mAdDownloadHandler[adId];

            //if (mAdsRewardTimeoutThread != null)
            //{
            //    StopCoroutine(mAdsRewardTimeoutThread);
            //    mAdsRewardTimeoutThread = null;
            //}
            //mAdsRewardTimeoutThread = CRAdsRewardTimeoutThread();
            //StartCoroutine(mAdsRewardTimeoutThread);

            (mCurrentFullscreenAd as AdmodAdStatusHandler).rewardedVideoData.Show();
            return true;
        }

        AdPosition AdapterBannerPosition(BannerPosition position)
        {
            switch (position)
            {
                case BannerPosition.Top:
                    return AdPosition.Top;
                case BannerPosition.Bottom:
                    return AdPosition.Bottom;
                case BannerPosition.TopLeft:
                    return AdPosition.TopLeft;
                case BannerPosition.TopRight:
                    return AdPosition.TopRight;
                case BannerPosition.BottomLeft:
                    return AdPosition.BottomRight;
                case BannerPosition.Center:
                    return AdPosition.Center;
            }
            return AdPosition.Bottom;
        }

        //protected override void AdsRequestCallback(AdsType type, bool result)
        //{
        //    switch (type)
        //    {
        //        case AdsType.Banner:
        //            break;
        //        case AdsType.Interstitial:
        //            break;
        //        case AdsType.RewardedVideo:
        //            break;
        //    }
        //}

#region BannerAds

        void BannerAdLoadedEvent(AdmodAdStatusHandler adHandler, object sender, string message, bool success)
        {
            if (AdsManager.Debugging && mCurrentDownload != adHandler) Debug.LogWarning("BannerAds - Return download handler is not same as current download status " + success + " current " + mCurrentDownload + "  other " + adHandler);

            if (mCurrentDownload == adHandler)
            {
                AdDownloadCallback(AdsType.Banner, success, message);
            }
            else
            {
                adHandler.OnAdAvailabilityUpdate(success);
            }

            //if ((object)InternalEventOnBannerAdsChanged != null)
            //{
            //    InternalEventOnBannerAdsChanged();
            //}

            if (mBannerAdVisibility)
            {
                ShowAdsBanner();
            }
            else
            {
                if (AdsManager.EventOnBannerAdsChanged != null) AdsManager.EventOnBannerAdsChanged(false);
                HideAdsBanner();
            }
            if (AdsManager.Debugging) Debug.Log(success ? "BannerAds - Request Success" : ("BannerAds - Request Fail - " + message));
        }

        void BannerAdOpenedEvent(AdmodAdStatusHandler adHandler)
        {
            if (!mBannerAdVisibility)
            {
                HideAdsBanner();
                return;
            }
            if (AdsManager.Debugging) Debug.Log("BannerAds - Ads Opened");
        }

#endregion

#region InterstitialAds

        void InterstitialAdLoadedEvent(AdmodAdStatusHandler adHandler, object sender, string message, bool success)
        {
            if (AdsManager.Debugging && mCurrentDownload != adHandler) Debug.LogWarning("InterstitialAds - Return download handler is not same as current download");

            if (mCurrentDownload == adHandler)
            {
                AdDownloadCallback(AdsType.Interstitial, success, message);
            }
            else
            {
                adHandler.OnAdAvailabilityUpdate(success);
            }

            //TODO
            //if ((object)InternalEventOnInterstitialAdsChanged != null)
            //{
            //    InternalEventOnInterstitialAdsChanged();
            //}
            if (AdsManager.Debugging) Debug.Log(success ? "InterstitialAds - Request Success" : ("InterstitialAds - Request Fail - " + message));
        }

        void InterstitialAdOpenedEvent(AdmodAdStatusHandler adHandler)
        {
            AudioListener.volume = 0;
            FullscreenAdShowing = true;

            if (AdsManager.Debugging) Debug.Log("InterstitialAds - Ads Opened");
        }

        void InterstitialAdClosedEvent(AdmodAdStatusHandler adHandler)
        {
            AudioListener.volume = 1;
            if (mInterstitialAdCallback != null)
            {
                mInterstitialAdCallback(true);
                mInterstitialAdCallback = null;
            }
            if (mCurrentFullscreenAd != null)
            {
                mCurrentFullscreenAd.OnAdAvailabilityUpdate(false);
                if (AdsManager.Debugging && mCurrentFullscreenAd != adHandler) Debug.LogWarning("RewardAds - Current Fullscreen Ad is not same as param adHandler");
                mCurrentFullscreenAd = null;
            }
            FullscreenAdShowing = false;
            if (AdsManager.Debugging) Debug.Log("InterstitialAds - Ads Closed");
        }

#endregion

#region RewardAds
        bool mHaveRewarded = false;
        IEnumerator mRewardedVideoDelayEventChanged = null;
        IEnumerator mDelayRewardCheckThread;

        void RewardedVideoAdLoadedEvent(AdmodAdStatusHandler adHandler, object sender, string message, bool success)
        {
            if (AdsManager.Debugging && mCurrentDownload != adHandler) Debug.LogWarning("RewardAds - Return download handler is not same as current download");

            if (mCurrentDownload == adHandler)
            {
                AdDownloadCallback(AdsType.RewardedVideo, success, message);
            }
            else
            {
                adHandler.OnAdAvailabilityUpdate(success);
            }

            if (BaseAdsAdapter.AdsAvailableSafeTime <= 0)
            {
                if ((object)InternalEventOnRewardAdsChanged != null)
                {
                    InternalEventOnRewardAdsChanged();
                }
            }
            else if (success)
            {
                if (mRewardedVideoDelayEventChanged != null)
                {
                    StopCoroutine(mRewardedVideoDelayEventChanged);
                    mRewardedVideoDelayEventChanged = null;
                }

                mRewardedVideoDelayEventChanged = CRDelayInternalEventOnRewardAdsChanged();
                StartCoroutine(mRewardedVideoDelayEventChanged);
            }
            else
            {
                if (mRewardedVideoDelayEventChanged != null)
                {
                    StopCoroutine(mRewardedVideoDelayEventChanged);
                    mRewardedVideoDelayEventChanged = null;
                }

                if ((object)InternalEventOnRewardAdsChanged != null)
                {
                    InternalEventOnRewardAdsChanged();
                }
            }

            if (AdsManager.Debugging) Debug.Log(success ? "RewardAds - Request Success" : ("RewardAds - Request Fail - " + message));
        }

        void RewardedVideoAdOpenedEvent(AdmodAdStatusHandler adHandler)
        {
            AudioListener.volume = 0;
            FullscreenAdShowing = true;
            mHaveRewarded = false;

            if (AdsManager.Debugging) Debug.Log("RewardAds - Ads Opened");
        }

        void RewardedVideoAdClosedEvent(AdmodAdStatusHandler adHandler)
        {
            AudioListener.volume = 1;

            if (mCurrentFullscreenAd != null)
            {
                if (AdsManager.Debugging && mCurrentFullscreenAd != adHandler) Debug.LogError("RewardAds - Current Fullscreen Ad is not same as param adHandler");
                mCurrentFullscreenAd.OnAdAvailabilityUpdate(false);
                mCurrentFullscreenAd = null;
            }

            if (mHaveRewarded)
            {
                if (mRewardAdCallback != null)
                {
                    mRewardAdCallback(mHaveRewarded);
                    mRewardAdCallback = null;
                }
                FullscreenAdShowing = false;
            }
            else
            {
                if (mDelayRewardCheckThread != null)
                {
                    StopCoroutine(mDelayRewardCheckThread);
                    mDelayRewardCheckThread = null;
                }

                mDelayRewardCheckThread = CRWaitForReward();
                StartCoroutine(mDelayRewardCheckThread);
            }

            if (AdsManager.Debugging) Debug.Log("RewardAds - Ads Closed");
        }

        void RewardedVideoAdRewardedEvent(AdmodAdStatusHandler adHandler)
        {
            mHaveRewarded = true;
            if (AdsManager.Debugging) Debug.Log("RewardAds - Handle Reward");
        }

        IEnumerator CRDelayInternalEventOnRewardAdsChanged()
        {
            WaitForSeconds waitTime = new WaitForSeconds(BaseAdsAdapter.AdsAvailableSafeTime);
            yield return waitTime;
            yield return null;
            mRewardedVideoDelayEventChanged = null;

            if ((object)InternalEventOnRewardAdsChanged != null)
            {
                InternalEventOnRewardAdsChanged();
            }
        }

        IEnumerator CRWaitForReward()
        {
            float waitTime = 1.0f;
            var waitHandler = new WaitForSecondsRealtime(0.03f);
            while (waitTime > 0)
            {
                yield return waitHandler;
                waitTime -= 0.03f;

                if (mHaveRewarded)
                {
                    if (mRewardAdCallback != null)
                    {
                        mRewardAdCallback(true);
                        mRewardAdCallback = null;
                    }
                    mHaveRewarded = false;
                    FullscreenAdShowing = false;
                    mDelayRewardCheckThread = null;
                    yield break;
                }
            }
            if (mRewardAdCallback != null)
            {
                mRewardAdCallback(mHaveRewarded);
                mRewardAdCallback = null;
            }
            mHaveRewarded = false;
            FullscreenAdShowing = false;
            mDelayRewardCheckThread = null;
        }
#endregion
    }

    public class AdmodAdStatusHandler : AdStatusHandler
    {
        public BannerView bannerData { get; protected set; }
        public InterstitialAd interstitialData { get; protected set; }
        public RewardedAd rewardedVideoData { get; protected set; }

        public AdmodAdStatusHandler(AdsType type, string id) : base(type, id) { }

        protected override bool BannerAdAvailable()
        {
            return bannerData != null && _Available;
        }

        protected override bool InterstitialAdAvailable()
        {
            return interstitialData != null && interstitialData.IsLoaded();
        }

        protected override bool RewardAdAvailable()
        {
            return rewardedVideoData != null && rewardedVideoData.IsLoaded();
        }

        protected override bool OfferWallAvailable()
        {
            return false;//not supported
        }

        public override void OnAdAvailabilityUpdate(bool result)
        {
            base.OnAdAvailabilityUpdate(result);
            if (result)
            {
                //mLastCheckTime = -1;
            }
            else
            {
                CleanData();
            }
        }

        public void AssignData(object ptr)
        {
            switch (_Type)
            {
                case AdsType.Banner:
                    bannerData = ptr as BannerView;
                    break;
                case AdsType.Interstitial:
                    interstitialData = ptr as InterstitialAd;
                    break;
                case AdsType.RewardedVideo:
                    rewardedVideoData = ptr as RewardedAd;
                    break;
            }
        }

        void CleanData()
        {
            if (bannerData != null)
            {
                bannerData.Destroy();
                bannerData = null;
            }
            else if (interstitialData != null)
            {
                interstitialData.Destroy();
                interstitialData = null;
            }
            else if (rewardedVideoData != null)
            {
                rewardedVideoData = null;
            }
        }
    }
}
#endif