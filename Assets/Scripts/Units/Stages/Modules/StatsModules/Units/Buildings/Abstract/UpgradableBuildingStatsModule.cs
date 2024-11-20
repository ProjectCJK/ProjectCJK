

using System;
using System.Collections.Generic;
using System.Linq;
using Managers;
using ScriptableObjects.Scripts.Buildings.Abstract;
using TMPro;
using UI.BuildingEnhancementPanel.UI;
using Units.Stages.Modules.StatsModules.Abstract;
using Units.Stages.Units.Buildings.Enums;
using Units.Stages.Units.Items.Enums;
using UnityEngine;

namespace Units.Stages.Modules.StatsModules.Units.Buildings.Abstract
{
    public interface IBuildingStatsModule
    {
    }

    public abstract class UpgradableBuildingStatsModule : BuildingStatsModule, IBuildingStatsModule
    {
        public abstract string[,] BuildingData { get; }
        public abstract string[,] BuildingOption1ValueData { get; }
        public abstract string[,] BuildingOption2ValueData { get; }
        public abstract string[,] BuildingOption1CostData { get; }
        public abstract string[,] BuildingOption2CostData { get; }
        
        public float CurrentBuildingOption1Value;
        public float CurrentBuildingOption2Value;
        public int MaxBuildingOption1Level;
        public int MaxBuildingOption2Level;
        public float NextBuildingOption1Value;
        public float NextBuildingOption2Value;
        public int RequiredGoldToUpgradeBuildingOption1;
        public int RequiredGoldToUpgradeBuildingOption2;
        public int RequiredBuildingLevelToUpgradeOption2Level;
        public int RequiredBuildingOption1LevelToUpgradeBuildingLevel;

        public string PanelTitle;
        public Sprite Slot1Icon;
        public Sprite Slot2Icon;
        public Sprite Slot3Icon;
        public string Slot1Title;
        public string Slot2Title;
        public string Slot3Title;
        public string Slot1Category1Title;
        public string Slot1Category2Title;
        public int Option1TextIconIndex;
        public int Option2TextIconIndex;
        public int RequiredGoldForUpgradeOption1IconIndex;
        public int RequiredRedGemForUpgradeOption2IconIndex;
        public int RequiredBuildingLevelToUpgradeOption2LevelIndex;
        
        protected UpgradableBuildingStatsModule(BuildingDataSO buildingDataSo) : base(buildingDataSo) { }
        
        protected void UpdateDefaultValue()
        {
            var buildingUpgradePanelData = DataManager.Instance.BuildingUpgradePanel.GetData();
            List<string> currentBuildingData = Enumerable.Range(0, buildingUpgradePanelData.GetLength(0))
                .Where(i => 
                    buildingUpgradePanelData[i, 1] == $"{BuildingKey}")
                .Select(i =>
                    Enumerable.Range(0, buildingUpgradePanelData.GetLength(1))
                        .Select(j => buildingUpgradePanelData[i, j]).ToList())
                .ToList().FirstOrDefault();

            if (currentBuildingData is { Count: > 0 })
            {
                PanelTitle = currentBuildingData[2];
                Slot1Title = currentBuildingData[3];
                Slot2Title = currentBuildingData[7];
                Slot3Title = currentBuildingData[9];
                Slot1Icon = DataManager.Instance.SpriteDatas[int.Parse(currentBuildingData[4])];
                Slot2Icon = DataManager.Instance.SpriteDatas[int.Parse(currentBuildingData[8])];
                Slot3Icon = DataManager.Instance.SpriteDatas[int.Parse(currentBuildingData[10])];
                Option1TextIconIndex = int.Parse(currentBuildingData[11]);
                Option2TextIconIndex = int.Parse(currentBuildingData[12]);
                RequiredGoldForUpgradeOption1IconIndex = int.Parse(currentBuildingData[15]);
                RequiredRedGemForUpgradeOption2IconIndex = int.Parse(currentBuildingData[16]);
                RequiredBuildingLevelToUpgradeOption2LevelIndex = int.Parse(currentBuildingData[17]);
                Slot1Category1Title = currentBuildingData[13];
                Slot1Category2Title = currentBuildingData[14];
            }
        }

