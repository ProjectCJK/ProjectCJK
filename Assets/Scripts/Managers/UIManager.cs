using System;
using Modules.DesignPatterns.Singletons;
using UI;
using UnityEngine;

namespace Managers
{
    public class UIManager : SingletonMono<UIManager>
    {
        [Header("=== 건물 업그레이드 패널 ===")] [SerializeField]
        private UI_BuildingEnhancement uiBuildingEnhancement;

        public void GetPanelBuildingEnhancement(
            string buildingName,
            Sprite buildingProductSprite,
            string buildingProductName,
            float currentBuildingOption1Value,
            float currentBuildingOption2Value,
            float nextBuildingOption1Value,
            float nextBuildingOption2Value,
            int currentBuildingLevel,
            int currentBuildingOption1Level,
            int currentBuildingOption2Level,
            int maxBuildingOption1Level,
            int maxBuildingOption2Level,
            int requiredGoldToUpgradeOption1Level,
            int requiredGoldToUpgradeOption2Level,
            int requiredBuildingLevelToUpgradeOption2Level,
            Action onClickUpgradeButtonForBuildingOption1,
            Action onClickUpgradeButtonForBuildingOption2)
        {
            uiBuildingEnhancement.Activate(
                buildingName,
                buildingProductSprite,
                buildingProductName,
                currentBuildingOption1Value,
                currentBuildingOption2Value,
                nextBuildingOption1Value,
                nextBuildingOption2Value,
                currentBuildingLevel,
                currentBuildingOption1Level,
                currentBuildingOption2Level,
                maxBuildingOption1Level,
                maxBuildingOption2Level,
                requiredGoldToUpgradeOption1Level,
                requiredGoldToUpgradeOption2Level,
                requiredBuildingLevelToUpgradeOption2Level,
                onClickUpgradeButtonForBuildingOption1,
                onClickUpgradeButtonForBuildingOption2
            );
        }

        public void ReturnPanelBuildingEnhancement()
        {
            uiBuildingEnhancement.Inactivate();
        }
    }
}