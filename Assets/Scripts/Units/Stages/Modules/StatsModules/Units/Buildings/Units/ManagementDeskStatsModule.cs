using System.Linq;
using Managers;
using ScriptableObjects.Scripts.Buildings.Units;
using Units.Stages.Modules.ProductModules.Abstract;
using Units.Stages.Modules.StatsModules.Units.Buildings.Abstract;
using Units.Stages.Units.Zones.Units.BuildingZones.Enums;
using Units.Stages.Units.Zones.Units.BuildingZones.Units;
using UnityEngine;

namespace Units.Stages.Modules.StatsModules.Units.Buildings.Units
{
    public interface IManagementDeskStatsModule : IBuildingStatsModule
    {
        
    }
    
    public class ManagementDeskStatsModule : BuildingStatsModule, IManagementDeskStatsModule
    {
        public string ManagementDeskName => "ManagementDesk";
        public Sprite ManagementDeskSprite;
        public float CurrentManagementDeskOption1Value;
        public float CurrentManagementDeskOption2Value;
        public float NextManagementDeskOption1Value;
        public float NextManagementDeskOption2Value;
        public int MaxManagementDeskOption1Level;
        public int MaxManagementDeskOption2Level;
        public int RequiredGoldToUpgradeManagementDeskOption1;
        public int RequiredGoldToUpgradeManagementDeskOption2;
        public int RequiredOption1LevelToUpgradeManagementDeskLevel;
        public int RequiredManagementDeskLevelToUpgradeOption2;
        
        // TODO : 데이터 저장해야함!
        public int CurrentManagementDeskLevel;
        public int CurrentManagementDeskOption1Level;
        public int CurrentManagementDeskOption2Level;
        
        public readonly EBuildingType BuildingType;
        public readonly string BuildingKey;
        public readonly string InputItemKey;
        public readonly string OutputItemKey;
        
        public ManagementDeskStatsModule(ManagementDeskDataSO managementDeskDataSo, ManagementDeskCustomSetting managementDeskCustomSetting) : base(managementDeskDataSo)
        {
            BuildingType = managementDeskDataSo.BuildingType;
            
            BuildingKey = ParserModule.ParseEnumToString(managementDeskDataSo.BuildingType);
            InputItemKey = ParserModule.ParseEnumToString(managementDeskCustomSetting.CurrencyType);
            OutputItemKey = ParserModule.ParseEnumToString(managementDeskCustomSetting.CurrencyType);
            
            CurrentManagementDeskLevel = 1;
            CurrentManagementDeskOption1Level = 1;
            CurrentManagementDeskOption2Level = 1;

            UpdateManagementDeskStatsModule();
        }

