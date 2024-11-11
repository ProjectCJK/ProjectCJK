using System.Linq;
using Managers;
using ScriptableObjects.Scripts.Buildings.Units;
using Units.Stages.Modules.StatsModules.Units.Buildings.Abstract;
using Units.Stages.Units.Buildings.Enums;
using Units.Stages.Units.Buildings.Units;
using Units.Stages.Units.Items.Enums;
using UnityEngine;

namespace Units.Stages.Modules.StatsModules.Units.Buildings.Units
{
    public interface IWareHouseStatsModule : IBuildingStatsModule
    {
    }

    public class WareHouseStatsModule : BuildingStatsModule, IWareHouseStatsModule
    {
        public readonly string BuildingKey;
        public readonly EBuildingType BuildingType;

        public readonly EItemType ItemType;
        public readonly EMaterialType MaterialType;

        // TODO : 데이터 저장해야함!
        public int CurrentWareHouseLevel;
        public int CurrentWareHouseOption1Level;
        public float CurrentWareHouseOption1Value;
        public int CurrentWareHouseOption2Level;
        public float CurrentWareHouseOption2Value;
        public int MaxWareHouseOption1Level;
        public int MaxWareHouseOption2Level;
        public float NextWareHouseOption1Value;
        public float NextWareHouseOption2Value;
        public int RequiredGoldToUpgradeWareHouseOption1;
        public int RequiredGoldToUpgradeWareHouseOption2;
        public int RequiredOption1LevelToUpgradeWareHouseLevel;
        public int RequiredWareHouseLevelToUpgradeOption2;
        public string WareHouseProductName;
        public Sprite WareHouseProductSprite;

        public WareHouseStatsModule(WareHouseDataSO wareHouseDataSo, WareHouseCustomSetting wareHouseCustomSetting) :
            base(wareHouseDataSo)
        {
            BuildingType = wareHouseDataSo.BuildingType;

            BuildingKey = ParserModule.ParseEnumToString(BuildingType);

            CurrentWareHouseLevel = 1;
            CurrentWareHouseOption1Level = 1;
            CurrentWareHouseOption2Level = 1;

            MaxMaterialInventorySize = wareHouseDataSo.BaseMaterialInventorySize;

            UpdateWareHouseStatsModule();
        }

        public string WareHouseName => "WareHouse";

        public int MaxMaterialInventorySize { get; }