        protected void UpdateBuildingStatsModule()
        {
            CurrentBuildingOption1Value = Enumerable.Range(0, BuildingOption1ValueData.GetLength(0))
                .Where(i =>
                    BuildingOption1ValueData[i, 1] == $"{BuildingKey}" &&
                    BuildingOption1ValueData[i, 2] == GameManager.Instance.ES3Saver.CurrentBuildingLevel[BuildingKey].ToString() &&
                    BuildingOption1ValueData[i, 3] == GameManager.Instance.ES3Saver.CurrentBuildingOption1Level[BuildingKey].ToString())
                .Select(i => ParserModule.ParseOrDefault(BuildingOption1ValueData[i, 4], CurrentBuildingOption1Value))
                .FirstOrDefault();

            CurrentBuildingOption2Value = Enumerable.Range(0, BuildingOption2ValueData.GetLength(0))
                .Where(i =>
                    BuildingOption2ValueData[i, 1] == $"{BuildingKey}" &&
                    BuildingOption2ValueData[i, 2] == GameManager.Instance.ES3Saver.CurrentBuildingLevel[BuildingKey].ToString() &&
                    BuildingOption2ValueData[i, 3] == GameManager.Instance.ES3Saver.CurrentBuildingOption2Level[BuildingKey].ToString())
                .Select(i => ParserModule.ParseOrDefault(BuildingOption2ValueData[i, 4], CurrentBuildingOption2Value))
                .FirstOrDefault();

            NextBuildingOption1Value = Enumerable.Range(0, BuildingOption1ValueData.GetLength(0))
                .Where(i =>
                    BuildingOption1ValueData[i, 1] == $"{BuildingKey}" &&
                    BuildingOption1ValueData[i, 2] == GameManager.Instance.ES3Saver.CurrentBuildingLevel[BuildingKey].ToString() &&
                    BuildingOption1ValueData[i, 3] == (GameManager.Instance.ES3Saver.CurrentBuildingOption1Level[BuildingKey] + 1).ToString())
                .Select(i => ParserModule.ParseOrDefault(BuildingOption1ValueData[i, 4], NextBuildingOption1Value))
                .FirstOrDefault();
            
            NextBuildingOption1Value = GameManager.Instance.ES3Saver.CurrentBuildingOption1Level[BuildingKey] != 1 && NextBuildingOption1Value == 0 ? CurrentBuildingOption1Value : NextBuildingOption1Value;

            NextBuildingOption2Value = Enumerable.Range(0, BuildingOption2ValueData.GetLength(0))
                .Where(i =>
                    BuildingOption2ValueData[i, 1] == $"{BuildingKey}" &&
                    BuildingOption2ValueData[i, 2] == GameManager.Instance.ES3Saver.CurrentBuildingLevel[BuildingKey].ToString() &&
                    BuildingOption2ValueData[i, 3] == (GameManager.Instance.ES3Saver.CurrentBuildingOption2Level[BuildingKey] + 1).ToString())
                .Select(i => ParserModule.ParseOrDefault(BuildingOption2ValueData[i, 4], NextBuildingOption2Value))
                .FirstOrDefault();
            
            NextBuildingOption2Value = GameManager.Instance.ES3Saver.CurrentBuildingOption2Level[BuildingKey] != 1 && NextBuildingOption2Value == 0 ? CurrentBuildingOption2Value : NextBuildingOption2Value;

            RequiredGoldToUpgradeBuildingOption1 = Enumerable.Range(0, BuildingOption1CostData.GetLength(0))
                .Where(i =>
                    BuildingOption1CostData[i, 1] == $"{BuildingKey}" &&
                    BuildingOption1CostData[i, 2] == GameManager.Instance.ES3Saver.CurrentBuildingLevel[BuildingKey].ToString() &&
                    BuildingOption1CostData[i, 3] == GameManager.Instance.ES3Saver.CurrentBuildingOption1Level[BuildingKey].ToString())
                .Select(i =>
                    ParserModule.ParseOrDefault(BuildingOption1CostData[i, 4], RequiredGoldToUpgradeBuildingOption1))
                .FirstOrDefault();

            RequiredGoldToUpgradeBuildingOption2 = Enumerable.Range(0, BuildingOption2CostData.GetLength(0))
                .Where(i =>
                    BuildingOption2CostData[i, 1] == $"{BuildingKey}" &&
                    BuildingOption2CostData[i, 2] == GameManager.Instance.ES3Saver.CurrentBuildingLevel[BuildingKey].ToString() &&
                    BuildingOption2CostData[i, 3] == GameManager.Instance.ES3Saver.CurrentBuildingOption2Level[BuildingKey].ToString())
                .Select(i =>
                    ParserModule.ParseOrDefault(BuildingOption2CostData[i, 4], RequiredGoldToUpgradeBuildingOption2))
                .FirstOrDefault();

            RequiredBuildingOption1LevelToUpgradeBuildingLevel = Enumerable.Range(0, BuildingData.GetLength(0))
                .Where(i =>
                    BuildingData[i, 1] == $"{BuildingKey}" &&
                    BuildingData[i, 2] == GameManager.Instance.ES3Saver.CurrentBuildingLevel[BuildingKey].ToString() &&
                    BuildingData[i, 5] == GameManager.Instance.ES3Saver.CurrentBuildingLevel[BuildingKey].ToString())
                .Select(i =>
                    ParserModule.ParseOrDefault(BuildingData[i, 6], RequiredBuildingOption1LevelToUpgradeBuildingLevel))
                .FirstOrDefault();
            
            RequiredBuildingLevelToUpgradeOption2Level = Enumerable.Range(0, BuildingOption2CostData.GetLength(0))
            .Where(i =>
                BuildingOption2CostData[i, 1] == $"{BuildingKey}" &&
                BuildingOption2CostData[i, 2] == GameManager.Instance.ES3Saver.CurrentBuildingLevel[BuildingKey].ToString() &&
                BuildingOption2CostData[i, 3] == GameManager.Instance.ES3Saver.CurrentBuildingOption2Level[BuildingKey].ToString())
            .Select(i =>
                ParserModule.ParseOrDefault(BuildingOption2CostData[i, 5], RequiredBuildingLevelToUpgradeOption2Level))
            .FirstOrDefault();

            MaxBuildingOption1Level = Enumerable.Range(0, BuildingData.GetLength(0))
                .Where(i =>
                    BuildingData[i, 1] == $"{BuildingKey}" &&
                    BuildingData[i, 2] == GameManager.Instance.ES3Saver.CurrentBuildingLevel[BuildingKey].ToString())
                .Select(i => ParserModule.ParseOrDefault(BuildingData[i, 3], MaxBuildingOption1Level))
                .FirstOrDefault();

            MaxBuildingOption2Level = Enumerable.Range(0, BuildingData.GetLength(0))
                .Where(i =>
                    BuildingData[i, 1] == $"{BuildingKey}" &&
                    BuildingData[i, 2] == GameManager.Instance.ES3Saver.CurrentBuildingLevel[BuildingKey].ToString())
                .Select(i => ParserModule.ParseOrDefault(BuildingData[i, 4], MaxBuildingOption1Level))
                .FirstOrDefault();


            UpdateValue();
        }
        