        private void UpdateManagementDeskStatsModule()
        {
            // ManagementDeskSprite = DataManager.Instance.ItemDataSo.ItemSprites.FirstOrDefault(item => item.ItemType == ItemType && item.StageMaterialType == StageMaterialType).Sprite;
             
            var ManagementDeskData = DataManager.Instance.ManagementDeskData.GetData();
            var ManagementDeskOption1ValueData = DataManager.Instance.ManagementDeskOption1ValueData.GetData();
            var ManagementDeskOption2ValueData = DataManager.Instance.ManagementDeskOption2ValueData.GetData();
            var ManagementDeskOption1CostData = DataManager.Instance.ManagementDeskOption1CostData.GetData();
            var ManagementDeskOption2CostData = DataManager.Instance.ManagementDeskOption2CostData.GetData();

            CurrentManagementDeskOption1Value = Enumerable.Range(0, ManagementDeskOption1ValueData.GetLength(0))
             .Where(i =>
                 ManagementDeskOption1ValueData[i, 1] == $"{BuildingType}" &&
                 ManagementDeskOption1ValueData[i, 2] == VolatileDataManager.Instance.CurrentStageLevel.ToString() &&
                 ManagementDeskOption1ValueData[i, 3] == CurrentManagementDeskOption1Level.ToString())
             .Select(i => ParserModule.ParseOrDefault(ManagementDeskOption1ValueData[i, 4], CurrentManagementDeskOption1Value))
             .FirstOrDefault();

            CurrentManagementDeskOption2Value = Enumerable.Range(0, ManagementDeskOption2ValueData.GetLength(0))
             .Where(i =>
                 ManagementDeskOption2ValueData[i, 1] == $"{BuildingType}" &&
                 ManagementDeskOption2ValueData[i, 2] == VolatileDataManager.Instance.CurrentStageLevel.ToString() &&
                 ManagementDeskOption2ValueData[i, 3] == CurrentManagementDeskOption2Level.ToString())
             .Select(i => ParserModule.ParseOrDefault(ManagementDeskOption2ValueData[i, 4], CurrentManagementDeskOption2Value))
             .FirstOrDefault();

            NextManagementDeskOption1Value = Enumerable.Range(0, ManagementDeskOption1ValueData.GetLength(0))
             .Where(i =>
                 ManagementDeskOption1ValueData[i, 1] == $"{BuildingType}" &&
                 ManagementDeskOption1ValueData[i, 2] == VolatileDataManager.Instance.CurrentStageLevel.ToString() &&
                 ManagementDeskOption1ValueData[i, 3] == (CurrentManagementDeskOption1Level + 1).ToString())
             .Select(i => ParserModule.ParseOrDefault(ManagementDeskOption1ValueData[i, 4], NextManagementDeskOption1Value))
             .FirstOrDefault();

            NextManagementDeskOption2Value = Enumerable.Range(0, ManagementDeskOption2ValueData.GetLength(0))
             .Where(i =>
                 ManagementDeskOption2ValueData[i, 1] == $"{BuildingType}" &&
                 ManagementDeskOption2ValueData[i, 2] == VolatileDataManager.Instance.CurrentStageLevel.ToString() &&
                 ManagementDeskOption2ValueData[i, 3] == (CurrentManagementDeskOption2Level + 1).ToString())
             .Select(i => ParserModule.ParseOrDefault(ManagementDeskOption2ValueData[i, 4], NextManagementDeskOption2Value))
             .FirstOrDefault();

            RequiredGoldToUpgradeManagementDeskOption1 = Enumerable.Range(0, ManagementDeskOption1CostData.GetLength(0))
             .Where(i =>
                 ManagementDeskOption1CostData[i, 1] == $"{BuildingType}" &&
                 ManagementDeskOption1CostData[i, 2] == VolatileDataManager.Instance.CurrentStageLevel.ToString() &&
                 ManagementDeskOption1CostData[i, 3] == CurrentManagementDeskOption1Level.ToString())
             .Select(i => ParserModule.ParseOrDefault(ManagementDeskOption1CostData[i, 4], RequiredGoldToUpgradeManagementDeskOption1))
             .FirstOrDefault();

            RequiredGoldToUpgradeManagementDeskOption2 = Enumerable.Range(0, ManagementDeskOption2CostData.GetLength(0))
             .Where(i =>
                 ManagementDeskOption2CostData[i, 1] == $"{BuildingType}" &&
                 ManagementDeskOption2CostData[i, 2] == VolatileDataManager.Instance.CurrentStageLevel.ToString() &&
                 ManagementDeskOption2CostData[i, 3] == CurrentManagementDeskOption2Level.ToString())
             .Select(i => ParserModule.ParseOrDefault(ManagementDeskOption2CostData[i, 4], RequiredGoldToUpgradeManagementDeskOption2))
             .FirstOrDefault();

            RequiredOption1LevelToUpgradeManagementDeskLevel = Enumerable.Range(0, ManagementDeskData.GetLength(0))
                .Where(i =>
                    ManagementDeskData[i, 1] == $"{BuildingType}" &&
                    ManagementDeskData[i, 2] == VolatileDataManager.Instance.CurrentStageLevel.ToString() &&
                    ManagementDeskData[i, 5] == CurrentManagementDeskLevel.ToString())
                .Select(i => ParserModule.ParseOrDefault(ManagementDeskData[i, 6], RequiredOption1LevelToUpgradeManagementDeskLevel))
                .FirstOrDefault();
            
            RequiredManagementDeskLevelToUpgradeOption2 = Enumerable.Range(0, ManagementDeskOption2CostData.GetLength(0))
             .Where(i =>
                 ManagementDeskOption2CostData[i, 1] == $"{BuildingType}" &&
                 ManagementDeskOption2CostData[i, 2] == VolatileDataManager.Instance.CurrentStageLevel.ToString() &&
                 ManagementDeskOption2CostData[i, 3] == CurrentManagementDeskOption2Level.ToString())
             .Select(i => ParserModule.ParseOrDefault(ManagementDeskOption2CostData[i, 5], RequiredManagementDeskLevelToUpgradeOption2))
             .FirstOrDefault();

            MaxManagementDeskOption1Level = Enumerable.Range(0, ManagementDeskData.GetLength(0))
             .Where(i =>
                 ManagementDeskData[i, 1] == $"{BuildingType}" &&
                 ManagementDeskData[i, 2] == VolatileDataManager.Instance.CurrentStageLevel.ToString())
             .Select(i => ParserModule.ParseOrDefault(ManagementDeskData[i, 3], MaxManagementDeskOption1Level))
             .FirstOrDefault();

            MaxManagementDeskOption2Level = Enumerable.Range(0, ManagementDeskData.GetLength(0))
             .Where(i =>
                 ManagementDeskData[i, 1] == $"{BuildingType}" &&
                 ManagementDeskData[i, 2] == VolatileDataManager.Instance.CurrentStageLevel.ToString())
             .Select(i => ParserModule.ParseOrDefault(ManagementDeskData[i, 4], MaxManagementDeskOption1Level))
             .FirstOrDefault();
        }

