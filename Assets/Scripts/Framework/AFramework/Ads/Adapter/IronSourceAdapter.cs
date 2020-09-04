#if USE_IRONSOURCE_ADS
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AFramework.Ads
{
    public class IronSourceAdapter : BaseAdsAdapter
    {
        protected BaseAdapterConfig Config { get { return mConfig; } }
        bool mHaveRewarded = false;

        public override void Init(object[] parameters)
	    {
#if DEVELOPMENT_BUILD
            //IronSource.Agent.setAdaptersDebug(true);
#endif

            mConfig = ((BaseAdapterConfig)parameters[0]);
            List<string> adsTypeSupport = new List<string>();
            if (AdsManager.I.SupportAdsType[(int)AdsType.Banner])
            {
                adsTypeSupport.Add(IronSourceAdUnits.BANNER);

                //Banner Ads
                IronSourceEvents.onBannerAdLoadedEvent += () =>
                {
                    if (AdsManager.Debugging) Debug.Log("onBannerAdLoadedEvent");
                    UnityMainThreadDispatcher.instance.Enqueue(HandleonBannerAdLoadedEvent);
                };
                IronSourceEvents.onBannerAdLoadFailedEvent += (result) =>
                {
                    if (AdsManager.Debugging) Debug.Log("onBannerAdLoadFailedEvent " + result.ToString());
                    var backupResult = result;
                    UnityMainThreadDispatcher.instance.Enqueue(() => { HandleonBannerAdLoadFailedEvent(backupResult); });
                };
                IronSourceEvents.onBannerAdClickedEvent += () =>
                {
                    if (AdsManager.Debugging) Debug.Log("onBannerAdClickedEvent ");
                    UnityMainThreadDispatcher.instance.Enqueue(HandleonBannerAdClickedEvent);
                };
                IronSourceEvents.onBannerAdScreenPresentedEvent += () =>
                {
                    if (AdsManager.Debugging) Debug.Log("onBannerAdScreenPresentedEvent ");
                    UnityMainThreadDispatcher.instance.Enqueue(HandleonBannerAdScreenPresentedEvent);
                };
                IronSourceEvents.onBannerAdScreenDismissedEvent += () =>
                {
                    if (AdsManager.Debugging) Debug.Log("onBannerAdScreenDismissedEvent ");
                    UnityMainThreadDispatcher.instance.Enqueue(HandleonBannerAdScreenDismissedEvent);
                };
                IronSourceEvents.onBannerAdLeftApplicationEvent += () =>
                {
                    if (AdsManager.Debugging) Debug.Log("onBannerAdLeftApplicationEvent");
                    UnityMainThreadDispatcher.instance.Enqueue(HandleonBannerAdLeftApplicationEvent);
                };
                //
            }

            if (AdsManager.I.SupportAdsType[(int)AdsType.Interstitial])
            {
                adsTypeSupport.Add(IronSourceAdUnits.INTERSTITIAL);

                //Interstitial Ads
                IronSourceEvents.onInterstitialAdReadyEvent += () =>
                {
                    if (AdsManager.Debugging) Debug.Log("onInterstitialAdReadyEvent");
                    UnityMainThreadDispatcher.instance.Enqueue(HandleonInterstitialAdReadyEvent);
                };
                IronSourceEvents.onInterstitialAdLoadFailedEvent += (result) =>
                {
                    if (AdsManager.Debugging) Debug.Log("onInterstitialAdLoadFailedEvent " + result.ToString());
                    var backupResult = result;
                    UnityMainThreadDispatcher.instance.Enqueue(() => { HandleonInterstitialAdLoadFailedEvent(backupResult); });
                };
                IronSourceEvents.onInterstitialAdShowSucceededEvent += () =>
                {
                    if (AdsManager.Debugging) Debug.Log("onInterstitialAdShowSucceededEvent");
                    UnityMainThreadDispatcher.instance.Enqueue(HandleonInterstitialAdShowSucceededEvent);
                };
                IronSourceEvents.onInterstitialAdShowFailedEvent += (result) =>
                {
                    if (AdsManager.Debugging) Debug.Log("onInterstitialAdShowFailedEvent " + result.ToString());
                    var backupResult = result;
                    UnityMainThreadDispatcher.instance.Enqueue(() => { HandleonInterstitialAdShowFailedEvent(backupResult); });
                };
                IronSourceEvents.onInterstitialAdClickedEvent += () =>
                {
                    if (AdsManager.Debugging) Debug.Log("onInterstitialAdClickedEvent");
                    UnityMainThreadDispatcher.instance.Enqueue(HandleonInterstitialAdClickedEvent);
                };
                IronSourceEvents.onInterstitialAdOpenedEvent += () =>
                {
                    if (AdsManager.Debugging) Debug.Log("onInterstitialAdOpenedEvent");
                    UnityMainThreadDispatcher.instance.Enqueue(HandleonInterstitialAdOpenedEvent);
                };
                IronSourceEvents.onInterstitialAdClosedEvent += () =>
                {
                    if (AdsManager.Debugging) Debug.Log("onInterstitialAdClosedEvent");
                    UnityMainThreadDispatcher.instance.Enqueue(HandleonInterstitialAdClosedEvent);
                };
                //
            }

            if (AdsManager.I.SupportAdsType[(int)AdsType.RewardedVideo])
            {
                adsTypeSupport.Add(IronSourceAdUnits.REWARDED_VIDEO);

                //Reward Ads
                IronSourceEvents.onRewardedVideoAdOpenedEvent += () =>
                {
                    if (AdsManager.Debugging) Debug.Log("onRewardedVideoAdOpenedEvent");
                    UnityMainThreadDispatcher.instance.Enqueue(HandleonRewardedVideoAdOpenedEvent);
                };
                IronSourceEvents.onRewardedVideoAdClosedEvent += () =>
                {
                    if (AdsManager.Debugging) Debug.Log("onRewardedVideoAdClosedEvent");
                    UnityMainThreadDispatcher.instance.Enqueue(HandleonRewardedVideoAdClosedEvent);
                };
                IronSourceEvents.onRewardedVideoAvailabilityChangedEvent += (result) =>
                {
                    if (AdsManager.Debugging) Debug.Log("onRewardedVideoAvailabilityChangedEvent " + result.ToString());
                    var backupResult = result;
                    UnityMainThreadDispatcher.instance.Enqueue(() => { HandleonRewardedVideoAvailabilityChangedEvent(backupResult); });
                };
                IronSourceEvents.onRewardedVideoAdStartedEvent += () =>
                {
                    if (AdsManager.Debugging) Debug.Log("onRewardedVideoAdStartedEvent");
                    UnityMainThreadDispatcher.instance.Enqueue(HandleonRewardedVideoAdStartedEvent);
                };
                IronSourceEvents.onRewardedVideoAdEndedEvent += () =>
                {
                    if (AdsManager.Debugging) Debug.Log("onRewardedVideoAdEndedEvent");
                    UnityMainThreadDispatcher.instance.Enqueue(HandleonRewardedVideoAdEndedEvent);
                };
                IronSourceEvents.onRewardedVideoAdRewardedEvent += (result) =>
                {
                    if (AdsManager.Debugging) Debug.Log("onRewardedVideoAdRewardedEvent");
                    var backupResult = result;
                    UnityMainThreadDispatcher.instance.Enqueue(() => { HandleonRewardedVideoAdRewardedEvent(backupResult); });
                };
                IronSourceEvents.onRewardedVideoAdShowFailedEvent += (result) =>
                {
                    if (AdsManager.Debugging) Debug.Log("onRewardedVideoAdShowFailedEvent " + result.ToString());
                    var backupResult = result;
                    UnityMainThreadDispatcher.instance.Enqueue(() => { HandleonRewardedVideoAdShowFailedEvent(backupResult); });
                };
                IronSourceEvents.onRewardedVideoAdClickedEvent += (result) =>
                {
                    if (AdsManager.Debugging) Debug.Log("onRewardedVideoAdClickedEvent");
                    var backupResult = result;
                    UnityMainThreadDispatcher.instance.Enqueue(() => { HandleonRewardedVideoAdClickedEvent(backupResult); });
                };
            }

            if (AdsManager.I.SupportAdsType[(int)AdsType.OfferWall])
            {
                IronSourceConfig.Instance.setClientSideCallbacks(true);
                adsTypeSupport.Add(IronSourceAdUnits.OFFERWALL);

                //Offer Wall
                IronSourceEvents.onOfferwallClosedEvent += () =>
                {
                    if (AdsManager.Debugging) Debug.Log("onOfferwallClosedEvent");
                    UnityMainThreadDispatcher.instance.Enqueue(HandleonOfferwallClosedEvent);
                };
                IronSourceEvents.onOfferwallOpenedEvent += () =>
                {
                    if (AdsManager.Debugging) Debug.Log("onOfferwallOpenedEvent");
                    UnityMainThreadDispatcher.instance.Enqueue(HandleonOfferwallOpenedEvent);
                };
                IronSourceEvents.onOfferwallShowFailedEvent += (result) =>
                {
                    if (AdsManager.Debugging) Debug.Log("onOfferwallShowFailedEvent " + result.ToString());
                    var backupResult = result;
                    UnityMainThreadDispatcher.instance.Enqueue(() => { HandleonOfferwallShowFailedEvent(backupResult); });
                };
                IronSourceEvents.onOfferwallAdCreditedEvent += (result) =>
                {
                    if (AdsManager.Debugging) Debug.Log("onOfferwallAdCreditedEvent " + result.ToString());
                    var backupResult = result;
                    UnityMainThreadDispatcher.instance.Enqueue(() => { HandleonOfferwallAdCreditedEvent(backupResult); });
                };
                IronSourceEvents.onGetOfferwallCreditsFailedEvent += (result) =>
                {
                    if (AdsManager.Debugging) Debug.Log("onGetOfferwallCreditsFailedEvent " + result.ToString());
                    var backupResult = result;
                    UnityMainThreadDispatcher.instance.Enqueue(() => { HandleonGetOfferwallCreditsFailedEvent(backupResult); });
                };
                IronSourceEvents.onOfferwallAvailableEvent += (result) =>
                {
                    if (AdsManager.Debugging) Debug.Log("onOfferwallAvailableEvent");
                    var backupResult = result;
                    UnityMainThreadDispatcher.instance.Enqueue(() => { HandleonOfferwallAvailableEvent(backupResult); });
                };
            }

            IronSource.Agent.shouldTrackNetworkState(true);
            IronSource.Agent.init(Config.Platform.IronsourceId, adsTypeSupport.ToArray());            

            base.Init(parameters);

#if DEVELOPMENT_BUILD
            IronSource.Agent.validateIntegration();
#endif
        }

        void OnApplicationPause(bool isPaused)
        {
            IronSource.Agent.onApplicationPause(isPaused);

#if UNITY_ANDROID
            if (!isPaused && mFullScreenAdShowing)
            {
                StartCoroutine(CRInteruptWaitCheck());
            }
#endif
        }

        IEnumerator CRInteruptWaitCheck()
        {
            yield return new WaitForSecondsRealtime(1);

            if (FullscreenAdShowing && AdsManager.IsInstanceValid())//fix some adapter does not return Ads Close event on interrupt
            {
                if (mCurrentFullscreenAd != null)
                {
                    mCurrentFullscreenAd.OnAdAvailabilityUpdate(false);
                    mCurrentFullscreenAd = null;
                }
                if (mInterstitialAdCallback != null)
                {
                    mInterstitialAdCallback(false);
                    mInterstitialAdCallback = null;
                }
                if (mRewardAdCallback != null)
                {
                    mRewardAdCallback(false);
                    mRewardAdCallback = null;
                }
                FullscreenAdShowing = false;
                AudioListener.volume = 1;
            }
        }

        protected override AdStatusHandler CreateAdHandler(AdsType type, string id)
        {
            return new IronsourceAdStatusHandler(type, id);
        }

        protected override void UpdateDownloadList()
        {
            AdStatusHandler handler = null;
            if (AdsManager.I.IsAdEnabled(AdsType.RewardedVideo))
            {
                handler = CreateAdHandler(AdsType.RewardedVideo, "");
                mAdDownloadHandler.Add("rewardad", handler);
                mAdHighPriorityList.Add(handler);
                mDefaultRewardAdList.Add(handler);
            }
            if (AdsManager.I.IsAdEnabled(AdsType.OfferWall))
            {
                handler = CreateAdHandler(AdsType.OfferWall, "");
                mAdDownloadHandler.Add("offerwall", handler);
                mAdHighPriorityList.Add(handler);
                mDefaultOfferWallList.Add(handler);
            }
            if (AdsManager.I.IsAdEnabled(AdsType.Interstitial))
            {
                handler = CreateAdHandler(AdsType.Interstitial, "");
                mAdDownloadHandler.Add("interstitialad" + 0, handler);
                mAdHighPriorityList.Add(handler);
                mDefaultInterstitialAdList.Add(handler);
            }
            if (AdsManager.I.IsAdEnabled(AdsType.Banner))
            {
                handler = CreateAdHandler(AdsType.Banner, "");
                mAdDownloadHandler.Add("bannerad" + 0, handler);
                mAdHighPriorityList.Add(handler);
                mDefaultBannerAdList.Add(handler);
            }

            //Tin: not support sdk 6.16.1, currently Ironsource Option/Demanded Ad is messup too much, callback is not consistency 
            //base.UpdateDownloadList();
        }

        protected override void DownloadAd(AdStatusHandler ad)
        {
            if (AdsManager.Debugging) Debug.Log("DownloadAd " + ad._Type.ToString() + " id " + ad._Id);
            base.DownloadAd(ad);
            switch(ad._Type)
            {
                case AdsType.Banner:
                    IronSource.Agent.loadBanner(IronSourceBannerSize.BANNER, AdapterBannerPosition(mBannerPosition));
                    break;
                case AdsType.Interstitial:
                    IronSource.Agent.loadInterstitial();
                    break;
            }
        }

#region Banner
        IronSourceBannerPosition AdapterBannerPosition(BannerPosition position)
        {
            switch (position)
            {
                case BannerPosition.Top:
                case BannerPosition.TopLeft:
                case BannerPosition.TopRight:
                    return IronSourceBannerPosition.TOP;
            }
            return IronSourceBannerPosition.BOTTOM;
        }

        public override void SetBannerPosition(BannerPosition position)
        {
            if (mBannerPosition == position) return;
            base.SetBannerPosition(position);
            //TODO
        }

        public override void ShowAdsBanner()
        {
            base.ShowAdsBanner();

            if (mDefaultBannerAdList.Count <= 0 || !mDefaultBannerAdList[0]._Available) return;
            IronSource.Agent.displayBanner();
            if (AdsManager.EventOnBannerAdsChanged != null)
            {
                AdsManager.EventOnBannerAdsChanged(true);
            }
        }

        public override void HideAdsBanner()
        {
            base.HideAdsBanner();
            if (AdsManager.EventOnBannerAdsChanged != null)
            {
                AdsManager.EventOnBannerAdsChanged(false);
            }

            if (mDefaultBannerAdList.Count <= 0 || !mDefaultBannerAdList[0]._Available) return;
            IronSource.Agent.hideBanner();
            
        }

        void HandleonBannerAdLoadedEvent()
        {
            AdDownloadCallback(AdsType.Banner, true, null);
            if (mBannerAdVisibility)
            {
                ShowAdsBanner();
            }
            else
            {
                HideAdsBanner();
            }
        }

        void HandleonBannerAdLoadFailedEvent(IronSourceError error)
        {
            AdDownloadCallback(AdsType.Banner, false, error.ToString());
        }

        void HandleonBannerAdClickedEvent()
        {
            AFramework.Analytics.TrackingManager.I.TrackAdsClick(AdsType.Banner);
        }

        void HandleonBannerAdScreenPresentedEvent() { }
        void HandleonBannerAdScreenDismissedEvent() { }
        void HandleonBannerAdLeftApplicationEvent() { }
#endregion

#region Interstitial
        public override bool ShowAdsInterstitial(Action<bool> callback, string adId = null)
        {
            if (!IsInterstitialAdAvailable(adId)) return false;
            base.ShowAdsInterstitial(callback, adId);
            mCurrentFullscreenAd = mDefaultInterstitialAdList[0];

            if (mAdsInterstitialTimeoutThread != null)
            {
                StopCoroutine(mAdsInterstitialTimeoutThread);
                mAdsInterstitialTimeoutThread = null;
            }
            mAdsInterstitialTimeoutThread = CRAdsInterstitialTimeoutThread();
            StartCoroutine(mAdsInterstitialTimeoutThread);

            IronSource.Agent.showInterstitial();
            return true;
        }

        void HandleonInterstitialAdReadyEvent()
        {
            AdDownloadCallback(AdsType.Interstitial, true, null);
        }

        void HandleonInterstitialAdLoadFailedEvent(IronSourceError error)
        {
            AdDownloadCallback(AdsType.Interstitial, false, error.ToString());
        }

        void HandleonInterstitialAdOpenedEvent() { } //HandleonInterstitialAdShowSucceededEvent

        void HandleonInterstitialAdClosedEvent()
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
                mCurrentFullscreenAd = null;
            }
            FullscreenAdShowing = false;
        }

        void HandleonInterstitialAdShowSucceededEvent()
        {
            AudioListener.volume = 0;
            FullscreenAdShowing = true;

            if (mAdsInterstitialTimeoutThread != null)
            {
                StopCoroutine(mAdsInterstitialTimeoutThread);
                mAdsInterstitialTimeoutThread = null;
            }

            if (AdsManager.EventOnFullScreenAdsShown != null)
            {
                AdsManager.EventOnFullScreenAdsShown();
            }
        }

        void HandleonInterstitialAdShowFailedEvent(IronSourceError error)
        {
            if (mInterstitialAdCallback != null)
            {
                mInterstitialAdCallback(false);
                mInterstitialAdCallback = null;
            }
            if (mCurrentFullscreenAd != null) mCurrentFullscreenAd.OnAdAvailabilityUpdate(false);
            mCurrentFullscreenAd = null;
        }

        void HandleonInterstitialAdClickedEvent()
        {
            AFramework.Analytics.TrackingManager.I.TrackAdsClick(AdsType.Interstitial);
        }

        //void HandleonInterstitialAdReadyDemandOnlyEvent(string id) { HandleonInterstitialAdReadyEvent(); }
        //void HandleonInterstitialAdLoadFailedDemandOnlyEvent(string id, IronSourceError error) { HandleonInterstitialAdLoadFailedEvent(error); }
        //void HandleonInterstitialAdOpenedDemandOnlyEvent(string id) { HandleonInterstitialAdOpenedEvent(); }
        //void HandleonInterstitialAdClosedDemandOnlyEvent(string id) { HandleonInterstitialAdClosedEvent(); }
        //void HandleonInterstitialAdShowFailedDemandOnlyEvent(string id, IronSourceError error) { HandleonInterstitialAdShowFailedEvent(error); }
        //void HandleonInterstitialAdClickedDemandOnlyEvent(string id) { HandleonInterstitialAdClickedEvent(); }

        IEnumerator mAdsInterstitialTimeoutThread;
        IEnumerator CRAdsInterstitialTimeoutThread()
        {
            int currentFrame = Time.frameCount;
            WaitForSecondsRealtime waitTime = new WaitForSecondsRealtime(0.5f);
            float totalWaitTime = 7;
            int totalWaitFrame = currentFrame + 90;
            while (totalWaitTime > 0)
            {
                totalWaitTime -= 0.5f;
                yield return waitTime;
            }
            yield return new WaitUntil(() => Time.frameCount > totalWaitFrame);
            HandleonInterstitialAdShowFailedEvent(new IronSourceError(-1, "timeout"));
        }
