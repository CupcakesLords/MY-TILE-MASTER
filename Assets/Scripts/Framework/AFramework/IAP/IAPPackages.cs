using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AFramework.IAP
{
    public enum eBillingSystem
    {
        Invalid = -1,
        SimpleIAPSystem,
        SDKBOXIAP
    }

    public enum eProductType
    {
        Consumable,
        NonConsumable,
        Subscription
    }

    [System.Serializable]
    public class RewardInfo
    {
        public string Name;
        public int Amount;
    }

    [System.Serializable]
    public class PackageInfo
    {
        public string PackageName;
        public PlatformString PackageIdentifier;
        public Sprite Icon;
        public Sprite[] CustomIcon;
        public eProductType Type;
        public string Title;
        public string Description;
        public double Price;
        public string Currency;
        public List<RewardInfo> Rewards;
        [System.NonSerialized]
        public string DisplayPrice;

        public virtual PackageInfo Clone()
        {
            var newData = new PackageInfo();
            newData.PackageName = PackageName;
            newData.PackageIdentifier = new PlatformString(PackageIdentifier);
            newData.Icon = Icon;
            newData.CustomIcon = CustomIcon;
            newData.Type = Type;
            newData.Title = Title;
            newData.Description = Description;
            newData.Price = Price;
            newData.Currency = Currency;
            newData.DisplayPrice = DisplayPrice;
            newData.Rewards = new List<RewardInfo>();
            for (int i = 0; i < Rewards.Count; ++i)
            {
                newData.Rewards.Add(Rewards[i]);
            }
            return newData;
        }
    }

    public class ActivePackageInfo
    {
        PackageInfo mCurrentActivePackage;
        public PackageInfo CurrentActivePackage { get { return mCurrentActivePackage; } }

        string mDefaultPackageIdentifier;
        public string DefaultPackageIdentifier { get { return mDefaultPackageIdentifier; } }

        Dictionary<string, PackageInfo> mPackageList;

        public void SetPackageList(string defaultPackageIdentifier, PackageInfo[] newList)
        {
            mPackageList = new Dictionary<string, PackageInfo>();
            for (int i = 0; i < newList.Length; ++i)
            {
                mPackageList[newList[i].PackageIdentifier.getString()] = newList[i];
            }
            mDefaultPackageIdentifier = defaultPackageIdentifier;
            mCurrentActivePackage = mPackageList[mDefaultPackageIdentifier];
        }

        public void SetActivePackage(string packageIdentifier)
        {
            if (mPackageList.ContainsKey(packageIdentifier))
            {
                mCurrentActivePackage = mPackageList[packageIdentifier];
            }
        }
    }

    [System.Serializable]
    [CreateAssetMenu(menuName = "ScriptableObject/AFramework/IAP/IAPPackagesInfo")]
    public class IAPPackages : ScriptableObject
    {
        [SerializeField] protected PackageInfo[] Data;
        public virtual PackageInfo[] CurrentData => Data;
    }
}
