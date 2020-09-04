using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if USE_FIREBASE
namespace AFramework.FirebaseService
{
    public static class FirebaseInstance
    {
        static Firebase.FirebaseApp sInstance = null;
        public static bool HasInstance { get { return sInstance != null; } }

        static System.Action EventOnInitSuccess;        
        static bool sIsInitializing = false;

        public static void ChecAndTryInit(System.Action callback)
        {
            if (HasInstance)
            {
                callback();
            }
            else if (sIsInitializing)
            {
                EventOnInitSuccess += callback;
            }
            else
            {
                sIsInitializing = true;
                EventOnInitSuccess += callback;
                Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
                {
                    var dependencyStatus = task.Result;
                    if (dependencyStatus == Firebase.DependencyStatus.Available)
                    {
                        sInstance = Firebase.FirebaseApp.DefaultInstance;
                        if ((object)EventOnInitSuccess != null)
                        {
                            EventOnInitSuccess();
                            EventOnInitSuccess = null;
                        }
                    }
                    else
                    {
                        UnityEngine.Debug.LogError(System.String.Format(
                          "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                        // Firebase Unity SDK is not safe to use here.
                    }
                    sIsInitializing = false;
                });
            }
        }

        public static IEnumerator CRGetAppToken()
        {
            System.Threading.Tasks.Task<string> t = Firebase.InstanceId.FirebaseInstanceId.DefaultInstance.GetTokenAsync();
            while (!t.IsCompleted) yield return new WaitForEndOfFrame();
            //Debug.Log(" FirebaseID is " + t.Result);
        }
    }
}
#endif
