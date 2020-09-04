using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AFramework.IAP
{
    public class IAPBaseWrapper : MonoBehaviour
    {
        public static System.Action EventOnIAPPackRefreshed;
        public static System.Action<string, string, object> EventOnPurchaseSucceeded;
        public static System.Action<string> EventOnPurchaseFailed;

        protected IAPManager mManager;

        public virtual void Init(object[] inputs) { }
        public virtual void PurchaseItem(string packageName) { }
        public virtual bool IsBoughtPackage(string productID) { return false; }
        public virtual object GetProductInfo(string productID) { return null; }

        protected void PurchaseSucceeded(string str, string receipt, object validated)
        {
            EventOnPurchaseSucceeded(str, receipt, validated);
        }

        protected void PurchaseFailed(string str)
        {
            EventOnPurchaseFailed(str);
        }
    }
}