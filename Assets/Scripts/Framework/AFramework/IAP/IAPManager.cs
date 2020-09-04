﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AFramework;
#if UNITY_PURCHASING
using UnityEngine.Purchasing;
#endif

namespace AFramework.IAP
{
    public class IAPManager : ManualSingletonMono<IAPManager>
    {
        public static System.Action EventOnIAPPackRefreshed;
        public static bool TamperedStore { get; protected set; }
        public static void FlagAsTampered() { TamperedStore = true; }

        public delegate void CallbackDelegate(bool b1, string s1);

        [SerializeField] protected FrameworkGlobalConfig _Config;
        public virtual IAPPackages PackageConfig { get { return _Config.IAPConfig; } }
        [SerializeField] eBillingSystem BillingSystemType;
        [SerializeField] protected bool _ShowSimpleLoadingScreen = false;

        IAPBaseWrapper mBillingSystem;
        CallbackDelegate mCallback;
        protected string mPurchaseLocation = "launch";

        protected Dictionary<string, string> mPackageIdToPackageNameMapping = new Dictionary<string, string>();
        protected Dictionary<string, string> mPackageIdToGroupPackageNameMapping = new Dictionary<string, string>();
               
        protected Dictionary<string, PackageInfo> mDefaultPackages = new Dictionary<string, PackageInfo>();//unique PackageName
        //public Dictionary<string, PackageInfo> DefaultPackages { get { return mDefaultPackages; } }

        protected Dictionary<string, PackageInfo> mCurrentPackages = new Dictionary<string, PackageInfo>();//unique PackageName
        //public Dictionary<string, PackageInfo> CurrentPackages { get { return mCurrentPackages; } }

        protected Dictionary<string, ActivePackageInfo> mActivePackages = new Dictionary<string, ActivePackageInfo>();//grouped PackageName
        public Dictionary<string, ActivePackageInfo> ActivePackages { get { return mActivePackages; } }

        string mLastPackId = "unknown";
        public string LastIAPPack { get { return mLastPackId; } }

#if UNITY_PURCHASING
        protected Dictionary<string, SubscriptionManager> mSubscriptionDict = new Dictionary<string, SubscriptionManager>();
#endif

        public bool IsBoughtPackage(string productID)
        {
            if (mBillingSystem == null) return false;

            return mBillingSystem.IsBoughtPackage(productID);
        }

        protected override void Awake()
        {
            TamperedStore = false;
            switch (BillingSystemType)
            {
                case eBillingSystem.SimpleIAPSystem:
                    mBillingSystem = this.gameObject.AddComponent<SimpleIAPSystemWrapper>();
                    break;
                case eBillingSystem.SDKBOXIAP:
                    mBillingSystem = this.gameObject.AddComponent<SDKBOXIAPWrapper>();
                    break;
                default:
                    Debug.Log("IAPManager invalid iap system ");
                    break;
            }

            int packageSplitMinNum = 999;
            int packageSplitMaxNum = -1;
            for (int i = 0; i < PackageConfig.CurrentData.Length; ++i)
            {
                var data = PackageConfig.CurrentData[i];
#if UNITY_EDITOR
                if (mDefaultPackages.ContainsKey(data.PackageName))
                {
                    Debug.LogError("Duplicate package name in iap config");
                }
#endif
                data.DisplayPrice = string.Format("{0} {1}", data.Price, data.Currency);
                mDefaultPackages[data.PackageName] = data.Clone();
                mCurrentPackages[data.PackageName] = data.Clone();
                mPackageIdToPackageNameMapping[data.PackageIdentifier.getString()] = data.PackageName;
                int splitNum = data.PackageIdentifier.getString().Split('.').Length;
                packageSplitMinNum = Mathf.Min(splitNum, packageSplitMinNum);
                packageSplitMaxNum = Mathf.Max(splitNum, packageSplitMaxNum);
            }

            int packageSplitNum = packageSplitMinNum != packageSplitMaxNum ? packageSplitMaxNum : packageSplitMaxNum + 1;
            Dictionary<string, List<PackageInfo>> filterList = new Dictionary<string, List<PackageInfo>>();
            List<string> removePairs = new List<string>();
            for (int i = 0; i < PackageConfig.CurrentData.Length; ++i)
            {
                var data = PackageConfig.CurrentData[i];
                var packageIdentifierSplit = data.PackageIdentifier.getString().Split('.');
                string defaultPackIdentifier = packageIdentifierSplit.Length < packageSplitNum ? data.PackageIdentifier.getString() : string.Format("{0}.{1}.{2}.{3}", packageIdentifierSplit[0], packageIdentifierSplit[1], packageIdentifierSplit[2], packageIdentifierSplit[3]);
                if (!filterList.ContainsKey(defaultPackIdentifier))
                {
                    filterList[defaultPackIdentifier] = new List<PackageInfo>();
                }
                filterList[defaultPackIdentifier].Add(mCurrentPackages[data.PackageName]);
                mPackageIdToGroupPackageNameMapping[data.PackageIdentifier.getString()] = mPackageIdToPackageNameMapping[defaultPackIdentifier];
            }
            foreach (var pair in filterList)
            {
                var newActivePackageInfo = new ActivePackageInfo();
                newActivePackageInfo.SetPackageList(pair.Key, pair.Value.ToArray());
                mActivePackages[mPackageIdToPackageNameMapping[pair.Key]] = newActivePackageInfo;
            }

            base.Awake();
            var create = UnityMainThreadDispatcher.instance;
        }

