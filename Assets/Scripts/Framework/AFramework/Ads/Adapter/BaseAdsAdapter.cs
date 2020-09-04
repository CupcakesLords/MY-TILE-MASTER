using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AFramework.Ads
{
    public class BaseAdsAdapter : MonoBehaviour
    {
        protected delegate AdsLoadState LoadStateDelegate();
        protected delegate bool boolDelegate();

        public static System.Action InternalEventOnBannerAdsChanged;
        public static System.Action InternalEventOnInterstitialAdsChanged;
        public static System.Action InternalEventOnRewardAdsChanged;
        public static System.Action InternalEventOnOfferWallChanged;
        protected static int[] sAdsErrorDelayTime = new int[4] { 0, 2, 4, 6 };
        public const float AdsAvailableSafeTime = 1.0f;

        protected bool mInited = false;
        public bool IsInited { get { return mInited; } }
        protected BaseAdapterConfig mConfig;
        protected BannerPosition mBannerPosition = BannerPosition.Bottom;
        protected System.Action<bool> mRewardAdCallback;
        protected System.Action<Dictionary<string, object>> mOfferWallCallback;
        protected System.Action<bool> mInterstitialAdCallback;
        protected IEnumerator mAdsThreadHolder;

        protected bool mBannerAdVisibility = false;
        protected bool mFullScreenAdShowing = false;
        public bool FullscreenAdShowing { get { return mFullScreenAdShowing || mCurrentFullscreenAd != null; } protected set { mFullScreenAdShowing = value; } }

        public virtual void Init(object[] parameters)
        {
            UpdateDownloadList();
            mInited = true;
        }
        public virtual void StartAdsThread()
        {
            if (mConfig == null) return;
            if (mAdsThreadHolder != null) return;
            mAdsThreadHolder = CRAutoRequestThread();
            StartCoroutine(mAdsThreadHolder);
        }

        public virtual void SetBannerPosition(BannerPosition position) { mBannerPosition = position; }
        public virtual void ShowAdsBanner() { mBannerAdVisibility = true; }
        public virtual void HideAdsBanner() { mBannerAdVisibility = false; }
        public virtual bool ShowAdsInterstitial(System.Action<bool> callback, string adId = null)
        {
            mInterstitialAdCallback = callback;
            return false;
        }

        public void AddInterstitialCallback(System.Action<bool> callback)
        {
            mInterstitialAdCallback += callback;
        }

        public virtual bool ShowAdsReward(System.Action<bool> callback, string adId = null)
        {
            mRewardAdCallback = callback;
            return false;
        }

        public void AddRewardCallback(System.Action<bool> callback)
        {
            mRewardAdCallback += callback;
        }

        public virtual bool ShowOfferWall(string placementName)
        {
            return false;
        }

        public virtual void CheckOfferwallReward() { }

        public void SetOfferWallCallback(System.Action<Dictionary<string, object>> callback)
        {
            mOfferWallCallback = callback;
        }

        public virtual bool IsInterstitialAdAvailable(string adId = null)
        {
            if (string.IsNullOrEmpty(adId))
            {
                for (int i = 0; i < mDefaultInterstitialAdList.Count; ++i)
                {
                    if (mDefaultInterstitialAdList[i].IsAvailable()) return true;
                }
                return false;
            }
            return mAdDownloadHandler[adId].IsAvailable();
        }
        public virtual bool IsRewardAdAvailable(string adId = null)
        {
            if (string.IsNullOrEmpty(adId))
            {
                for (int i = 0; i < mDefaultRewardAdList.Count; ++i)
                {
                    if (mDefaultRewardAdList[i].IsAvailable()) return true;
                }
                return false;
            }
            return mAdDownloadHandler[adId].IsAvailable();
        }

        public virtual bool IsOfferWallAvailable(string adId = null)
        {
            if (string.IsNullOrEmpty(adId))
            {
                for (int i = 0; i < mDefaultOfferWallList.Count; ++i)
                {
                    if (mDefaultOfferWallList[i].IsAvailable()) return true;
                }
                return false;
            }
            return mAdDownloadHandler[adId].IsAvailable();
        }

        protected Dictionary<string, AdStatusHandler> mAdDownloadHandler = new Dictionary<string, AdStatusHandler>();
        protected List<AdStatusHandler> mAdHighPriorityList = new List<AdStatusHandler>();
        protected List<AdStatusHandler> mAdLowPriorityList = new List<AdStatusHandler>();
        protected List<AdStatusHandler> mDefaultBannerAdList = new List<AdStatusHandler>();
        protected List<AdStatusHandler> mDefaultInterstitialAdList = new List<AdStatusHandler>();
        protected List<AdStatusHandler> mDefaultRewardAdList = new List<AdStatusHandler>();
        protected List<AdStatusHandler> mDefaultOfferWallList = new List<AdStatusHandler>();
        protected AdsLoadState mAdLoadState = AdsLoadState.Idle;
        protected AdStatusHandler mCurrentDownload = null;
        protected AdStatusHandler CurrentDownload { get { return mCurrentDownload; } }
        protected AdStatusHandler mCurrentFullscreenAd = null;

        protected virtual AdStatusHandler CreateAdHandler(AdsType type, string id) { return new AdStatusHandler(type, id); }

        protected virtual void UpdateDownloadList()
        {
            var banners = mConfig.Platform.BannerlId.Split(';');
            var interstitials = mConfig.Platform.InterstitialId.Split(';');
            var rewards = mConfig.Platform.RewardedVideoId.Split(';');
            var offerwalls = mConfig.Platform.OfferWallId.Split(';');

            var bannerPlacements = mConfig.Platform.BannerAdPlacementConfig.Split(';');
            var interstitialPlacements = mConfig.Platform.InterstitialAdPlacementConfig.Split(';');
            var rewardPlacements = mConfig.Platform.RewardAdPlacementConfig.Split(';');
            var offerwallPlacements = mConfig.Platform.OfferWallPlacementConfig.Split(';');

            AdStatusHandler handler = null;
            if (rewards.Length > 0 && !string.IsNullOrEmpty(rewards[0]))
            {
                handler = CreateAdHandler(AdsType.RewardedVideo, rewards[0]);
                mAdDownloadHandler.Add(handler._Id, handler);
                mAdHighPriorityList.Add(handler);
                mDefaultRewardAdList.Add(handler);
            }
            if (offerwalls.Length > 0 && !string.IsNullOrEmpty(offerwalls[0]))
            {
                handler = CreateAdHandler(AdsType.OfferWall, offerwalls[0]);
                mAdDownloadHandler.Add(handler._Id, handler);
                mAdHighPriorityList.Add(handler);
                mDefaultOfferWallList.Add(handler);
            }
            if (interstitials.Length > 0 && !string.IsNullOrEmpty(interstitials[0]))
            {
                handler = CreateAdHandler(AdsType.Interstitial, interstitials[0]);
                mAdDownloadHandler.Add(handler._Id, handler);
                mAdHighPriorityList.Add(handler);
                mDefaultInterstitialAdList.Add(handler);
            }
            if (banners.Length > 0 && !string.IsNullOrEmpty(banners[0]))
            {
                handler = CreateAdHandler(AdsType.Banner, banners[0]);
                mAdDownloadHandler.Add(handler._Id, handler);
                mAdHighPriorityList.Add(handler);
                mDefaultBannerAdList.Add(handler);
            }
            

            int length = Mathf.Max(bannerPlacements.Length / 2, interstitialPlacements.Length / 2, rewardPlacements.Length / 2, offerwallPlacements.Length / 2);
            for (int i = 0; i < length; ++i)
            {
                int index = i * 2;
                if (index < rewardPlacements.Length && !mAdDownloadHandler.ContainsKey(rewardPlacements[index + 1]))
                {
                    handler = CreateAdHandler(AdsType.RewardedVideo, rewardPlacements[index + 1]);
                    mAdDownloadHandler.Add(handler._Id, handler);
                    mAdLowPriorityList.Add(handler);
                }
                if (index < offerwallPlacements.Length && !mAdDownloadHandler.ContainsKey(offerwallPlacements[index + 1]))
                {
                    handler = CreateAdHandler(AdsType.RewardedVideo, offerwallPlacements[index + 1]);
                    mAdDownloadHandler.Add(handler._Id, handler);
                    mAdLowPriorityList.Add(handler);
                }
                if (index < interstitialPlacements.Length && !mAdDownloadHandler.ContainsKey(interstitialPlacements[index + 1]))
                {
                    handler = CreateAdHandler(AdsType.Interstitial, interstitialPlacements[index + 1]);
                    mAdDownloadHandler.Add(handler._Id, handler);
                    mAdLowPriorityList.Add(handler);
                }
                if (index < bannerPlacements.Length && !mAdDownloadHandler.ContainsKey(bannerPlacements[index + 1]))
                {
                    handler = CreateAdHandler(AdsType.Banner, bannerPlacements[index + 1]);
                    mAdDownloadHandler.Add(handler._Id, handler);
                    mAdLowPriorityList.Add(handler);
                }
            }

            length = Mathf.Max(rewards.Length, interstitials.Length, banners.Length, offerwalls.Length);
            for (int i = 1; i < length; ++i)
            {
                if (i < rewards.Length && !mAdDownloadHandler.ContainsKey(rewards[i]))
                {
                    handler = CreateAdHandler(AdsType.RewardedVideo, rewards[i]);
                    mAdDownloadHandler.Add(handler._Id, handler);
                    mAdLowPriorityList.Add(handler);
                    mDefaultRewardAdList.Add(handler);
                }
                if (i < offerwalls.Length && !mAdDownloadHandler.ContainsKey(offerwalls[i]))
                {
                    handler = CreateAdHandler(AdsType.RewardedVideo, offerwalls[i]);
                    mAdDownloadHandler.Add(handler._Id, handler);
                    mAdLowPriorityList.Add(handler);
                    mDefaultOfferWallList.Add(handler);
                }
                if (i < interstitials.Length && !mAdDownloadHandler.ContainsKey(interstitials[i]))
                {
                    handler = CreateAdHandler(AdsType.Interstitial, interstitials[i]);
                    mAdDownloadHandler.Add(handler._Id, handler);
                    mAdLowPriorityList.Add(handler);
                    mDefaultInterstitialAdList.Add(handler);
                }
                if (i < banners.Length && !mAdDownloadHandler.ContainsKey(banners[i]))
                {
                    handler = CreateAdHandler(AdsType.Banner, banners[i]);
                    mAdDownloadHandler.Add(handler._Id, handler);
                    mAdLowPriorityList.Add(handler);
                    mDefaultBannerAdList.Add(handler);
                }
            }
        }

        IEnumerator CRAutoRequestThread()
        {
            var highPriorityList = mAdHighPriorityList.ToArray();
            var lowPriorityList = mAdLowPriorityList.ToArray();
            if (highPriorityList.Length <= 0) yield break;
            int offsetIndex = 0;
            int errorCount = 0;
            float downloadTime = 0;
            AdStatusHandler lastDownloadHandler = null;
            while (true)
            {
                AdStatusHandler selectedDownload = null;
                for (int i = 0; i < highPriorityList.Length; ++i)
                {
                    if (!highPriorityList[i].IsAvailable(0) && highPriorityList[i] != lastDownloadHandler)
                    {
                        selectedDownload = highPriorityList[i];
                        break;
                    }
                }

                if (selectedDownload == null)
                {
                    for (int i = 0; i < lowPriorityList.Length; ++i)
                    {
                        int index = (offsetIndex + i) % lowPriorityList.Length;
                        if (!lowPriorityList[index].IsAvailable(0) && lowPriorityList[index] != lastDownloadHandler)
                        {
                            selectedDownload = lowPriorityList[index];
                            break;
                        }
                    }
                    ++offsetIndex;
                }

                lastDownloadHandler = selectedDownload;
                if (selectedDownload != null)
                {
                    while (mCurrentFullscreenAd != null || FullscreenAdShowing)
                    {
                        yield return null;
                    }

                    DownloadAd(selectedDownload);
                    downloadTime = 0;
                    while (mAdLoadState == AdsLoadState.Downloading)
                    {
                        yield return null;

                        downloadTime += Time.deltaTime;
                        if (downloadTime >= mConfig.GetDownloadTimeout())
                        {
                            mAdLoadState = AdsLoadState.Error;
                            if (selectedDownload != null) AdDownloadCallback(selectedDownload._Type, false, "timeout");
                        }
                    }

                    if (mAdLoadState == AdsLoadState.Error)
                    {
                        yield return new WaitForSeconds(mConfig.GetErrorRetryInterval() + sAdsErrorDelayTime[Mathf.Min(errorCount, sAdsErrorDelayTime.Length - 1)]);
                        ++errorCount;
                    }
                    else
                    {
                        errorCount = 0;
                    }
                }
                
                yield return null;
            }
        }

        protected virtual void DownloadAd(AdStatusHandler ad)
        {
            mAdLoadState = AdsLoadState.Downloading;
            mCurrentDownload = ad;
            //Need each adapter to implement it's own code
        }

        protected virtual void AdDownloadCallback(AdsType adType, bool result, string message)
        {
            if (mCurrentDownload == null)
            {
                if (AdsManager.Debugging) Debug.Log("AdDownload mCurrentDownload is null but there is still have callback");
                return;
            }

            if (adType != mCurrentDownload._Type)
            {
                if (AdsManager.Debugging) Debug.Log("AdDownload callback type " + adType + " does not match current download type " + mCurrentDownload._Type);
                return;
            }

            if (result)
            {
                if (AdsManager.Debugging) Debug.Log("AdDownload " + mCurrentDownload._Type + " id " + mCurrentDownload._Id + " result success");
                mAdLoadState = AdsLoadState.Loaded;
            }
            else
            {
                if (AdsManager.Debugging) Debug.Log("AdDownload " + mCurrentDownload._Type + " id " + mCurrentDownload._Id + " result failed: " + message);
                mAdLoadState = AdsLoadState.Error;
            }
            mCurrentDownload.OnAdAvailabilityUpdate(result);
            mCurrentDownload = null;
        }
    }

    public class AdStatusHandler
    {
        public AdsType _Type { get; protected set; }
        public string _Id { get; protected set; }
        public bool _Available { get; protected set; }

        public AdStatusHandler(AdsType type, string id)
        {
            _Type = type;
            _Id = id;
        }

        const float MaxAvailableTime = float.MaxValue - 999;
        protected float mLastCheckTime = -1;
        protected float mAdAvailableTime = MaxAvailableTime;
        public virtual bool IsAvailable(float safeTime = BaseAdsAdapter.AdsAvailableSafeTime)
        {
            float checkTime = _Available ? 5.0f : 1.0f;
            if (mLastCheckTime + checkTime < Time.time)
            {
                bool cache = _Available;
                switch (_Type)
                {
                    case AdsType.Banner:
                        cache = BannerAdAvailable();
                        break;
                    case AdsType.Interstitial:
                        cache = InterstitialAdAvailable();
                        break;
                    case AdsType.RewardedVideo:
                        cache = RewardAdAvailable();
                        break;
                    case AdsType.OfferWall:
                        cache = OfferWallAvailable();
                        break;
                }
                
                if (cache != _Available)
                {
                    OnAdAvailabilityUpdate(cache);
                }
                else
                {
                    mLastCheckTime = Time.time;
                }
            }
            return _Available && Time.time >= (mAdAvailableTime + safeTime);
        }

        protected virtual bool BannerAdAvailable() { return false; }
        protected virtual bool InterstitialAdAvailable() { return false; }
        protected virtual bool RewardAdAvailable() { return false; }
        protected virtual bool OfferWallAvailable() { return false; }

        public virtual void OnAdAvailabilityUpdate(bool result)
        {
            mLastCheckTime = Time.time;
            _Available = result;
            if (result) mAdAvailableTime = Time.time;
            else mAdAvailableTime = MaxAvailableTime;
        }
    }

}