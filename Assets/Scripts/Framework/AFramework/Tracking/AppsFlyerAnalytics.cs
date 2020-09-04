using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#if USE_APPSFLYER_ANALYTICS
namespace AFramework.Analytics
{
    public class AppsFlyerAnalytics : IAnalytic
    {
        public bool InitSuccess { get; set; }
        public string UDID { get { return InitSuccess ? AppsFlyer.getAppsFlyerId() : null; } }

        public void ApplicationOnPause(bool Paused)
        {

        }

        public void Init(params string[] args)
        {
            //Mandatory - set your AppsFlyer’s Developer key.
            string appflyerKey = args[0];
            string appId = args[1];
            if (appflyerKey == "" || appId == "") return;

            AppsFlyer.setAppsFlyerKey(appflyerKey);
            //AppsFlyer.setIsDebug(true);
#if UNITY_IOS
            AppsFlyer.setAppID(appId);
            AppsFlyer.trackAppLaunch ();
#elif UNITY_ANDROID
            //Mandatory - set your Android package name
            AppsFlyer.setAppID(appId);
            AppsFlyer.init(appflyerKey);
#endif
            InitSuccess = true;
#if DEVELOPMENT_BUILD
            AppsFlyer.setIsSandbox(true);
#endif

#if APPSFLYER_UNINSTALL_EVENT
#if     USE_FIREBASE_MESSAGING && UNITY_ANDROID
            if (AFramework.FirebaseService.FirebaseMessaging.FirebaseMessagingToken == null || AFramework.FirebaseService.FirebaseMessaging.FirebaseMessagingToken.Length <= 0)
            {
                AFramework.FirebaseService.FirebaseMessaging.EventOnTokenReceived += OnFirebaseMessagingTokenReceived;
            }
            else
            {
                AppsFlyer.updateServerUninstallToken(AFramework.FirebaseService.FirebaseMessaging.FirebaseMessagingToken);
            }
#elif   UNITY_IOS
            UnityEngine.iOS.NotificationServices.RegisterForNotifications(UnityEngine.iOS.NotificationType.Alert | UnityEngine.iOS.NotificationType.Badge | UnityEngine.iOS.NotificationType.Sound, true);
            TrackingManager.I.StartCoroutine(CRWaitForNotificationToken());
#endif
#endif
        }

        public void TrackEvent(string eventName, Dictionary<string, object> parameters)
        {
            Dictionary<string, string> temp = parameters.ToDictionary(x => x.Key.ToString(), x => x.Value.ToString());
            AppsFlyer.trackRichEvent(eventName, temp);
        }

        public void TrackEvent(string eventName)
        {
            var tempDictionary = new Dictionary<string, string>();
            AppsFlyer.trackRichEvent(eventName, tempDictionary);
        }

#if APPSFLYER_UNINSTALL_EVENT
#if     USE_FIREBASE_MESSAGING && UNITY_ANDROID
        public void OnFirebaseMessagingTokenReceived(Firebase.Messaging.TokenReceivedEventArgs token)
        {
            AppsFlyer.updateServerUninstallToken(token.Token);
        }
#endif
#if     UNITY_IOS
        IEnumerator CRWaitForNotificationToken()
        {
            WaitForSecondsRealtime waitTime = new WaitForSecondsRealtime(0.2f);
            byte[] token = null;
            do
            {
                yield return waitTime;
                token = UnityEngine.iOS.NotificationServices.deviceToken;
            } while (token == null);
            AppsFlyer.registerUninstall(token);
        }

#endif
#endif
    }
}
#endif