using System;
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
            string kitchenName,
            Sprite kitchenProductSprite,
            string kitchenProductName,
            int currentKitchenOption1Value,
            float currentKitchenOption2Value,
            int nextKitchenOption1Value,
            float nextKitchenOption2Value,
            int currentKitchenLevel,
            int currentKitchenOption1Level,
            int currentKitchenOption2Level,
            int maxKitchenOption1Level,
            int maxKitchenOption2Level,
            int requiredGoldToUpgradeOption1Level,
            int requiredGoldToUpgradeOption2Level,
            int requiredKitchenLevelToUpgradeOption2Level,
            Action onClickUpgradeButtonForKitchenOption1,
            Action onClickUpgradeButtonForKitchenOption2)
        {
         
            uiKitchenEnhancement.Activate(
                kitchenName,
                kitchenProductSprite,
                kitchenProductName,
                currentKitchenOption1Value,
                currentKitchenOption2Value,
                nextKitchenOption1Value,
                nextKitchenOption2Value,
                currentKitchenLevel,
                currentKitchenOption1Level,
                currentKitchenOption2Level,
                maxKitchenOption1Level,
                maxKitchenOption2Level,
                requiredGoldToUpgradeOption1Level,
                requiredGoldToUpgradeOption2Level,
                requiredKitchenLevelToUpgradeOption2Level,
                onClickUpgradeButtonForKitchenOption1,
                onClickUpgradeButtonForKitchenOption2
                );
        }

        public void ReturnUIKitchenEnhancement()
        {
            uiKitchenEnhancement.Inactivate();
        }
    }
}
