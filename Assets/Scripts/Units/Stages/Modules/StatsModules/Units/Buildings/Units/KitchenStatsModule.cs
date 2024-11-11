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
    public interface IKitchenStatsModule : IBuildingStatsModule
    {
    }

    public class KitchenStatsModule : BuildingStatsModule, IKitchenStatsModule
    {
        public readonly string BuildingKey;
        public readonly EBuildingType BuildingType;
        public readonly string InputItemKey;
        public readonly EItemType InputItemType;

        public readonly EItemType ItemType;
        public readonly EMaterialType MaterialType;
        public readonly string OutputItemKey;
        public readonly EItemType OutputItemType;
        public readonly EStageMaterialType StageMaterialType;

        // TODO : 데이터 저장해야함!
        public int CurrentKitchenLevel;
        public int CurrentKitchenOption1Level;
        public float CurrentKitchenOption1Value;
        public int CurrentKitchenOption2Level;
        public float CurrentKitchenOption2Value;
        public string KitchenProductName;
        public Sprite KitchenProductSprite;
        public int MaxKitchenOption1Level;
        public int MaxKitchenOption2Level;
        public float NextKitchenOption1Value;
        public float NextKitchenOption2Value;
        public int RequiredGoldToUpgradeKitchenOption1;
        public int RequiredGoldToUpgradeKitchenOption2;
        public int RequiredKitchenLevelToUpgradeOption2;
        public int RequiredOption1LevelToUpgradeKitchenDeskLevel;

        public KitchenStatsModule(KitchenDataSO kitchenDataSo, KitchenCustomSetting kitchenCustomSetting) : base(
            kitchenDataSo)
        {
            MaterialType = kitchenCustomSetting.MaterialType;
            BuildingType = kitchenDataSo.BuildingType;
            InputItemType = kitchenCustomSetting.InputItemType;
            OutputItemType = kitchenCustomSetting.OutputItemType;

            BuildingKey = ParserModule.ParseEnumToString(BuildingType, MaterialType);
            InputItemKey = ParserModule.ParseEnumToString(InputItemType, MaterialType);
            OutputItemKey = ParserModule.ParseEnumToString(OutputItemType, MaterialType);

            ItemType = OutputItemType;
            StageMaterialType = VolatileDataManager.Instance.MaterialMappings[MaterialType];

            CurrentKitchenLevel = 1;
            CurrentKitchenOption1Level = 1;
            CurrentKitchenOption2Level = 1;

            MaxMaterialInventorySize = kitchenDataSo.BaseMaterialInventorySize;

            UpdateKitchenStatsModule();
        }

        public string KitchenName => $"Kitchen {MaterialType}";

        public int MaxMaterialInventorySize { get; }

        private void UpdateKitchenStatsModule()
        {
            KitchenProductName = $"{VolatileDataManager.Instance.MaterialMappings[MaterialType]}";
            KitchenProductSprite = DataManager.Instance.ItemDataSo.ItemSprites.FirstOrDefault(item =>
                item.ItemType == ItemType && item.StageMaterialType == StageMaterialType).Sprite;

            var KitchenData = DataManager.Instance.KitchenData.GetData();
            var KitchenOption1ValueData = DataManager.Instance.KitchenOption1ValueData.GetData();
            var KitchenOption2ValueData = DataManager.Instance.KitchenOption2ValueData.GetData();
            var KitchenOption1CostData = DataManager.Instance.KitchenOption1CostData.GetData();
            var KitchenOption2CostData = DataManager.Instance.KitchenOption2CostData.GetData();

            CurrentKitchenOption1Value = Enumerable.Range(0, KitchenOption1ValueData.GetLength(0))
                .Where(i =>
                    KitchenOption1ValueData[i, 1] == $"{BuildingKey}" &&
                    KitchenOption1ValueData[i, 2] == VolatileDataManager.Instance.CurrentStageLevel.ToString() &&
                    KitchenOption1ValueData[i, 3] == CurrentKitchenOption1Level.ToString())
                .Select(i => ParserModule.ParseOrDefault(KitchenOption1ValueData[i, 4], CurrentKitchenOption1Value))
                .FirstOrDefault();

            CurrentKitchenOption2Value = Enumerable.Range(0, KitchenOption2ValueData.GetLength(0))
                .Where(i =>
                    KitchenOption2ValueData[i, 1] == $"{BuildingKey}" &&
                    KitchenOption2ValueData[i, 2] == VolatileDataManager.Instance.CurrentStageLevel.ToString() &&
                    KitchenOption2ValueData[i, 3] == CurrentKitchenOption2Level.ToString())
                .Select(i => ParserModule.ParseOrDefault(KitchenOption2ValueData[i, 4], CurrentKitchenOption2Value))
                .FirstOrDefault();

            NextKitchenOption1Value = Enumerable.Range(0, KitchenOption1ValueData.GetLength(0))
                .Where(i =>
                    KitchenOption1ValueData[i, 1] == $"{BuildingKey}" &&
                    KitchenOption1ValueData[i, 2] == VolatileDataManager.Instance.CurrentStageLevel.ToString() &&
                    KitchenOption1ValueData[i, 3] == (CurrentKitchenOption1Level + 1).ToString())
                .Select(i => ParserModule.ParseOrDefault(KitchenOption1ValueData[i, 4], NextKitchenOption1Value))
                .FirstOrDefault();

            NextKitchenOption2Value = Enumerable.Range(0, KitchenOption2ValueData.GetLength(0))
                .Where(i =>
                    KitchenOption2ValueData[i, 1] == $"{BuildingKey}" &&
                    KitchenOption2ValueData[i, 2] == VolatileDataManager.Instance.CurrentStageLevel.ToString() &&
                    KitchenOption2ValueData[i, 3] == (CurrentKitchenOption2Level + 1).ToString())
                .Select(i => ParserModule.ParseOrDefault(KitchenOption2ValueData[i, 4], NextKitchenOption2Value))
                .FirstOrDefault();

            RequiredGoldToUpgradeKitchenOption1 = Enumerable.Range(0, KitchenOption1CostData.GetLength(0))
                .Where(i =>
                    KitchenOption1CostData[i, 1] == $"{BuildingType}_{MaterialType}" &&
                    KitchenOption1CostData[i, 2] == VolatileDataManager.Instance.CurrentStageLevel.ToString() &&
                    KitchenOption1CostData[i, 3] == CurrentKitchenOption1Level.ToString())
                .Select(i =>
                    ParserModule.ParseOrDefault(KitchenOption1CostData[i, 4], RequiredGoldToUpgradeKitchenOption1))
                .FirstOrDefault();

            RequiredGoldToUpgradeKitchenOption2 = Enumerable.Range(0, KitchenOption2CostData.GetLength(0))
                .Where(i =>
                    KitchenOption2CostData[i, 1] == $"{BuildingType}_{MaterialType}" &&
                    KitchenOption2CostData[i, 2] == VolatileDataManager.Instance.CurrentStageLevel.ToString() &&
                    KitchenOption2CostData[i, 3] == CurrentKitchenOption2Level.ToString())
                .Select(i =>
                    ParserModule.ParseOrDefault(KitchenOption2CostData[i, 4], RequiredGoldToUpgradeKitchenOption2))
                .FirstOrDefault();

            RequiredOption1LevelToUpgradeKitchenDeskLevel = Enumerable.Range(0, KitchenData.GetLength(0))
                .Where(i =>
                    KitchenData[i, 1] == $"{BuildingType}_{MaterialType}" &&
                    KitchenData[i, 2] == VolatileDataManager.Instance.CurrentStageLevel.ToString() &&
                    KitchenData[i, 5] == CurrentKitchenLevel.ToString())
                .Select(i =>
                    ParserModule.ParseOrDefault(KitchenData[i, 6], RequiredOption1LevelToUpgradeKitchenDeskLevel))
                .FirstOrDefault();

            RequiredKitchenLevelToUpgradeOption2 = Enumerable.Range(0, KitchenOption2CostData.GetLength(0))
                .Where(i =>
                    KitchenOption2CostData[i, 1] == $"{BuildingType}_{MaterialType}" &&
                    KitchenOption2CostData[i, 2] == VolatileDataManager.Instance.CurrentStageLevel.ToString() &&
                    KitchenOption2CostData[i, 3] == CurrentKitchenOption2Level.ToString())
                .Select(i =>
                    ParserModule.ParseOrDefault(KitchenOption2CostData[i, 5], RequiredKitchenLevelToUpgradeOption2))
                .FirstOrDefault();

            MaxKitchenOption1Level = Enumerable.Range(0, KitchenData.GetLength(0))
                .Where(i =>
                    KitchenData[i, 1] == $"{BuildingType}_{MaterialType}" &&
                    KitchenData[i, 2] == VolatileDataManager.Instance.CurrentStageLevel.ToString())
                .Select(i => ParserModule.ParseOrDefault(KitchenData[i, 3], MaxKitchenOption1Level))
                .FirstOrDefault();

            MaxKitchenOption2Level = Enumerable.Range(0, KitchenData.GetLength(0))
                .Where(i =>
                    KitchenData[i, 1] == $"{BuildingType}_{MaterialType}" &&
                    KitchenData[i, 2] == VolatileDataManager.Instance.CurrentStageLevel.ToString())
                .Select(i => ParserModule.ParseOrDefault(KitchenData[i, 4], MaxKitchenOption1Level))
                .FirstOrDefault();


            UpdateProductPrice();
        }

        private void OnClickUpgradeButtonForKitchenOption1()
        {
            if (RequiredGoldToUpgradeKitchenOption1 <= CurrencyManager.Instance.Gold)
            {
                CurrencyManager.Instance.RemoveGold(RequiredGoldToUpgradeKitchenOption1);
                IncreaseCurrentKitchenOption1Level();
            }
        }

        private void OnClickUpgradeButtonForKitchenOption2()
        {
            if (RequiredGoldToUpgradeKitchenOption2 <= CurrencyManager.Instance.Gold)
            {
                CurrencyManager.Instance.RemoveGold(RequiredGoldToUpgradeKitchenOption2);
                IncreaseCurrentKitchenOption2Level();
            }
        }

        private void IncreaseCurrentKitchenOption1Level()
        {
            CurrentKitchenOption1Level++;

            if (CurrentKitchenOption1Level >= RequiredOption1LevelToUpgradeKitchenDeskLevel) CurrentKitchenLevel++;

            UpdateKitchenStatsModule();
            GetUIKitchenEnhancement();
        }

        private void IncreaseCurrentKitchenOption2Level()
        {
            CurrentKitchenOption2Level++;

            UpdateKitchenStatsModule();
            GetUIKitchenEnhancement();
        }

        public void GetUIKitchenEnhancement()
        {
            UIManager.Instance.GetPanelBuildingEnhancement(
                KitchenName,
                KitchenProductSprite,
                KitchenProductName,
                CurrentKitchenOption1Value,
                CurrentKitchenOption2Value,
                NextKitchenOption1Value,
                NextKitchenOption2Value,
                CurrentKitchenLevel,
                CurrentKitchenOption1Level,
                CurrentKitchenOption2Level,
                MaxKitchenOption1Level,
                MaxKitchenOption2Level,
                RequiredGoldToUpgradeKitchenOption1,
                RequiredGoldToUpgradeKitchenOption2,
                RequiredKitchenLevelToUpgradeOption2,
                OnClickUpgradeButtonForKitchenOption1,
                OnClickUpgradeButtonForKitchenOption2
            );
        }

        public void ReturnUIKitchenEnhancement()
        {
            UIManager.Instance.ReturnPanelBuildingEnhancement();
        }

        private void UpdateProductPrice()
        {
            if (VolatileDataManager.Instance.ItemPrices.ContainsKey(OutputItemKey))
                VolatileDataManager.Instance.ItemPrices[OutputItemKey] = (int)CurrentKitchenOption1Value;
            else
                VolatileDataManager.Instance.ItemPrices.TryAdd(OutputItemKey, (int)CurrentKitchenOption1Value);
        }
    }
}