using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

#if USE_FIREBASE && USE_FIREBASE_REMOTECONFIG
namespace AFramework.FirebaseService
{
    public class FirebaseRemoteConfig : MonoBehaviour
    {
        const string AF_CUSTOM_EVENT = "af_custom_event_v2";
        const string ADS_INTERSTITIAL_FOR_REWARD = "ads_interstitial_for_reward";
        const string FIREBASE_EXPERIMENT_ID = "firebase_experiment_id";

        public static System.Action EventFectchData;

        protected bool mInited = false;
        protected bool mIsLoading = false;
        protected bool mLoadSuccess = false;
        protected double mLastLoadTime = -999999999;

        void Start()
        {
            FirebaseInstance.ChecAndTryInit(Init);
        }

        void Init()
        {
            SetupDefaultConfig();
            mInited = true;

            FetchDataAsync();
        }

        protected virtual void SetupDefaultConfig()
        {
            System.Collections.Generic.Dictionary<string, object> defaults =
              new System.Collections.Generic.Dictionary<string, object>();
            
            //need to override this function to init default value

            Firebase.RemoteConfig.FirebaseRemoteConfig.SetDefaults(defaults);
        }

        protected virtual void UpdateGameParams()
        {
            var ads_config = Firebase.RemoteConfig.FirebaseRemoteConfig.GetValue(ADS_INTERSTITIAL_FOR_REWARD);
            if (ads_config.Source == Firebase.RemoteConfig.ValueSource.RemoteValue && AFramework.Ads.AdsManager.IsInstanceValid())
            {
                AFramework.Ads.AdsManager.I.SetUseInterstitialForReward(ads_config.BooleanValue);
            }

            var af_events = Firebase.RemoteConfig.FirebaseRemoteConfig.GetValue(AF_CUSTOM_EVENT);
            if (af_events.Source == Firebase.RemoteConfig.ValueSource.RemoteValue)
            {
                AFramework.Analytics.TrackingManager.I.UpdateCustomRuleData(af_events.StringValue);
            }

#if USE_FB_ANALYTICS
            var fb_experiment_id = Firebase.RemoteConfig.FirebaseRemoteConfig.GetValue(FIREBASE_EXPERIMENT_ID);
            if (fb_experiment_id.Source == Firebase.RemoteConfig.ValueSource.RemoteValue && !string.IsNullOrEmpty(fb_experiment_id.StringValue))
            {
                Analytics.FacebookAnalytics.SetExperimentId(fb_experiment_id.StringValue);
            }
#endif
        }

        Task FetchDataAsync()
        {
            if (!mInited || mIsLoading || mLastLoadTime + 12 * 60 * 60 > AFramework.Utility.GetCurrentTimeSecond())
            {
                return null;
            }

            mIsLoading = true;
            System.Threading.Tasks.Task fetchTask = Firebase.RemoteConfig.FirebaseRemoteConfig.FetchAsync(
#if DEVELOPMENT_BUILD
                System.TimeSpan.Zero
#endif
            );
            return fetchTask.ContinueWith(FetchComplete);
        }

        void FetchComplete(Task fetchTask)
        {
            var info = Firebase.RemoteConfig.FirebaseRemoteConfig.Info;
            switch (info.LastFetchStatus)
            {
                case Firebase.RemoteConfig.LastFetchStatus.Success:
                    Firebase.RemoteConfig.FirebaseRemoteConfig.ActivateFetched();
                    mLoadSuccess = true;
                    UnityMainThreadDispatcher.instance.Enqueue(UpdateGameParams);
                    mLastLoadTime = AFramework.Utility.GetCurrentTimeSecond();
                    //DebugLog(String.Format("Remote data loaded and ready (last fetch time {0}).",
                    //                       info.FetchTime));
                    break;
                case Firebase.RemoteConfig.LastFetchStatus.Failure:
                    {
                        var dic = new Dictionary<string, object>();
                        dic["reason"] = info.LastFetchFailureReason.ToString();
                        AFramework.Analytics.TrackingManager.instance.TrackEvent("REMOTE_CONFIG_FAIL", dic);
                        switch (info.LastFetchFailureReason)
                        {
                            case Firebase.RemoteConfig.FetchFailureReason.Error:
                                //DebugLog("Fetch failed for unknown reason");
                                mLastLoadTime = AFramework.Utility.GetCurrentTimeSecond() + 5 * 60;
                                break;
                            case Firebase.RemoteConfig.FetchFailureReason.Throttled:
                                //DebugLog("Fetch throttled until " + info.ThrottledEndTime);
                                mLastLoadTime = info.ThrottledEndTime.ToUniversalTime().Subtract(System.DateTime.MinValue).TotalSeconds;
                                break;
                        }
                    }
                    break;
                case Firebase.RemoteConfig.LastFetchStatus.Pending:
                    //DebugLog("Latest Fetch call still pending.");
                    mLastLoadTime = AFramework.Utility.GetCurrentTimeSecond() + 5 * 60;
                    break;
            }
            mIsLoading = false;
        }

        protected virtual void OnApplicationPause(bool isPaused)
        {
            if (!isPaused)
            {
                FetchDataAsync();
            }
        }
    }
}
#endif
