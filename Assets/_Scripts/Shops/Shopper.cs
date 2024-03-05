using System;
using UnityEngine;

namespace RPG.Shops
{
    public class Shopper : MonoBehaviour
    {
        public event Action OnActiveShopChanged;

        private Shop activeShop;

        #region Properties
        public Shop ActiveShop
        {
            get => activeShop;
        }
        #endregion

        /// <summary>
        /// Need to debug
        /// </summary>
        /// <param name="shop"></param>
        public void SetActiveShop(Shop shop)
        {
            if (activeShop != null)
            {
                activeShop.SetShopper(null);
            }

            activeShop = shop;

            if (activeShop != null)
            {
                activeShop.SetShopper(this);
            }

            OnActiveShopChanged?.Invoke();
        }
    }
}