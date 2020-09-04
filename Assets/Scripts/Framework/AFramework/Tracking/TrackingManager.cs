using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AFramework;
using AFramework.Analytics;

namespace AFramework.Analytics
{
    public class TrackingManager : AFramework.SingletonMono<TrackingManager>
    {
        public const string EVENT_IAP_CLICK = "IAP_CLICKED";
        public const string EVENT_IAP_BOUGHT = "IAP_BOUGHT";
        public const string EVENT_ADS_UNKNOWN = "ADS_UNKNOWN";
        public const string EVENT_ADS_BANNER = "ADS_BANNER";
        public const string EVENT_ADS_INTERSTITIAL = "ADS_INTERSTITIAL";
        public const string EVENT_ADS_REWARD = "ADS_REWARD";

        public virtual string AppflyerKey { get { return ""; } }
        public virtual string AppflyerAppId { get { return ""; } }

        protected List<IAnalytic> Analytics = new List<IAnalytic>();
        protected List<IAnalytic> LimitedAnalytics = new List<IAnalytic>();
        protected Dictionary<string, List<CustomRuleEvent>> appsflyerCustomEventList = new Dictionary<string, List<CustomRuleEvent>>();

        protected string AppsflyerID { get; private set; }

        protected void Start()
        {
            AppsflyerID = null;
            InitTracking();
        }

        protected virtual void InitTracking()
        {
            IAnalytic analytic;
#if USE_APPSFLYER_ANALYTICS
            var afCallbackObj = new GameObject("AppsFlyerTrackerCallbacks", typeof(AppsFlyerTrackerCallbacks));
            afCallbackObj.transform.SetParent(this.transform);

            analytic = new AppsFlyerAnalytics();
            LimitedAnalytics.Add(analytic);
            analytic.Init(AppflyerKey, AppflyerAppId);
            AppsflyerID = analytic.InitSuccess ? (analytic as AppsFlyerAnalytics).UDID : null;
#endif
#if USE_FB_ANALYTICS
            analytic = new FacebookAnalytics();
            Analytics.Add(analytic);
            analytic.Init();
#endif
#if USE_UNITY_ANALYTICS
            analytic = new AFramework.Analytics.UnityAnalytics();
            Analytics.Add(analytic);
            analytic.Init();
#endif
#if USE_FIREBASE_ANALYTICS
            analytic = new FirebaseAnalytics();
            Analytics.Add(analytic);
            analytic.Init();
#endif
        }

        public void TrackEvent(string eventName)
        {
            TrackEvent(eventName, new Dictionary<string, object>());
        }
        
        public void TrackEvent(string eventName, params string[] list)
        {
            Dictionary<string, object> Dic = new Dictionary<string, object>();
            for (int i = 0; i < list.Length; i += 2)
                Dic[list[i]] = list[i + 1];
            TrackEvent(eventName, Dic);
        }

        public void TrackEvent(string eventName, Dictionary<string, object> parameters)
        {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
            if (parameters == null) Debug.LogError("TrackEvent parameters is NULL");
#endif
            TrackCustomRuleLimitedEvent(eventName, parameters);

            if (AppsflyerID != null)
            {
                parameters["appsflyer_id"] = AppsflyerID;
            }

#if FAKE_TRACKING
            string paramLog = "";
            foreach(var pair in parameters)
            {
                paramLog += string.Format(" {0} {1}", pair.Key, pair.Value.ToString());
            }
            Debug.Log("Normal event: " + eventName + paramLog);
#else
            foreach (IAnalytic a in Analytics)
            {
                a.TrackEvent(eventName, parameters);
            }
#endif
        }

        public void TrackEvent(string eventName, string Key, string value)
        {
            Dictionary<string, object> Dic = new Dictionary<string, object>();
            Dic[Key] = value;
            TrackEvent(eventName, Dic);
        }

        public void TrackLimitedEvent(string eventName, Dictionary<string, object> parameters)
        {
#if FAKE_TRACKING
            string paramLog = "";
            foreach (var pair in parameters)
            {
                paramLog += string.Format(" {0} {1}", pair.Key, pair.Value.ToString());
            }
            Debug.Log("Limit event: " + eventName + paramLog);
#else
            foreach (IAnalytic a in LimitedAnalytics)
            {
                a.TrackEvent(eventName, parameters);
            }
#endif
        }

        public void TrackLimitedEvent(string eventName)
        {
#if FAKE_TRACKING
            Debug.Log("Limit event: " + eventName);
#else
            foreach (IAnalytic a in LimitedAnalytics)
            {
                a.TrackEvent(eventName);
            }
#endif
        }


        //private void OnApplicationQuit()
        //{
        //    Debug.LogError("TrackingManager: Add tracking OnApplicationQuit");
        //}

        void OnApplicationPause(bool pauseStatus)
        {
            foreach (IAnalytic a in Analytics)
            {
                a.ApplicationOnPause(pauseStatus);
            }
        }

