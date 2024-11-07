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

        public void ReturnUIKitchenEnhancement() => uiKitchenEnhancement.gameObject.SetActive(false);

        public void GetUIKitchenEnhancement(
            string kitchenName,
            Sprite kitchenProductSprite,
            string kitchenProductName,
            int currentKitchenOption1Value,
            float currentKitchenOption2Value,
            int currentKitchenLevel,
            int currentKitchenOption1Level,
            int nextKitchenOption1Value,
            int requiredGoldToUpgradeOption1Level,
            int currentKitchenOption2Level,
            float nextKitchenOption2Value,
            int requiredGoldToUpgradeOption2Level)
        {
         
            uiKitchenEnhancement.Initialize(
                kitchenName,
                kitchenProductSprite,
                kitchenProductName,
                currentKitchenOption1Value,
                currentKitchenOption2Value,
                currentKitchenLevel,
                currentKitchenOption1Level,
                nextKitchenOption1Value,
                requiredGoldToUpgradeOption1Level,
                currentKitchenOption2Level,
                nextKitchenOption2Value,
                requiredGoldToUpgradeOption2Level
                );
            uiKitchenEnhancement.gameObject.SetActive(true);
        }
    }
}
