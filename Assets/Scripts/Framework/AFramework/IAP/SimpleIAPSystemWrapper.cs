using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if SIS_IAP
using SIS;
#endif

namespace AFramework.IAP
{
    public class SimpleIAPSystemWrapper : IAPBaseWrapper
    {
#if SIS_IAP
        SIS.IAPManager mSystem;

        public override void Init(object[] inputs)
        {
            SIS.IAPManager.purchaseSucceededEvent += PurchaseSucceeded;
            SIS.IAPManager.purchaseFailedEvent += PurchaseFailed;
            mManager = (AFramework.IAP.IAPManager)inputs[0];
            IAPGroup defaultGroup = new IAPGroup();
            defaultGroup.id = defaultGroup.name = "default";
            var currentPackage = mManager.PackageConfig;
            for (int i = 0; i < currentPackage.CurrentData.Length; ++i)
            {
                var data = currentPackage.CurrentData[i];
                var item = new IAPObject();
                item.id = data.PackageIdentifier.getString();
                defaultGroup.items.Add(item);
            }

            mSystem = this.GetComponentInChildren<SIS.IAPManager>();
            var prefab = Resources.Load<SIS.IAPManager>("IAPManager");
            prefab.IAPs.Clear();
            prefab.IAPs.Add(defaultGroup);
            mSystem = Instantiate(prefab, this.transform);
#if UNITY_ANDROID
            mSystem.storeKeys.googleKey = System.Convert.ToBase64String(UnityEngine.Purchasing.Security.GooglePlayTangle.Data());
#endif
            StartCoroutine(CRWaitForProductUpdate());
            mSystem.Initialize();

#if UNITY_EDITOR
            SIS.IAPManager.isDebug = true;
            var validator = mSystem.GetComponent<ReceiptValidator>();
            if (validator != null) Destroy(validator);
            else Debug.LogError("IAP does not have any receipt validator");
#endif
        }

        public override void PurchaseItem(string packageName)
        {
            SIS.IAPManager.PurchaseProduct(packageName);
        }

        IEnumerator CRWaitForProductUpdate()
        {
            WaitForSeconds waitTime = new WaitForSeconds(1);
            while (SIS.IAPManager.controller == null) yield return waitTime;
            var allProduct = SIS.IAPManager.controller.products.all;
            for (int i = 0; i < allProduct.Length; ++i)
            {
                if (!allProduct[i].metadata.localizedPrice.Equals(null))
                {
                    double newPrice;
                    try
                    {
                        newPrice = (double)allProduct[i].metadata.localizedPrice;
                        mManager.UpdatePrice(allProduct[i].definition.id, newPrice, allProduct[i].metadata.isoCurrencyCode);
                    }
                    catch (System.Exception e)
                    {

                    }
                }
            }

            var defaultPackages = mManager.PackageConfig.CurrentData;
            var currentPackages = mManager.ActivePackages;
            int priceRound = 0;
            int priceDelta = 0;
            for (int i = 0; i < defaultPackages.Length; ++i)
            {
                var defaultPack = defaultPackages[i];
                var currentPack = mManager.PackageIdentifierToPackageInfo(defaultPack.PackageIdentifier.getString(), false);
                if (currentPack.Currency == defaultPack.Currency)//USD
                {
                    if (currentPack.Price % 1 == 0)
                    {
                        ++priceRound;
                    }
                    priceDelta += Mathf.RoundToInt((float)currentPack.Price - (float)defaultPack.Price);
                }
            }

            if (defaultPackages.Length == priceRound || (priceDelta / defaultPackages.Length) >= 8)//if Price in USD is not XX.99 but is only has XX. or Delta change with default config is too big
            {
                IAP.IAPManager.FlagAsTampered();
            }

            if (EventOnIAPPackRefreshed != null) EventOnIAPPackRefreshed();
        }

        public override bool IsBoughtPackage(string productID)
        {
            if (SIS.IAPManager.controller == null ||
                SIS.IAPManager.controller.products == null ||
                SIS.IAPManager.controller.products.WithID(productID) == null) return false;
            return SIS.IAPManager.controller.products.WithID(productID).hasReceipt;
        }

        public override object GetProductInfo(string productID)
        {
            if (SIS.IAPManager.controller == null ||
                SIS.IAPManager.controller.products == null ||
                SIS.IAPManager.controller.products.WithID(productID) == null) return null;
            return SIS.IAPManager.controller.products.WithID(productID);
        }
#endif
    }
}