#endregion

#region Reward
        public override bool ShowAdsReward(System.Action<bool> callback, string adId = null)
        {
            if (!IsRewardAdAvailable(adId)) return false;
            base.ShowAdsReward(callback);
            mCurrentFullscreenAd = mDefaultRewardAdList[0];

            if (mAdsRewardTimeoutThread != null)
            {
                StopCoroutine(mAdsRewardTimeoutThread);
                mAdsRewardTimeoutThread = null;
            }
            mAdsRewardTimeoutThread = CRAdsRewardTimeoutThread();
            StartCoroutine(mAdsRewardTimeoutThread);

            IronSource.Agent.showRewardedVideo();
            return true;
        }

        //IEnumerator CRAdsRewardInsurance()
        //{
        //    float insuranceTime = 35f;
        //    while (insuranceTime > 0 && FullscreenAdShowing)
        //    {
        //        insuranceTime -= Time.fixedUnscaledDeltaTime;
        //        if (insuranceTime <= 0)
        //        {
        //            mHaveRewarded = true;
        //        }
        //        yield return null;
        //    }
        //}

        void HandleonRewardedVideoAdShowFailedEvent(IronSourceError error)
        {
            AudioListener.volume = 1;
            if (mRewardAdCallback != null)
            {
                mRewardAdCallback(false);
                mRewardAdCallback = null;
            }
            if (mCurrentFullscreenAd != null) mCurrentFullscreenAd.OnAdAvailabilityUpdate(false);
            mCurrentFullscreenAd = null;
        }

        void HandleonRewardedVideoAdOpenedEvent()
        {
            AudioListener.volume = 0;
            FullscreenAdShowing = true;
            mHaveRewarded = false;

            if (mAdsRewardTimeoutThread != null)
            {
                StopCoroutine(mAdsRewardTimeoutThread);
                mAdsRewardTimeoutThread = null;
            }

            if (AdsManager.EventOnFullScreenAdsShown != null)
            {
                AdsManager.EventOnFullScreenAdsShown();
            }
            //StartCoroutine(CRAdsRewardInsurance());
        }

        IEnumerator mDelayRewardCheckThread;
        void HandleonRewardedVideoAdClosedEvent()
        {
            AudioListener.volume = 1;

            if (mCurrentFullscreenAd != null)
            {
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

        void HandleonRewardedVideoAdStartedEvent() { }
        void HandleonRewardedVideoAdEndedEvent() { }

        void HandleonRewardedVideoAdRewardedEvent(IronSourcePlacement placement)
        {
            mHaveRewarded = true;
        }
        void HandleonRewardedVideoAdClickedEvent(IronSourcePlacement placement)
        {
            AFramework.Analytics.TrackingManager.I.TrackAdsClick(AdsType.RewardedVideo);
        }

        IEnumerator mRewardedVideoDelayEventChanged = null;
        void HandleonRewardedVideoAvailabilityChangedEvent(bool isAvailable)
        {
            if (isAvailable && CurrentDownload != null && CurrentDownload == mDefaultRewardAdList[0])
            {
                AdDownloadCallback(AdsType.RewardedVideo, true, "");
            }
            else
            {
                mDefaultRewardAdList[0].OnAdAvailabilityUpdate(isAvailable);
            }

            if (BaseAdsAdapter.AdsAvailableSafeTime <= 0)
            {
                if ((object)InternalEventOnRewardAdsChanged != null)
                {
                    InternalEventOnRewardAdsChanged();
                }
            }
            else if (isAvailable)
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

        //void HandleonRewardedVideoAdLoadedDemandOnlyEvent(string id) { }
        //void HandleonRewardedVideoAdLoadFailedDemandOnlyEvent(string id, IronSourceError error) { /*TODO*/ }

        //void HandleonRewardedVideoAdOpenedDemandOnlyEvent(string id) { HandleonRewardedVideoAdOpenedEvent(); }
        //void HandleonRewardedVideoAdClosedDemandOnlyEvent(string id) { HandleonRewardedVideoAdClosedEvent(); }
        //void HandleonRewardedVideoAdRewardedDemandOnlyEvent(string id) { HandleonRewardedVideoAdRewardedEvent(); }
        //void HandleonRewardedVideoAdShowFailedDemandOnlyEvent(string id, IronSourceError error) { HandleonRewardedVideoAdShowFailedEvent(error); }
        //void HandleonRewardedVideoAdClickedDemandOnlyEvent(string id) { HandleonRewardedVideoAdClickedEvent(); }

        IEnumerator mAdsRewardTimeoutThread;
        IEnumerator CRAdsRewardTimeoutThread()
        {
            int currentFrame = Time.frameCount;
            WaitForSecondsRealtime waitTime = new WaitForSecondsRealtime(0.5f);
            float totalWaitTime = 7;
            int totalWaitFrame = currentFrame + 90;
            while (totalWaitTime > 0)
            {
                totalWaitTime -= 0.5f;
                yield return waitTime;
            }
            yield return new WaitUntil(() => Time.frameCount > totalWaitFrame);
            HandleonRewardedVideoAdShowFailedEvent(new IronSourceError(-1, "timeout"));
        }
#endregion
#region OfferWall

        public override bool ShowOfferWall(string placementName)
        {
            if (string.IsNullOrEmpty(placementName))
            {
                IronSource.Agent.showOfferwall();
            }

            IronSource.Agent.showOfferwall(placementName);
            return true;
        }

        public override void CheckOfferwallReward()
        {
            IronSource.Agent.getOfferwallCredits();
        }

        void HandleonOfferwallClosedEvent()
        {
            AudioListener.volume = 1;
        }

        void HandleonOfferwallOpenedEvent()
        {
            AudioListener.volume = 0;
        }

        void HandleonOfferwallShowFailedEvent(IronSourceError error)
        {
            AudioListener.volume = 1;
        }

        void HandleonOfferwallAdCreditedEvent(Dictionary<string, object> dict)
        {
            if (AdsManager.Debugging)
            {
                foreach (KeyValuePair<string, object> entry in dict)
                {
                    Debug.Log(string.Format("OfferwallAdCreditedEvent: {0}: {1}", entry.Value, entry.Key));
                }
            }
                
            if (mOfferWallCallback != null)
            {
                mOfferWallCallback(dict);
            }
        }

        void HandleonGetOfferwallCreditsFailedEvent(IronSourceError error)
        {
            if (mOfferWallCallback != null)
            {
                mOfferWallCallback(null);
            }
        }

        IEnumerator mOfferWallDelayEventChanged = null;
        void HandleonOfferwallAvailableEvent(bool isAvailable)
        {
            if (isAvailable && CurrentDownload != null && CurrentDownload == mDefaultOfferWallList[0])
            {
                AdDownloadCallback(AdsType.OfferWall, true, "");
            }
            else
            {
                mDefaultOfferWallList[0].OnAdAvailabilityUpdate(isAvailable);
            }

            if (BaseAdsAdapter.AdsAvailableSafeTime <= 0)
            {
                if ((object)InternalEventOnOfferWallChanged != null)
                {
                    InternalEventOnOfferWallChanged();
                }
            }
            else if (isAvailable)
            {
                if (mOfferWallDelayEventChanged != null)
                {
                    StopCoroutine(mOfferWallDelayEventChanged);
                    mOfferWallDelayEventChanged = null;
                }
                mOfferWallDelayEventChanged = CRDelayInternalEventOnOfferWallChanged();
                StartCoroutine(mOfferWallDelayEventChanged);
            }
            else
            {
                if (mOfferWallDelayEventChanged != null)
                {
                    StopCoroutine(mOfferWallDelayEventChanged);
                    mOfferWallDelayEventChanged = null;
                }

                if ((object)InternalEventOnOfferWallChanged != null)
                {
                    InternalEventOnOfferWallChanged();
                }
            }
        }

        IEnumerator CRDelayInternalEventOnOfferWallChanged()
        {
            WaitForSeconds waitTime = new WaitForSeconds(BaseAdsAdapter.AdsAvailableSafeTime);
            yield return waitTime;
            yield return null;
            mOfferWallDelayEventChanged = null;
            if ((object)InternalEventOnOfferWallChanged != null)
            {
                InternalEventOnOfferWallChanged();
            }
        }
#endregion

    }

    public class IronsourceAdStatusHandler : AdStatusHandler
    {
        public IronsourceAdStatusHandler(AdsType type, string id) : base(type, id) { }

        protected override bool BannerAdAvailable()
        {
            return _Available;
        }

        protected override bool InterstitialAdAvailable()
        {
            return string.IsNullOrEmpty(_Id) ? IronSource.Agent.isInterstitialReady() : IronSource.Agent.isISDemandOnlyInterstitialReady(_Id);
        }

        protected override bool RewardAdAvailable()
        {
            return string.IsNullOrEmpty(_Id) ? IronSource.Agent.isRewardedVideoAvailable() : IronSource.Agent.isISDemandOnlyRewardedVideoAvailable(_Id);
        }

        protected override bool OfferWallAvailable()
        {
            return IronSource.Agent.isOfferwallAvailable();
        }
    }
}
#endif