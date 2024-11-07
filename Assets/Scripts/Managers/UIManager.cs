using Modules.DesignPatterns.Singletons;
using UI;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Managers
{
    public class UIManager : SingletonMono<UIManager>
    {
        [Header("=== 건물 업그레이드 패널 ===")]
        [SerializeField] private UI_KitchenEnhancement uiKitchenEnhancement;

        public void GetUIKitchenEnhancement(
            string textKitchen,
            Sprite imageKitchenProduct,
            string textKitchenProduct,
            string textCurrentKitchenProductPrice,
            string textCurrentKitchenProductTime,
            string textCurrentKitchenLevel,
            string textCurrentKitchenProductPriceLevel,
            string textCurrentKitchenProductPrice2,
            string textNextKitchenProductPrice,
            string textRequiredGoldToUpgradeProductPrice,
            string textCurrentKitchenProductTimeLevel,
            string textCurrentKitchenProductTime2,
            string textNextKitchenProductTime,
            string textRequiredGoldToUpgradeProductTime
        )
        {
            uiKitchenEnhancement.Initialize(
                textKitchen,
                imageKitchenProduct,
                textKitchenProduct,
                textCurrentKitchenProductPrice,
                textCurrentKitchenProductTime,
                textCurrentKitchenLevel,
                textCurrentKitchenProductPriceLevel,
                textCurrentKitchenProductPrice2,
                textNextKitchenProductPrice,
                textRequiredGoldToUpgradeProductPrice,
                textCurrentKitchenProductTimeLevel,
                textCurrentKitchenProductTime2,
                textNextKitchenProductTime,
                textRequiredGoldToUpgradeProductTime
            );
            
            uiKitchenEnhancement.gameObject.SetActive(true);
        }

        public void ReturnUIKitchenEnhancement() => uiKitchenEnhancement.gameObject.SetActive(false);
    }
}