        public void GetUIBuildingEnhancement()
        {
            var uiBuildingEnhancementData = new UIBuildingEnhancementData
            {
                PanelTitle = PanelTitle,
                Slot1Icon = Slot1Icon,
                Slot2Icon = Slot2Icon,
                Slot3Icon = Slot3Icon,
                Slot1Title = Slot1Title,
                Slot2Title = Slot2Title,
                Slot3Title = Slot3Title,
                Slot1Category1Title = Slot1Category1Title,
                Slot1Category2Title = Slot1Category2Title,
                Option1TextIconIndex = Option1TextIconIndex,
                Option2TextIconIndex = Option2TextIconIndex,
                RequiredGoldForUpgradeOption1IconIndex = RequiredGoldForUpgradeOption1IconIndex,
                RequiredRedGemForUpgradeOption2IconIndex = RequiredRedGemForUpgradeOption2IconIndex,
                RequiredBuildingLevelToUpgradeOption2LevelIndex = RequiredBuildingLevelToUpgradeOption2LevelIndex,
                CurrentBuildingOption1Value = CurrentBuildingOption1Value,
                CurrentBuildingOption2Value = CurrentBuildingOption2Value,
                NextBuildingOption1Value = NextBuildingOption1Value,
                NextBuildingOption2Value = NextBuildingOption2Value,
                CurrentBuildingLevel = GameManager.Instance.ES3Saver.CurrentBuildingLevel[BuildingKey],
                CurrentBuildingOption1Level = GameManager.Instance.ES3Saver.CurrentBuildingOption1Level[BuildingKey],
                CurrentBuildingOption2Level = GameManager.Instance.ES3Saver.CurrentBuildingOption2Level[BuildingKey],
                MaxBuildingOption1Level = MaxBuildingOption1Level,
                MaxBuildingOption2Level = MaxBuildingOption2Level,
                RequiredGoldToUpgradeOption1Level = RequiredGoldToUpgradeBuildingOption1,
                RequiredRedGemToUpgradeOption2Level = RequiredGoldToUpgradeBuildingOption2,
                RequiredBuildingLevelToUpgradeOption2Level = RequiredBuildingLevelToUpgradeOption2Level,
                OnClickUpgradeButtonForBuildingOption1 = OnClickUpgradeButtonForBuildingOption1,
                OnClickUpgradeButtonForBuildingOption2 = OnClickUpgradeButtonForBuildingOption2
            };
    
            UIManager.Instance.UI_Panel_BuildingEnhancement.Activate(uiBuildingEnhancementData);
        }

