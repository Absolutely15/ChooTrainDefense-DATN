using System.Collections.Generic;
using System.Linq;
using UnityEngine.Purchasing;

namespace NVTT
{
    public static class IAPUtilities
    {
        public static List<string> GetProductsID()
        {
            var productCatalog = ProductCatalog.LoadDefaultCatalog();

            return productCatalog.allProducts.Select(item => item.id).ToList();
        }
        public static List<string> GetProductsIDType(ProductType productType)
        {
            var productCatalog = ProductCatalog.LoadDefaultCatalog();
            return (from item in productCatalog.allProducts where item.type == productType select item.id).ToList();
        }
        
        public static void MyDebug(string text) => InAppPurchaseManager.Instance.MyDebug(text);
        public static void MyDebugWarning(string text) => InAppPurchaseManager.Instance.MyDebugWarning(text);
        public static void MyDebugError(string text) => InAppPurchaseManager.Instance.MyDebugError(text);
    }
}