        private void UpdateWareHouseStatsModule()
        {
            var WareHouseData = DataManager.Instance.WareHouseData.GetData();
            var WareHouseOption1ValueData = DataManager.Instance.WareHouseOption1ValueData.GetData();
            var WareHouseOption2ValueData = DataManager.Instance.WareHouseOption2ValueData.GetData();
            var WareHouseOption1CostData = DataManager.Instance.WareHouseOption1CostData.GetData();
            var WareHouseOption2CostData = DataManager.Instance.WareHouseOption2CostData.GetData();

            CurrentWareHouseOption1Value = Enumerable.Range(0, WareHouseOption1ValueData.GetLength(0))
                .Where(i =>
                    WareHouseOption1ValueData[i, 1] == $"{BuildingKey}" &&
                    WareHouseOption1ValueData[i, 2] == VolatileDataManager.Instance.CurrentStageLevel.ToString() &&
                    WareHouseOption1ValueData[i, 3] == CurrentWareHouseOption1Level.ToString())
                .Select(i => ParserModule.ParseOrDefault(WareHouseOption1ValueData[i, 4], CurrentWareHouseOption1Value))
                .FirstOrDefault();

            CurrentWareHouseOption2Value = Enumerable.Range(0, WareHouseOption2ValueData.GetLength(0))
                .Where(i =>
                    WareHouseOption2ValueData[i, 1] == $"{BuildingKey}" &&
                    WareHouseOption2ValueData[i, 2] == VolatileDataManager.Instance.CurrentStageLevel.ToString() &&
                    WareHouseOption2ValueData[i, 3] == CurrentWareHouseOption2Level.ToString())
                .Select(i => ParserModule.ParseOrDefault(WareHouseOption2ValueData[i, 4], CurrentWareHouseOption2Value))
                .FirstOrDefault();

            NextWareHouseOption1Value = Enumerable.Range(0, WareHouseOption1ValueData.GetLength(0))
                .Where(i =>
                    WareHouseOption1ValueData[i, 1] == $"{BuildingKey}" &&
                    WareHouseOption1ValueData[i, 2] == VolatileDataManager.Instance.CurrentStageLevel.ToString() &&
                    WareHouseOption1ValueData[i, 3] == (CurrentWareHouseOption1Level + 1).ToString())
                .Select(i => ParserModule.ParseOrDefault(WareHouseOption1ValueData[i, 4], NextWareHouseOption1Value))
                .FirstOrDefault();

            NextWareHouseOption2Value = Enumerable.Range(0, WareHouseOption2ValueData.GetLength(0))
                .Where(i =>
                    WareHouseOption2ValueData[i, 1] == $"{BuildingKey}" &&
                    WareHouseOption2ValueData[i, 2] == VolatileDataManager.Instance.CurrentStageLevel.ToString() &&
                    WareHouseOption2ValueData[i, 3] == (CurrentWareHouseOption2Level + 1).ToString())
                .Select(i => ParserModule.ParseOrDefault(WareHouseOption2ValueData[i, 4], NextWareHouseOption2Value))
                .FirstOrDefault();

            RequiredGoldToUpgradeWareHouseOption1 = Enumerable.Range(0, WareHouseOption1CostData.GetLength(0))
                .Where(i =>
                    WareHouseOption1CostData[i, 1] == $"{BuildingKey}" &&
                    WareHouseOption1CostData[i, 2] == VolatileDataManager.Instance.CurrentStageLevel.ToString() &&
                    WareHouseOption1CostData[i, 3] == CurrentWareHouseOption1Level.ToString())
                .Select(i =>
                    ParserModule.ParseOrDefault(WareHouseOption1CostData[i, 4], RequiredGoldToUpgradeWareHouseOption1))
                .FirstOrDefault();

            RequiredGoldToUpgradeWareHouseOption2 = Enumerable.Range(0, WareHouseOption2CostData.GetLength(0))
                .Where(i =>
                    WareHouseOption2CostData[i, 1] == $"{BuildingKey}" &&
                    WareHouseOption2CostData[i, 2] == VolatileDataManager.Instance.CurrentStageLevel.ToString() &&
                    WareHouseOption2CostData[i, 3] == CurrentWareHouseOption2Level.ToString())
                .Select(i =>
                    ParserModule.ParseOrDefault(WareHouseOption2CostData[i, 4], RequiredGoldToUpgradeWareHouseOption2))
                .FirstOrDefault();

            RequiredOption1LevelToUpgradeWareHouseLevel = Enumerable.Range(0, WareHouseData.GetLength(0))
                .Where(i =>
                    WareHouseData[i, 1] == $"{BuildingKey}" &&
                    WareHouseData[i, 2] == VolatileDataManager.Instance.CurrentStageLevel.ToString() &&
                    WareHouseData[i, 5] == CurrentWareHouseLevel.ToString())
                .Select(i =>
                    ParserModule.ParseOrDefault(WareHouseData[i, 6], RequiredOption1LevelToUpgradeWareHouseLevel))
                .FirstOrDefault();

            RequiredWareHouseLevelToUpgradeOption2 = Enumerable.Range(0, WareHouseOption2CostData.GetLength(0))
                .Where(i =>
                    WareHouseOption2CostData[i, 1] == $"{BuildingKey}" &&
                    WareHouseOption2CostData[i, 2] == VolatileDataManager.Instance.CurrentStageLevel.ToString() &&
                    WareHouseOption2CostData[i, 3] == CurrentWareHouseOption2Level.ToString())
                .Select(i =>
                    ParserModule.ParseOrDefault(WareHouseOption2CostData[i, 5], RequiredWareHouseLevelToUpgradeOption2))
                .FirstOrDefault();

            MaxWareHouseOption1Level = Enumerable.Range(0, WareHouseData.GetLength(0))
                .Where(i =>
                    WareHouseData[i, 1] == $"{BuildingKey}" &&
                    WareHouseData[i, 2] == VolatileDataManager.Instance.CurrentStageLevel.ToString())
                .Select(i => ParserModule.ParseOrDefault(WareHouseData[i, 3], MaxWareHouseOption1Level))
                .FirstOrDefault();

            MaxWareHouseOption2Level = Enumerable.Range(0, WareHouseData.GetLength(0))
                .Where(i =>
                    WareHouseData[i, 1] == $"{BuildingKey}" &&
                    WareHouseData[i, 2] == VolatileDataManager.Instance.CurrentStageLevel.ToString())
                .Select(i => ParserModule.ParseOrDefault(WareHouseData[i, 4], MaxWareHouseOption1Level))
                .FirstOrDefault();
        }

        private void OnClickUpgradeButtonForWareHouseOption1()
        {
            if (RequiredGoldToUpgradeWareHouseOption1 <= CurrencyManager.Instance.Gold)
            {
                CurrencyManager.Instance.RemoveGold(RequiredGoldToUpgradeWareHouseOption1);
                IncreaseCurrentWareHouseOption1Level();
            }
        }

        private void OnClickUpgradeButtonForWareHouseOption2()
        {
            if (RequiredGoldToUpgradeWareHouseOption2 <= CurrencyManager.Instance.Gold)
            {
                CurrencyManager.Instance.RemoveGold(RequiredGoldToUpgradeWareHouseOption2);
                IncreaseCurrentWareHouseOption2Level();
            }
        }

        private void IncreaseCurrentWareHouseOption1Level()
        {
            CurrentWareHouseOption1Level++;

            if (CurrentWareHouseOption1Level >= RequiredOption1LevelToUpgradeWareHouseLevel) CurrentWareHouseLevel++;

            UpdateWareHouseStatsModule();
            GetUIWareHouseEnhancement();
        }

        private void IncreaseCurrentWareHouseOption2Level()
        {
            CurrentWareHouseOption2Level++;

            UpdateWareHouseStatsModule();
            GetUIWareHouseEnhancement();
        }

        public void GetUIWareHouseEnhancement()
        {
            UIManager.Instance.GetPanelBuildingEnhancement(
                WareHouseName,
                WareHouseProductSprite,
                WareHouseProductName,
                CurrentWareHouseOption1Value,
                CurrentWareHouseOption2Value,
                NextWareHouseOption1Value,
                NextWareHouseOption2Value,
                CurrentWareHouseLevel,
                CurrentWareHouseOption1Level,
                CurrentWareHouseOption2Level,
                MaxWareHouseOption1Level,
                MaxWareHouseOption2Level,
                RequiredGoldToUpgradeWareHouseOption1,
                RequiredGoldToUpgradeWareHouseOption2,
                RequiredWareHouseLevelToUpgradeOption2,
                OnClickUpgradeButtonForWareHouseOption1,
                OnClickUpgradeButtonForWareHouseOption2
            );
        }

        public void ReturnUIWareHouseEnhancement()
        {
            UIManager.Instance.ReturnPanelBuildingEnhancement();
        }
    }
}