        public void TrackIAPClick(string packageId, string location)
        {
            var parameters = new Dictionary<string, object>();
            parameters["pack"] = packageId;
            parameters["screen"] = location;
            TrackEvent(EVENT_IAP_CLICK, parameters);
        }

        public void TrackIAPPurchase(string packageId, string receipt, object validated, string location)
        {
#if UNITY_ANDROID
            if (AFramework.IAP.IAPManager.TamperedStore || receipt == null) return;

            if (validated == null)
            {
                return;
            }
#if UNITY_PURCHASING
            UnityEngine.Purchasing.Security.IPurchaseReceipt[] validatedData = (UnityEngine.Purchasing.Security.IPurchaseReceipt[])validated;
            string currentPack = validatedData[0].productID;
            if (currentPack != packageId)
            {
                return;
            }
#endif
#endif

            IAP.PackageInfo iapInfo = AFramework.IAP.IAPManager.instance.PackageIdentifierToPackageInfo(packageId, false);
            if (iapInfo == null)
            {
                iapInfo = AFramework.IAP.IAPManager.instance.PackageIdentifierToPackageInfo(packageId, true);
            }
            if (iapInfo == null) return;

            {
                var parameters = new Dictionary<string, object>();
                //parameters["value"] = iapInfo.Price;
                //parameters["currency"] = iapInfo.Currency;
                //parameters["type"] = iapInfo.Type.ToString();
                parameters["pack"] = iapInfo.PackageIdentifier.getString();
                parameters["screen"] = location;

                TrackEvent(EVENT_IAP_BOUGHT, parameters);
            }

#if USE_APPSFLYER_ANALYTICS
            {
                var parameters = new Dictionary<string, object>();
                parameters[AFInAppEvents.REVENUE] = iapInfo.Price * 0.69;
                parameters[AFInAppEvents.CURRENCY] = iapInfo.Currency;
                parameters[AFInAppEvents.CONTENT_TYPE] = iapInfo.Type.ToString();
                parameters[AFInAppEvents.CONTENT_ID] = iapInfo.PackageIdentifier.getString();
                parameters[AFInAppEvents.PRICE] = iapInfo.Price;
                parameters[AFInAppEvents.QUANTITY] = 1;

                TrackLimitedEvent(AFInAppEvents.PURCHASE, parameters);
            }
#endif
        }

#region Ads
        Dictionary<string, object> mRewardAdsTrackingCacheData;
        public virtual void TrackAdsView(AFramework.Ads.AdsType adsType, Dictionary<string, object> args)
        {
            mRewardAdsTrackingCacheData = null;
            switch (adsType)
            {
                case Ads.AdsType.Banner:
                    TrackEvent("ADS_BANNER_IMPRESSION", args);
                    break;
                case Ads.AdsType.Interstitial:
                    TrackEvent("ADS_INTERSTITIAL_IMPRESSION", args);
                    TrackLimitedEvent("af_ad_view_interstitial");
                    break;
                case Ads.AdsType.RewardedVideo:
                    mRewardAdsTrackingCacheData = args;
                    TrackEvent("ADS_REWARD_IMPRESSION", args);
                    TrackLimitedEvent("af_ad_view_rewarded");
                    break;
                case Ads.AdsType.InterstitialRewardedVideo:
                    mRewardAdsTrackingCacheData = args;
                    TrackEvent("ADS_REWARD_INTERSTITIAL_IMPRESSION", args);
                    TrackLimitedEvent("af_ad_view_rewarded");
                    break;
                case Ads.AdsType.OfferWall:
                    TrackEvent("ADS_OFFERWALL_IMPRESSION", args);
                    break;
            }
        }

        public virtual void TrackAdsClick(AFramework.Ads.AdsType adsType)
        {
            switch (adsType)
            {
                case Ads.AdsType.Banner:
                    TrackEvent("ADS_BANNER_CLICK");
                    break;
                case Ads.AdsType.Interstitial:
                    if (mRewardAdsTrackingCacheData != null)//should be Ads.AdsType.InterstitialRewardedVideo
                    {
                        TrackEvent("ADS_REWARD_INTERSTITIAL_CLICK", mRewardAdsTrackingCacheData);
                    }
                    else
                    {
                        TrackEvent("ADS_INTERSTITIAL_CLICK");
                    }
                    break;
                case Ads.AdsType.RewardedVideo:
                    TrackEvent("ADS_REWARD_CLICK", mRewardAdsTrackingCacheData);
                    break;
            }

            {
                var parameters = new Dictionary<string, object>();
                parameters["af_adrev_ad_type"] = adsType.ToString();
                TrackLimitedEvent("af_ad_click", parameters);
            }
        }

        public virtual void TrackAdsReady(AFramework.Ads.AdsType adsType, Dictionary<string, object> args)
        {
            switch (adsType)
            {
                case Ads.AdsType.Banner:
                    TrackEvent("ADS_BANNER_REQUEST", args);
                    break;
                case Ads.AdsType.Interstitial:
                    TrackEvent("ADS_INTERSTITIAL_REQUEST", args);
                    break;
                case Ads.AdsType.RewardedVideo:
                    mRewardAdsTrackingCacheData = args;
                    TrackEvent("ADS_REWARD_REQUEST", args);
                    break;
                case Ads.AdsType.OfferWall:
                    TrackEvent("ADS_OFFERWALL_REQUEST", args);
                    break;
            }
        }
#endregion