        private void Start()
        {
            if (mBillingSystem != null)
            {
                mBillingSystem.Init(new object[] { this });
                IAPBaseWrapper.EventOnPurchaseSucceeded += OnPurchaseSucceededEvent_AppUIThread;
                IAPBaseWrapper.EventOnPurchaseFailed += OnPurchaseFailedEvent;
                IAPBaseWrapper.EventOnIAPPackRefreshed += OnIAPPackRefreshed;
            }
        }

        public void UpdatePrice(string identifier, double newPrice, string newCurrency)
        {
            var packInfo = PackageIdentifierToPackageInfo(identifier);
            if (packInfo == null) return;
            packInfo.Price = newPrice;
            packInfo.Currency = newCurrency;
            packInfo.DisplayPrice = string.Format("{0} {1}", newPrice, newCurrency);
        }

        public virtual void PurchaseItem(string packageName, CallbackDelegate callback, string purchaseLocation)
        {
            mCallback = callback;
            mPurchaseLocation = purchaseLocation;
#if TEST_IAP
            OnPurchaseSucceededEvent(ActivePackages[packageName].CurrentActivePackage.PackageIdentifier.getString(), null, null);
#else
            mBillingSystem.PurchaseItem(ActivePackages[packageName].CurrentActivePackage.PackageIdentifier.getString());
            AFramework.Analytics.TrackingManager.I.TrackIAPClick(ActivePackages[packageName].CurrentActivePackage.PackageIdentifier.getString(), purchaseLocation);
            if (_ShowSimpleLoadingScreen)
            {
                AFramework.UI.CanvasManager.ShowSystemLoadingPopup(true);
                StartCoroutine(CRAutohideLoading());
            }
#endif
        }

        protected virtual void OnIAPPackRefreshed()
        {
            try
            {
#if UNITY_PURCHASING
                //currently only support SimpleIAP
                if (!(mBillingSystem is SimpleIAPSystemWrapper)) return;

#if UNITY_IOS && SIS_IAP
#if DEVELOPMENT_BUILD
            {
                IAppleExtensions extensions = SIS.IAPManager.extensions.GetExtension<IAppleExtensions>();
                extensions.simulateAskToBuy = true;
            }
#endif
            if (mIntroductoryInfoDict == null)
            {
                //only need for ios, on android it is support to be null
                mIntroductoryInfoDict = SIS.IAPManager.extensions.GetExtension<IAppleExtensions>().GetIntroductoryPriceDictionary();
            }
#endif

                foreach (var pair in mCurrentPackages)
                {
                    var currentPack = pair.Value;
                    if (currentPack.Type != eProductType.Subscription) continue;
                    var packIdentifier = currentPack.PackageIdentifier.getString();
                    if (mSubscriptionDict.ContainsKey(packIdentifier)) continue;
                    var item = (UnityEngine.Purchasing.Product)mBillingSystem.GetProductInfo(packIdentifier);
                    if (!item.hasReceipt || item.receipt == null) continue;
                    if (!checkIfProductIsAvailableForSubscriptionManager(item.receipt)) continue;
                    string intro_json = (mIntroductoryInfoDict == null || !mIntroductoryInfoDict.ContainsKey(item.definition.storeSpecificId)) ? null : mIntroductoryInfoDict[item.definition.storeSpecificId];
                    SubscriptionManager p = new SubscriptionManager(item, intro_json);
                    mSubscriptionDict[packIdentifier] = p;
                }
#endif
            }
            catch (System.Exception e) { }
            if ((object)EventOnIAPPackRefreshed != null) EventOnIAPPackRefreshed();
        }