        private void OnClickUpgradeButtonForManagementDeskOption1()
        {
            if (RequiredGoldToUpgradeManagementDeskOption1 <= CurrencyManager.Instance.Gold)
            {
                CurrencyManager.Instance.RemoveGold(RequiredGoldToUpgradeManagementDeskOption1);
                IncreaseCurrentManagementDeskOption1Level();
            }
        }

        private void OnClickUpgradeButtonForManagementDeskOption2()
        {
            if (RequiredGoldToUpgradeManagementDeskOption2 <= CurrencyManager.Instance.Gold)
            {
                CurrencyManager.Instance.RemoveGold(RequiredGoldToUpgradeManagementDeskOption2);
                IncreaseCurrentManagementDeskOption2Level();
            }
        }

        private void IncreaseCurrentManagementDeskOption1Level()
        {
            CurrentManagementDeskOption1Level++;
            
            if (CurrentManagementDeskOption1Level >= RequiredOption1LevelToUpgradeManagementDeskLevel)
            {
                CurrentManagementDeskLevel++;
            }

            UpdateManagementDeskStatsModule();
            GetUIManagementDeskEnhancement();
        }

        private void IncreaseCurrentManagementDeskOption2Level()
        {
            CurrentManagementDeskOption2Level++;

            UpdateManagementDeskStatsModule();
            GetUIManagementDeskEnhancement();
        }

        public void GetUIManagementDeskEnhancement()
        {
            UIManager.Instance.GetPanelBuildingEnhancement(
                ManagementDeskName,
                ManagementDeskSprite,
                ManagementDeskName,
                CurrentManagementDeskOption1Value,
                CurrentManagementDeskOption2Value,
                NextManagementDeskOption1Value,
                NextManagementDeskOption2Value,
                CurrentManagementDeskLevel,
                CurrentManagementDeskOption1Level,
                CurrentManagementDeskOption2Level,
                MaxManagementDeskOption1Level,
                MaxManagementDeskOption2Level,
                RequiredGoldToUpgradeManagementDeskOption1,
                RequiredGoldToUpgradeManagementDeskOption2,
                RequiredManagementDeskLevelToUpgradeOption2,
                OnClickUpgradeButtonForManagementDeskOption1,
                OnClickUpgradeButtonForManagementDeskOption2
            );
        }

        public void ReturnUIManagementDeskEnhancement()
        {
            UIManager.Instance.ReturnPanelBuildingEnhancement();
        }
    }
}