        public virtual void TrackMenuActiveTime(string menuName, float time, Dictionary<string, object> additionalParams = null)
        {
            var dic = new Dictionary<string, object>();
            dic["name"] = menuName;
            dic["time"] = time;
            if (additionalParams != null)
            {
                foreach (var pair in additionalParams)
                {
                    dic[pair.Key] = pair.Value;
                }
            }
            TrackEvent("MENU", dic);
        }

        public virtual void FirebaseTutorialBegin(Dictionary<string, object> additionalParams = null)
        {
            if (additionalParams != null)
            {
                TrackEvent("tutorial_begin", additionalParams);
            }
            else
            {
                TrackEvent("tutorial_begin");
            }
        }

        public virtual void FirebaseTutorialComplete(Dictionary<string, object> additionalParams = null)
        {
            if (additionalParams != null)
            {
                TrackEvent("tutorial_complete", additionalParams);
            }
            else
            {
                TrackEvent("tutorial_complete");
            }
        }

        public virtual void FirebaseLevelUp(string character, int level, Dictionary<string, object> additionalParams = null)
        {
            var dic = new Dictionary<string, object>();
            dic["character"] = character;
            dic["level"] = level;
            if (additionalParams != null)
            {
                foreach (var pair in additionalParams)
                {
                    dic[pair.Key] = pair.Value;
                }
            }
            TrackEvent("level_up", dic);
        }

        public virtual void FirebasePostScore(string character, int level, int score, Dictionary<string, object> additionalParams = null)
        {
            var dic = new Dictionary<string, object>();
            dic["character"] = character;
            dic["level"] = level;
            dic["score"] = score;
            if (additionalParams != null)
            {
                foreach (var pair in additionalParams)
                {
                    dic[pair.Key] = pair.Value;
                }
            }
            TrackEvent("post_score", dic);
        }

        public virtual void FirebaseEarnVirtualCurrency(string _type, int _amount, string _reason, Dictionary<string, object> additionalParams = null)
        {
            var dic = new Dictionary<string, object>();
            dic["virtual_currency_name"] = _type;
            dic["value"] = _amount;
            dic["reason"] = _reason;
            if (additionalParams != null)
            {
                foreach (var pair in additionalParams)
                {
                    dic[pair.Key] = pair.Value;
                }
            }
            TrackEvent("earn_virtual_currency", dic);
        }

        public virtual void FirebaseSpendVirtualCurrency(string _type, int _amount, string _reason, Dictionary<string, object> additionalParams = null)
        {
            var dic = new Dictionary<string, object>();
            dic["virtual_currency_name"] = _type;
            dic["value"] = _amount;
            dic["item_name"] = _reason;
            if (additionalParams != null)
            {
                foreach (var pair in additionalParams)
                {
                    dic[pair.Key] = pair.Value;
                }
            }
            TrackEvent("spend_virtual_currency", dic);
        }

        public virtual void FirebaseSelectContent(string content_type, string item_id, Dictionary<string, object> additionalParams = null)
        {
            var dic = new Dictionary<string, object>();
            dic["content_type"] = content_type;
            dic["item_id"] = item_id;
            if (additionalParams != null)
            {
                foreach (var pair in additionalParams)
                {
                    dic[pair.Key] = pair.Value;
                }
            }
            TrackEvent("select_content", dic);
        }

        public virtual void UpdateCustomRuleData(string data)
        {
            if (string.IsNullOrEmpty(data))
            {
                appsflyerCustomEventList = new Dictionary<string, List<CustomRuleEvent>>();
                return;
            }
            CustomRuleEvent[] eventData = JsonHelper.getJsonArray<CustomRuleEvent>(data);
            var newEventList = new Dictionary<string, List<CustomRuleEvent>>();
            for (int i = 0; i < eventData.Length; i++)
            {
                if (eventData[i].Build())
                {
                    if (!newEventList.ContainsKey(eventData[i].eventName))
                    {
                        newEventList[eventData[i].eventName] = new List<CustomRuleEvent>();
                    }
                    newEventList[eventData[i].eventName].Add(eventData[i]);
                }
            }
            appsflyerCustomEventList = newEventList;
        }

        void TrackCustomRuleLimitedEvent(string eventName, Dictionary<string, object> parameters)
        {
            if (!appsflyerCustomEventList.ContainsKey(eventName)) return;
            var listEvent = appsflyerCustomEventList[eventName];
            for (int i = 0; i < listEvent.Count; ++i)
            {
                var result = listEvent[i].ProcessEvent(parameters);
                if (string.IsNullOrEmpty(result)) continue;
                TrackLimitedEvent(result);
            }
            
        }
    }
}