        protected virtual void OnPurchaseSucceededEvent_AppUIThread(string packageId, string receipt, object validated)
        {
            UnityMainThreadDispatcher.instance.Enqueue(CROnPurchaseSucceededEvent(packageId, receipt, validated));
        }

        private IEnumerator CROnPurchaseSucceededEvent(string packageId, string receipt, object validated)
        {
            yield return null;
            OnPurchaseSucceededEvent(packageId, receipt, validated);
        }

        protected virtual void OnPurchaseSucceededEvent(string packageId, string receipt, object validated)
        {
            if (_ShowSimpleLoadingScreen) AFramework.UI.CanvasManager.ShowSystemLoadingPopup(false);
            if (mPurchaseLocation == null) mPurchaseLocation = "unknown";
            var pack = PackageIdentifierToPackageInfo(packageId);
            mLastPackId = "unknown";
            if (pack != null)
            {
                mLastPackId = packageId;
                var rewards = pack.Rewards;
                for (int i = 0; i < rewards.Count; ++i)
                {
                    HandleReward(rewards[i].Name, rewards[i].Amount);
                }
                SaveGameManager.instance.Save();
                if (mCallback != null)
                {
                    mCallback(true, mPackageIdToGroupPackageNameMapping[packageId]);
                    mCallback = null;
                }

                if (CanTrackIAP())
                {
                    TrackPurchase(packageId, receipt, validated, mPurchaseLocation);
                }
            }
            else
            {
                if (mCallback != null)
                {
                    mCallback(false, "unknow package id");
                    mCallback = null;
                }
            }
            OnIAPPackRefreshed();
        }

        protected bool CanTrackIAP()
        {
            return !(mPurchaseLocation == null || mPurchaseLocation == "unknown" || mPurchaseLocation == "restore" || mPurchaseLocation == "launch");
        }

        protected virtual void TrackPurchase(string packageId, string receipt, object validated, string location)
        {
            AFramework.Analytics.TrackingManager.instance.TrackIAPPurchase(packageId, receipt, validated, location);
        }

        protected virtual void OnPurchaseFailedEvent(string message)
        {
            UnityMainThreadDispatcher.instance.Enqueue(CROnPurchaseFailedEvent(message));
        }

        IEnumerator CROnPurchaseFailedEvent(string message)
        {
            yield return null;
            if (_ShowSimpleLoadingScreen) AFramework.UI.CanvasManager.ShowSystemLoadingPopup(false);
            Debug.LogError("Error " + message);
            if (mCallback != null)
            {
                mCallback(false, message);
                mCallback = null;
            }
        }

        protected virtual void HandleReward(string rewardName, int rewardAmount)
        {
            Debug.Log("Need to override");
        }

        public virtual void SetActivePackages(string[] newList)
        {
            for (int i = 0; i < newList.Length; ++i)
            {
                foreach (var pair in mActivePackages)
                {
                    if (newList[i].Contains(pair.Key))
                    {
                        pair.Value.SetActivePackage(newList[i]);
                        break;
                    }
                }
            }

            if ((object)EventOnIAPPackRefreshed != null) EventOnIAPPackRefreshed();
        }

        public PackageInfo PackageIdentifierToPackageInfo(string id, bool defaultPackage = false)
        {
            if (!mPackageIdToPackageNameMapping.ContainsKey(id)) return null;
            var allPackages = defaultPackage ? mDefaultPackages : mCurrentPackages;
            return allPackages[mPackageIdToPackageNameMapping[id]];
        }

        public ActivePackageInfo PackageIdentifierToActivePackage(string id)
        {
            if (!mPackageIdToGroupPackageNameMapping.ContainsKey(id)) return null;
            return mActivePackages[mPackageIdToGroupPackageNameMapping[id]];
        }

        public bool RestorePurchase()
        {
            if (mBillingSystem == null) return false;
            foreach (var pair in mCurrentPackages)
            {
                if (mBillingSystem.IsBoughtPackage(pair.Value.PackageIdentifier.getString()))
                {
                    var rewards = pair.Value.Rewards;
                    for (int i = 0; i < rewards.Count; ++i)
                    {
                        RestoreReward(rewards[i].Name, rewards[i].Amount);
                    }
                }
            }
#if SIS_IAP
            mPurchaseLocation = "restore";
            mBillingSystem.PurchaseItem("restore");
#endif
            return true;
        }

        void RestoreReward(string rewardName, int rewardAmount)
        {
            if (!CanRestoreReward(rewardName)) return;
            HandleReward(rewardName, rewardAmount);
        }

        protected virtual bool CanRestoreReward(string rewardName) { return false; }