        public void ReturnUIBuildingEnhancement()
        {
            UIManager.Instance.UI_Panel_BuildingEnhancement.Inactivate();
        }

        protected abstract void UpdateValue();
        
        private void OnClickUpgradeButtonForBuildingOption1()
        {
            if (RequiredGoldToUpgradeBuildingOption1 <= CurrencyManager.Instance.Gold)
            {
                CurrencyManager.Instance.RemoveCurrency(ECurrencyType.Gold, RequiredGoldToUpgradeBuildingOption1);
                IncreaseCurrentBuildingOption1Level();
            }
        }

        private void OnClickUpgradeButtonForBuildingOption2()
        {
            if (RequiredGoldToUpgradeBuildingOption2 <= CurrencyManager.Instance.Gold)
            {
                CurrencyManager.Instance.RemoveCurrency(ECurrencyType.Gold, RequiredGoldToUpgradeBuildingOption2);
                IncreaseCurrentBuildingOption2Level();
            }
        }

        private void IncreaseCurrentBuildingOption1Level()
        {
            GameManager.Instance.ES3Saver.CurrentBuildingOption1Level[BuildingKey]++;

            if (GameManager.Instance.ES3Saver.CurrentBuildingOption1Level[BuildingKey] >= RequiredBuildingOption1LevelToUpgradeBuildingLevel) GameManager.Instance.ES3Saver.CurrentBuildingLevel[BuildingKey]++;

            UpdateBuildingStatsModule();
            GetUIBuildingEnhancement();

            OnTriggerBuildingAnimation?.Invoke(EBuildingAnimatorParameter.Upgrade_Coin);
            QuestManager.Instance.OnUpdateCurrentQuestProgress?.Invoke(EQuestType1.LevelUpOption1, BuildingKey, 1);
        }

        private void IncreaseCurrentBuildingOption2Level()
        {
            GameManager.Instance.ES3Saver.CurrentBuildingOption2Level[BuildingKey]++;

            UpdateBuildingStatsModule();
            GetUIBuildingEnhancement();
            
            OnTriggerBuildingAnimation?.Invoke(EBuildingAnimatorParameter.Upgrade_Cookie);
        }
    }
}