        IEnumerator CRAutohideLoading()
        {
            yield return new WaitForSeconds(7);
            if (_ShowSimpleLoadingScreen) AFramework.UI.CanvasManager.ShowSystemLoadingPopup(false);
        }

#if UNITY_PURCHASING
        Dictionary<string, string> mIntroductoryInfoDict;
        void Test()
        {
            if (mIntroductoryInfoDict == null)
            {
                //only need for ios, on android it is support to be null
#if UNITY_IOS && SIS_IAP
                mIntroductoryInfoDict = SIS.IAPManager.extensions.GetExtension<IAppleExtensions>().GetIntroductoryPriceDictionary();
//#elif UNITY_ANDROID
//                mIntroductoryInfoDict = SIS.IAPManager.extensions.GetExtension<IGooglePlayStoreExtensions>().GetProductJSONDictionary();
#endif
            }

            var item = (UnityEngine.Purchasing.Product)mBillingSystem.GetProductInfo("aaa");
            if (!item.hasReceipt || item.receipt == null) return;
            if (!checkIfProductIsAvailableForSubscriptionManager(item.receipt)) return;
            string intro_json = (mIntroductoryInfoDict == null || !mIntroductoryInfoDict.ContainsKey(item.definition.storeSpecificId)) ? null : mIntroductoryInfoDict[item.definition.storeSpecificId];
            SubscriptionManager p = new SubscriptionManager(item, intro_json);
            SubscriptionInfo info = p.getSubscriptionInfo();
            Debug.Log("product id is: " + info.getProductId());
            Debug.Log("purchase date is: " + info.getPurchaseDate());
            Debug.Log("subscription next billing date is: " + info.getExpireDate());
            Debug.Log("is subscribed? " + info.isSubscribed().ToString());
            Debug.Log("is expired? " + info.isExpired().ToString());
            Debug.Log("is cancelled? " + info.isCancelled());
            Debug.Log("product is in free trial peroid? " + info.isFreeTrial());
            Debug.Log("product is auto renewing? " + info.isAutoRenewing());
            Debug.Log("subscription remaining valid time until next billing date is: " + info.getRemainingTime());
            Debug.Log("is this product in introductory price period? " + info.isIntroductoryPricePeriod());
            Debug.Log("the product introductory localized price is: " + info.getIntroductoryPrice());
            Debug.Log("the product introductory price period is: " + info.getIntroductoryPricePeriod());
            Debug.Log("the number of product introductory price period cycles is: " + info.getIntroductoryPricePeriodCycles());
        }

        private bool checkIfProductIsAvailableForSubscriptionManager(string receipt)
        {
            var receipt_wrapper = (Dictionary<string, object>)MiniJson.JsonDecode(receipt);
            if (!receipt_wrapper.ContainsKey("Store") || !receipt_wrapper.ContainsKey("Payload"))
            {
                Debug.Log("The product receipt does not contain enough information");
                return false;
            }
            var store = (string)receipt_wrapper["Store"];
            var payload = (string)receipt_wrapper["Payload"];

            if (payload != null)
            {
                switch (store)
                {
                    case GooglePlay.Name:
                        {
                            var payload_wrapper = (Dictionary<string, object>)MiniJson.JsonDecode(payload);
                            if (!payload_wrapper.ContainsKey("json"))
                            {
                                Debug.Log("The product receipt does not contain enough information, the 'json' field is missing");
                                return false;
                            }
                            var original_json_payload_wrapper = (Dictionary<string, object>)MiniJson.JsonDecode((string)payload_wrapper["json"]);
                            if (original_json_payload_wrapper == null || !original_json_payload_wrapper.ContainsKey("developerPayload"))
                            {
                                Debug.Log("The product receipt does not contain enough information, the 'developerPayload' field is missing");
                                return false;
                            }
                            var developerPayloadJSON = (string)original_json_payload_wrapper["developerPayload"];
                            var developerPayload_wrapper = (Dictionary<string, object>)MiniJson.JsonDecode(developerPayloadJSON);
                            if (developerPayload_wrapper == null || !developerPayload_wrapper.ContainsKey("is_free_trial") || !developerPayload_wrapper.ContainsKey("has_introductory_price_trial"))
                            {
                                Debug.Log("The product receipt does not contain enough information, the product is not purchased using 1.19 or later");
                                return false;
                            }
                            return true;
                        }
                    case AppleAppStore.Name:
                    case AmazonApps.Name:
                    case MacAppStore.Name:
                        {
                            return true;
                        }
                    default:
                        {
                            return false;
                        }
                }
            }
            return false;
        }
#endif

#if UNITY_EDITOR
        void Reset()
        {
            _Config = FrameworkGlobalConfig.Instance;
        }
#endif
    }
}

